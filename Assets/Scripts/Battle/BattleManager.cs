using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public enum BattleMenuState
{
    Root,  // たたかう/さくせん/にげる
    Fight, // こうげき/じゅもん/とくぎ/ぼうぎょ
    Spell, // メラ/ギラ/ホイミ/もどる
    Busy   // 演出中(入力不可)
}

public class BattleManager : MonoBehaviour
{
    public static int[] NextEnemyIDs = new int[] { 0, 1, 2 };

    [Header("EnemyData")]
    public EnemyDatabase EnemyDB;
    private List<EnemyInstance> enemies = new List<EnemyInstance>();

    [Header("Enemy / Player Visual")]
    public Transform EnemyModelRoot;
    public Transform PlayerModelRoot;
    private GameObject playerModelInstance;

    [Header("PlayerStatus と LevelSystem の参照")]
    public PlayerStatus PlayerStatus;
    public LevelSystem LevelSystem;

    [Header("PlayerData")]
    public float PlayerMaxHP = 30f;
    public float PlayerHP = 30;
    public float PlayerAttackMin = 5;
    public float PlayerAttackMax = 10;
    public float PlayerMaxMP = 10f;
    public float PlayerMP = 10;

    [Header("UI")]
    public TextMeshProUGUI PlayerHPText;
    public TextMeshProUGUI PlayerMPText;
    public TextMeshProUGUI EnemyNameText;
    public TextMeshProUGUI EnemyHPText;
    public TextMeshProUGUI DialogText;

    [Header("Player Damage Effects")]
    public UnityEngine.UI.Image HitFlashImage;
    public Camera MainCamera;
    public Color HitFlashColor = new Color(1, 0, 0, 0.5f);
    public float HitFlashDuration = 0.2f;
    public float HitShakeIntensity = 0.2f;
    public float HitShakeDuration = 0.3f;

    [Header("DQ Like Menu")]
    public GameObject RootmenuPanel;
    public Transform RootMenuRoot;
    public GameObject FightMenuPanel;
    public Transform FightMenuRoot;

    public MenuButton MenuButtonPrefab;
    public GameObject EnemyUIPrefab; // 頭上UIのプレハブ

    private BattleMenuState menuState = BattleMenuState.Root;
    private bool isGuarding = false;
    private bool isPlayerTurn = true;

    void Start()
    {
        SetupEnemiesFromDB();
        ApplyPlayerStatus();
        UpdateUI();
        BuildRootMenu();
        SpawnEnemyModels();
        SpawnPlayerModel();

        if (enemies.Count > 1)
        {
            DialogText.text = "魔物たちが現れた!";
        }
        else if (enemies.Count == 1)
        {
            DialogText.text = $"{enemies[0].Data.DisplayName}が現れた!";
        }
    }

    public void ApplyPlayerStatus()
    {
        if (PlayerStatus == null) return;
        PlayerMaxHP = PlayerState.Instance.MaxHP;
        PlayerHP = Mathf.Min(PlayerHP, PlayerMaxHP);
        PlayerAttackMin = PlayerState.Instance.AttackMin;
        PlayerAttackMax = PlayerState.Instance.AttackMax;
        PlayerMaxMP = PlayerStatus.MaxMP;
        PlayerMP = Mathf.Min(PlayerMP, PlayerMaxMP);
    }

    private void SetupEnemiesFromDB()
    {
        if (EnemyDB == null)
        {
            Debug.LogError("EnemyDBが設定されていません");
            return;
        }

        enemies.Clear();
        foreach (int id in NextEnemyIDs)
        {
            var data = EnemyDB.GetByID(id);
            if (data != null)
            {
                enemies.Add(new EnemyInstance(data));
            }
        }

        if (enemies.Count == 0)
        {
            Debug.LogError("有効な敵データが見つかりません");
        }
    }

