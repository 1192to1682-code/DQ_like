using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public enum BattleMenuState
{
    Root,  // たたかう/さくせん/にげる
    Fight, // こうげき/じゅもん/とくぎ/ぼうぎょ
    Busy   // 演出中(入力不可)
}

public class BattleManager : MonoBehaviour
{
    public static int[] NextEnemyIDs = new int[] { 0, 1, 2 };

    [Header("EnemyData")]
    public EnemyDatabase EnemyDB;
    private List<EnemyInstance> enemies = new List<EnemyInstance>();

    [Header("Enemy Visual")]
    public Transform EnemyModelRoot;

    [Header("PlayerStatus と LevelSystem の参照")]
    public PlayerStatus PlayerStatus;
    public LevelSystem LevelSystem;

    [Header("PlayerData")]
    public float PlayerMaxHP = 30f;
    public float PlayerHP = 30;
    public float PlayerAttackMin = 5;
    public float PlayerAttackMax = 10;

    [Header("UI")]
    public TextMeshProUGUI PlayerHPText;
    public TextMeshProUGUI EnemyNameText;
    public TextMeshProUGUI EnemyHPText;
    public TextMeshProUGUI DialogText;

    [Header("DQ Like Menu")]
    public GameObject RootmenuPanel;
    public Transform RootMenuRoot;
    public GameObject FightMenuPanel;
    public Transform FightMenuRoot;

    public MenuButton MenuButtonPrefab;

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
        PlayerMaxHP = PlayerStatus.MaxHP;
        PlayerHP = Mathf.Min(PlayerHP, PlayerMaxHP);
        PlayerAttackMin = PlayerStatus.AttackMin;
        PlayerAttackMax = PlayerStatus.AttackMax;
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
            FightMenuPanel.SetActive(state == BattleMenuState.Fight);
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
            StartCoroutine(ExecuteHealSpell());
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

    private System.Collections.IEnumerator ExecuteAttack()
    {
        isPlayerTurn = false;
        SetMenuState(BattleMenuState.Busy);

        EnemyInstance target = GetFirstLivingEnemy();
        if (target == null) yield break;

        DialogText.text = "プレイヤーの攻撃!";
        yield return new WaitForSeconds(1f);

        var damage = Mathf.Ceil(Random.Range(PlayerAttackMin, PlayerAttackMax));
        target.CurrentHP -= damage;

        if (target.Animator != null)
        {
            target.Animator.SetTrigger("Damage");
            StartCoroutine(FlashColor(target, Color.red, 0.2f));
        }

        DialogText.text = $"{target.Data.DisplayName}に{damage}ダメージ!";
        UpdateUI();
        yield return new WaitForSeconds(1f);

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

    private System.Collections.IEnumerator ExecuteHealSpell()
    {
        isPlayerTurn = false;
        SetMenuState(BattleMenuState.Busy);
        DialogText.text = "キュア";
        yield return new WaitForSeconds(0.6f);

        float heal = Mathf.CeilToInt(PlayerMaxHP * 0.25f) + 2;
        PlayerHP = Mathf.Min(PlayerMaxHP, PlayerHP + heal);
        DialogText.text = $"{heal}かいふく";
        UpdateUI();
        yield return new WaitForSeconds(0.8f);
        StartCoroutine(EnemyTurn());
    }

    private System.Collections.IEnumerator ExecutePowerSkill()
    {
        isPlayerTurn = false;
        SetMenuState(BattleMenuState.Busy);

        EnemyInstance target = GetFirstLivingEnemy();
        if (target == null) yield break;

        DialogText.text = "つよく切り付けた";
        yield return new WaitForSeconds(0.6f);

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
        }
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