    private void SetMenuState(BattleMenuState state)
    {
        menuState = state;

        if (RootmenuPanel != null)
        {
            RootmenuPanel.SetActive(state == BattleMenuState.Root);
        }

        if (FightMenuPanel != null)
        {
            FightMenuPanel.SetActive(state == BattleMenuState.Fight || state == BattleMenuState.Spell);
        }

        if (state == BattleMenuState.Busy)
        {
            if (RootmenuPanel != null) RootmenuPanel.SetActive(false);
            if (FightMenuPanel != null) FightMenuPanel.SetActive(false);
        }
    }

    private void BuildRootMenu()
    {
        ClearChildren(RootMenuRoot);
        CreateButton(RootMenuRoot, "たたかう", () =>
        {
            if (!isPlayerTurn) return;
            BuildFightMenu();
            SetMenuState(BattleMenuState.Fight);
            DialogText.text = "どうする？";
        });

        CreateButton(RootMenuRoot, "アイテム", () =>
        {
           if (!isPlayerTurn) return;
           if (!InventryManager.Instance.UseItem("ポーション（回復薬）"))
            {
                DialogText.text = "ポーションがない";
                return;

            }

            StartCoroutine(ExecuteUseItem());

            


        });


        CreateButton(RootMenuRoot, "にげる", () =>
        {
            if (!isPlayerTurn) return;
            StartCoroutine(TryEscape());
        });
    }

    private void BuildFightMenu()
    {
        ClearChildren(FightMenuRoot);
        CreateButton(FightMenuRoot, "こうげき", () =>
        {
            if (!isPlayerTurn) return;
            StartCoroutine(ExecuteAttack());
        });

        CreateButton(FightMenuRoot, "じゅもん", () =>
        {
            if (!isPlayerTurn) return;
            BuildSpellMenu();
            SetMenuState(BattleMenuState.Spell);
        });

        CreateButton(FightMenuRoot, "とくぎ", () =>
        {
            if (!isPlayerTurn) return;
            StartCoroutine(ExecutePowerSkill());
        });

        CreateButton(FightMenuRoot, "ぼうぎょ", () =>
        {
            if (!isPlayerTurn) return;
            StartCoroutine(ExecuteGurad());
        });

        CreateButton(FightMenuRoot, "もどる", () =>
        {
            SetMenuState(BattleMenuState.Root);
            DialogText.text = "どうする";
        });
    }

    private void BuildSpellMenu()
    {
        ClearChildren(FightMenuRoot);
        CreateButton(FightMenuRoot, "メラ (2MP)", () => CastSpell(2, () => StartCoroutine(ExecuteMera())));
        CreateButton(FightMenuRoot, "ギラ (4MP)", () => CastSpell(4, () => StartCoroutine(ExecuteGira())));
        CreateButton(FightMenuRoot, "ホイミ (3MP)", () => CastSpell(3, () => StartCoroutine(ExecuteHoimi())));
        CreateButton(FightMenuRoot, "もどる", () =>
        {
            BuildFightMenu();
            SetMenuState(BattleMenuState.Fight);
        });
    }

    private void CastSpell(int cost, System.Action spellEffect)
    {
        if (!isPlayerTurn) return;
        if (PlayerMP < cost)
        {
            DialogText.text = "MPがたりない！";
            return;
        }
        PlayerMP -= cost;
        UpdateUI();
        spellEffect.Invoke();
    }

    private System.Collections.IEnumerator ExecuteAttack()
    {
        isPlayerTurn = false;
        SetMenuState(BattleMenuState.Busy);

        EnemyInstance target = GetFirstLivingEnemy();
        if (target == null) yield break;

        DialogText.text = "プレイヤーの攻撃!";
        yield return StartCoroutine(TriggerPlayerActionEffects("Attack"));
        yield return new WaitForSeconds(0.4f);

        var damage = Mathf.Ceil(Random.Range(PlayerAttackMin, PlayerAttackMax));
        target.CurrentHP -= damage;

        if (target.Animator != null)
        {
            target.Animator.SetTrigger("Damage");
            StartCoroutine(FlashColor(target, Color.red, 0.2f));
        }

        DialogText.text = $"{target.Data.DisplayName}に{damage}ダメージ!";
        UpdateUI();
        yield return new WaitForSeconds(0.6f);

        if (target.IsDead)
        {
            DialogText.text = $"{target.Data.DisplayName}を倒した!";
            if (target.Animator != null) target.Animator.SetTrigger("Die");
            yield return new WaitForSeconds(1f);
        }

        if (AreAllEnemiesDead())
        {
            Victory();
        }
        else
        {
            StartCoroutine(EnemyTurn());
        }
    }

    private EnemyInstance GetFirstLivingEnemy()
    {
        return enemies.Find(e => !e.IsDead);
    }

    private bool AreAllEnemiesDead()
    {
        return enemies.TrueForAll(e => e.IsDead);
    }

    private System.Collections.IEnumerator ExecuteGurad()
    {
        isPlayerTurn = false;
        SetMenuState(BattleMenuState.Busy);
        isGuarding = true;
        DialogText.text = "身を守っている!";
        yield return new WaitForSeconds(1f);
        StartCoroutine(EnemyTurn());
    }

    private System.Collections.IEnumerator ExecuteUseItem()
    {
        isPlayerTurn = false;
        float heal = InventryManager.Instance.GetItemDate(
     "ポーション（回復薬）").Power;
        PlayerHP += heal;
        if (PlayerHP > PlayerMaxHP)
        {

            PlayerHP = PlayerMaxHP;

        }

        DialogText.text = $"HPが{heal}回復した";
        UpdateUI();

        yield return new WaitForSeconds(0.8f);


        SetMenuState(BattleMenuState.Busy);



        StartCoroutine(EnemyTurn());
    }

    private System.Collections.IEnumerator TryEscape()
    {
        bool success = Random.value < 0.5f;
        if (success)
        {
            DialogText.text = "うまくにげきれた";
            Invoke(nameof(ReturnToField), 1.2f);
        }
        else
        {
            DialogText.text = "まわりこまれた";
            isPlayerTurn = false;
            SetMenuState(BattleMenuState.Busy);
            yield return new WaitForSeconds(0.8f);
            StartCoroutine(EnemyTurn());
        }
    }

    private System.Collections.IEnumerator ExecuteHoimi()
    {
        isPlayerTurn = false;
        SetMenuState(BattleMenuState.Busy);
        DialogText.text = "ホイミ！";
        yield return new WaitForSeconds(0.6f);

        float heal = Mathf.CeilToInt(PlayerMaxHP * 0.25f) + 5;
        PlayerHP = Mathf.Min(PlayerMaxHP, PlayerHP + heal);
        
        // 回復演出
        yield return StartCoroutine(FlashScreen(0.4f, new Color(0, 1, 0, 0.4f))); // Green

        DialogText.text = $"HPが{heal}かいふくした！";
        UpdateUI();
        yield return new WaitForSeconds(0.8f);
        StartCoroutine(EnemyTurn());
    }

    private System.Collections.IEnumerator ExecuteMera()
    {
        isPlayerTurn = false;
        SetMenuState(BattleMenuState.Busy);

        EnemyInstance target = GetFirstLivingEnemy();
        if (target == null) yield break;

        DialogText.text = "メラ！";
        yield return StartCoroutine(FlashScreen(0.3f, new Color(1, 0.4f, 0, 0.4f))); // Orange
        yield return new WaitForSeconds(0.3f);

        var damage = Mathf.Ceil(Random.Range(10, 15));
        target.CurrentHP -= damage;

        if (target.Animator != null)
        {
            target.Animator.SetTrigger("Damage");
            StartCoroutine(FlashColor(target, new Color(1, 0.5f, 0), 0.2f)); // Orange
        }

        DialogText.text = $"{target.Data.DisplayName}に{damage}のダメージ！";
        UpdateUI();
        yield return new WaitForSeconds(0.8f);

        if (target.IsDead)
        {
            DialogText.text = $"{target.Data.DisplayName}を倒した!";
            if (target.Animator != null) target.Animator.SetTrigger("Die");
            yield return new WaitForSeconds(1f);
        }

        CheckBattleEnd();
    }

    private System.Collections.IEnumerator ExecuteGira()
    {
        isPlayerTurn = false;
        SetMenuState(BattleMenuState.Busy);

        DialogText.text = "ギラ！";
        yield return StartCoroutine(FlashScreen(0.4f, new Color(1, 1, 0, 0.3f))); // Yellow
        yield return new WaitForSeconds(0.4f);

        List<EnemyInstance> livingEnemies = enemies.FindAll(e => !e.IsDead);
        foreach (var target in livingEnemies)
        {
            var damage = Mathf.Ceil(Random.Range(7, 12));
            target.CurrentHP -= damage;

            if (target.Animator != null)
            {
                target.Animator.SetTrigger("Damage");
                StartCoroutine(FlashColor(target, Color.yellow, 0.2f));
            }
        }

        DialogText.text = "まものたちを焼き払った！";
        UpdateUI();
        yield return new WaitForSeconds(0.8f);

        foreach (var target in livingEnemies)
        {
            if (target.IsDead)
            {
                DialogText.text = $"{target.Data.DisplayName}を倒した!";
                if (target.Animator != null) target.Animator.SetTrigger("Die");
                yield return new WaitForSeconds(0.5f);
            }
        }

        CheckBattleEnd();
    }

    private void CheckBattleEnd()
    {
        if (AreAllEnemiesDead())
        {
            Victory();
        }
        else
        {
            StartCoroutine(EnemyTurn());
        }
    }

    private System.Collections.IEnumerator ExecutePowerSkill()
    {
        isPlayerTurn = false;
        SetMenuState(BattleMenuState.Busy);

        EnemyInstance target = GetFirstLivingEnemy();
        if (target == null) yield break;

        DialogText.text = "つよく切り付けた";
        yield return StartCoroutine(TriggerPlayerActionEffects("Attack"));
        yield return StartCoroutine(ShakeCamera(0.2f, 0.15f));
        yield return new WaitForSeconds(0.4f);

        var damage = Mathf.Ceil(Random.Range(PlayerAttackMin, PlayerAttackMax) * 1.6f + 2);
        target.CurrentHP -= damage;

        if (target.Animator != null)
        {
            target.Animator.SetTrigger("Damage");
            StartCoroutine(FlashColor(target, Color.red, 0.2f));
        }

        DialogText.text = $"{target.Data.DisplayName}に{damage}ダメージ!";
        UpdateUI();
        yield return new WaitForSeconds(0.8f);

        if (target.IsDead)
        {
            DialogText.text = $"{target.Data.DisplayName}を倒した!";
            if (target.Animator != null) target.Animator.SetTrigger("Die");
            yield return new WaitForSeconds(1f);
        }

        if (AreAllEnemiesDead())
        {
            Victory();
        }
        else
        {
            StartCoroutine(EnemyTurn());
        }
    }

    void CreateButton(Transform root, string label, System.Action onClick)
    {
        if (MenuButtonPrefab == null || root == null) return;
        var btn = Instantiate(MenuButtonPrefab, root);
        btn.Setup(label, onClick);
    }

    void ClearChildren(Transform root)
    {
        if (root == null) return;
        for (int i = root.childCount - 1; i >= 0; i--)
        {
            Destroy(root.GetChild(i).gameObject);
        }
    }

    private void SpawnEnemyModels()
    {
        if (EnemyModelRoot == null) return;

        foreach (var enemy in enemies)
        {
            enemy.DestroyModel();
        }

        float spacing = 2.0f;
        float startX = -(enemies.Count - 1) * spacing * 0.5f;

        for (int i = 0; i < enemies.Count; i++)
        {
            var enemy = enemies[i];
            if (enemy.Data.ModelPrefab == null) continue;

            GameObject instance = Instantiate(enemy.Data.ModelPrefab, EnemyModelRoot);
            enemy.ModelInstance = instance;

            Vector3 pos = enemy.Data.ModelPosition;
            pos.x += startX + (i * spacing);
            
            instance.transform.localPosition = pos;
            instance.transform.localEulerAngles = enemy.Data.ModelRotation;
            instance.transform.localScale = enemy.Data.ModelScale;

            enemy.Animator = instance.GetComponentInChildren<Animator>();

            // 頭上UIの生成
            if (EnemyUIPrefab != null)
            {
                GameObject uiInstance = Instantiate(EnemyUIPrefab, instance.transform);
                uiInstance.transform.localPosition = enemy.Data.UIOffset;
                enemy.UIInstance = uiInstance;
            }
        }
    }

    private void SpawnPlayerModel()
    {
        if (PlayerModelRoot == null || PlayerStatus == null || PlayerStatus.PlayerModelPrefab == null) return;

        if (playerModelInstance != null)
        {
            Destroy(playerModelInstance);
        }

        playerModelInstance = Instantiate(PlayerStatus.PlayerModelPrefab, PlayerModelRoot);
        playerModelInstance.transform.localPosition = PlayerStatus.BattlePosition;
        playerModelInstance.transform.localEulerAngles = PlayerStatus.BattleRotation;
        playerModelInstance.transform.localScale = PlayerStatus.BattleScale;
    }

    public void OnAttackButton()
    {
        if (!isPlayerTurn) return;
        StartCoroutine(ExecuteAttack());
    }

    private System.Collections.IEnumerator EnemyTurn()
    {
        foreach (var enemy in enemies)
        {
            if (enemy.IsDead) continue;

            DialogText.text = $"{enemy.Data.DisplayName}の攻撃!";
            if (enemy.Animator != null)
            {
                enemy.Animator.SetTrigger("Attack");
            }

            yield return new WaitForSeconds(1f);

            var damage = Mathf.Ceil(Random.Range(enemy.Data.AttackMin, enemy.Data.AttackMax));
            if (isGuarding)
            {
                damage = Mathf.Ceil(damage * 0.5f);
                isGuarding = false;
            }

            PlayerHP -= damage;
            
            // プレイヤーへの攻撃演出をテキスト表示の前に行う
            yield return StartCoroutine(TriggerPlayerHitEffects());

            DialogText.text = $"{damage}ダメージ!";
            UpdateUI();

            yield return new WaitForSeconds(1f);

            if (PlayerHP <= 0f)
            {
                GameOver();
                yield break;
            }
        }

        isPlayerTurn = true;
        SetMenuState(BattleMenuState.Root);
        DialogText.text = "どうする";
    }

    public void UpdateUI()
    {
        PlayerHPText.text = $"HP:{PlayerHP}/{PlayerMaxHP}";
        if (PlayerMPText != null)
        {
            PlayerMPText.text = $"MP:{PlayerMP}/{PlayerMaxMP}";
        }
        
        EnemyInstance firstLiving = GetFirstLivingEnemy();
        if (firstLiving != null)
        {
            EnemyNameText.text = firstLiving.Data.DisplayName;
            EnemyHPText.text = $"HP:{firstLiving.CurrentHP}/{firstLiving.Data.MaxHP}";
            
            int livingCount = enemies.FindAll(e => !e.IsDead).Count;
            if (livingCount > 1)
            {
                EnemyNameText.text += $" 他{livingCount - 1}体";
            }
        }
        else
        {
            EnemyNameText.text = "---";
            EnemyHPText.text = "HP: 0";
        }

        // 個別エネミーの頭上UI更新
        foreach (var enemy in enemies)
        {
            if (enemy.UIInstance != null)
            {
                if (enemy.IsDead)
                {
                    enemy.UIInstance.SetActive(false);
                }
                else
                {
                    enemy.UIInstance.SetActive(true);
                    var texts = enemy.UIInstance.GetComponentsInChildren<TextMeshProUGUI>();
                    // プレハブの構造に合わせて調整（0番目を名前、1番目をHPと想定）
                    if (texts.Length >= 2)
                    {
                        texts[0].text = enemy.Data.DisplayName;
                        texts[1].text = $"HP: {enemy.CurrentHP}";
                    }
                }
            }
        }
    }

    private void Victory()
    {
        DialogText.text = "勝利";

        int totalExp = 0;
        foreach (var enemy in enemies)
        {
            totalExp += enemy.Data.ExpReward;
        }

        int levelUps = 0;
        if (LevelSystem != null)
        {
            levelUps = LevelSystem.AddExp(totalExp);
        }

        ApplyPlayerStatus();
        UpdateUI();

        if (levelUps > 0)
        {
            DialogText.text += $"\n{totalExp}EXPかくとく\nレベルが{PlayerStatus.Level}になった";
        }
        else
        {
            DialogText.text += $"\n{totalExp}EXPかくとく";
        }

        Invoke(nameof(ReturnToField), 2f);
    }

    private void GameOver()
    {
        DialogText.text = "全滅した・・・";
        Invoke(nameof(ReturnToField), 2f);
    }

    private void ReturnToField()
    {
        SceneManager.LoadScene("Field_01");
    }

    private System.Collections.IEnumerator TriggerPlayerActionEffects(string animTrigger)
    {
        if (playerModelInstance != null)
        {
            Animator playerAnim = playerModelInstance.GetComponentInChildren<Animator>();
            if (playerAnim != null)
            {
                playerAnim.SetTrigger(animTrigger);
            }
        }
        yield return null;
    }

    private System.Collections.IEnumerator TriggerPlayerHitEffects()
    {
        // プレイヤーモデルのアニメーション
        if (playerModelInstance != null)
        {
            Animator playerAnim = playerModelInstance.GetComponentInChildren<Animator>();
            if (playerAnim != null)
            {
                playerAnim.SetTrigger("Damage");
            }
        }

        // カメラシェイクとフラッシュを並列で実行
        var shake = StartCoroutine(ShakeCamera(HitShakeDuration, HitShakeIntensity));
        var flash = StartCoroutine(FlashScreen(HitFlashDuration, HitFlashColor));

        yield return shake;
        yield return flash;
    }

    private System.Collections.IEnumerator ShakeCamera(float duration, float intensity)
    {
        if (MainCamera == null) MainCamera = Camera.main;
        if (MainCamera == null) yield break;

        Vector3 originalPos = MainCamera.transform.localPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * intensity;
            float y = Random.Range(-1f, 1f) * intensity;

            MainCamera.transform.localPosition = originalPos + new Vector3(x, y, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }

        MainCamera.transform.localPosition = originalPos;
    }

    private System.Collections.IEnumerator FlashScreen(float duration, Color color)
    {
        if (HitFlashImage == null) yield break;

        HitFlashImage.gameObject.SetActive(true);
        HitFlashImage.color = color;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            Color c = color;
            c.a = Mathf.Lerp(color.a, 0, elapsed / duration);
            HitFlashImage.color = c;
            elapsed += Time.deltaTime;
            yield return null;
        }

        HitFlashImage.gameObject.SetActive(false);
    }

    private System.Collections.IEnumerator FlashColor(EnemyInstance enemy, Color color, float duration)
    {
        if (enemy.ModelInstance == null) yield break;

        var renderers = enemy.ModelInstance.GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (var r in renderers)
        {
            r.material.EnableKeyword("_EMISSION");
            r.material.SetColor("_EmissionColor", color);
        }

        yield return new WaitForSeconds(duration);

        foreach (var r in renderers)
        {
            r.material.SetColor("_EmissionColor", Color.black);
        }
    }
}
