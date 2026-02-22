using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public enum BattleMenuState
{

    Root, //たたかう/さくせん/にげる
    Fight,//こうげき/じゅもん//とくぎ/防御
    Busy//演出中(入力不可)

}

public class BattleManager : MonoBehaviour
{
    public static int NextEnemyID = 0;

    [Header("EnemyData")]
    public EnemyDatabase EnemyDB;
    private EnemyData currentEnemy;

    [Header("Enemy Visual")]
    public Transform EnemyModelRoot;
    private GameObject enemyModelInstance;


    [Header("PlayerData")]
    public float PlayerMaxHP = 30f;
    public float PlayerHP = 30;
    public float PlayerAttackMin = 5;
    public float PlayerAttackMax = 10;

    [Header("Enemy HP")]
    public float EnemyHP;

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

        SetupEnemyFromDB();
        UpdateUI();

        BuildRootMenu();

        SpawnEnemyModel();

        DialogText.text = $"{currentEnemy.DisplayName}が現れた!";

    }

    private void SetupEnemyFromDB()
    {

        if (EnemyDB == null)
        {

            Debug.LogError("EnemyDBが設定されていません");
            return;

        }

        currentEnemy = EnemyDB.GetByID(NextEnemyID);

        if (currentEnemy == null)
        {
            Debug.LogError("NextEnemyIDがEnemyDBに見つかりません");
            return;

        }

        EnemyHP = currentEnemy.MaxHP;
    }

    private void SetMenuState(BattleMenuState state)
    {
        menuState = state;

        if (RootmenuPanel != null)
        {
            RootmenuPanel.SetActive(
                state == BattleMenuState.Root);

        }

        if (FightMenuPanel != null)

        {
            FightMenuPanel.SetActive(
                 state == BattleMenuState.Fight);

        }

        if (state == BattleMenuState.Busy)
        {
            if (RootmenuPanel != null)
            {
                RootmenuPanel.SetActive(false);

            }

            if (FightMenuPanel != null)

            {
                FightMenuPanel.SetActive(false);

            }
        }
    }


    private void BuildRootMenu()
    {

        ClearChildren(RootMenuRoot);
        CreateButton(RootMenuRoot, "たたかう", () => {
        
        if(!isPlayerTurn)

            {
                return;
            }

            //たたかうメニューを設定します
            //Todo:ここにあとで設定用のメソッドを追記する
            BuildFightMenu();
            SetMenuState(BattleMenuState.Fight);
            DialogText.text = "どうする？";
        });

        CreateButton(RootMenuRoot, "さくせん", () =>
        {
            if(!isPlayerTurn)
            {
                return;

            }
            DialogText.text = "さくせんはまだつかえない!";

        });

        CreateButton(RootMenuRoot, "にげる", () =>
        {

            if (!isPlayerTurn)
            {

                return;
            }

            StartCoroutine (TryEscape());
            //Todo：逃げるを使う

        });

    }

    private void BuildFightMenu()
    {

        ClearChildren(FightMenuRoot);
        CreateButton(FightMenuRoot, "こうげき", () =>
        {

            if (!isPlayerTurn)
            {
                return;

            }

            StartCoroutine(ExecuteAttack());

        });

    CreateButton(FightMenuRoot, "じゅもん", () =>
        {

            if (!isPlayerTurn)
            {
                return;

            }

            StartCoroutine(ExecuteHealSpell());

        });//Todo

        CreateButton(FightMenuRoot, "とくぎ", () =>
        {

            if (!isPlayerTurn)
            {
                return;

            }

            StartCoroutine(ExecutePowerSkill());

        });//Todo

        CreateButton(FightMenuRoot, "ぼうぎょ", () =>
        {

            if (!isPlayerTurn)
            {
                return;

            }

            StartCoroutine(ExecuteGurad());

        });//Todo

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


        DialogText.text = "プレイヤーの攻撃!";
        yield return new WaitForSeconds(1f);

        var damage =
            Mathf.Ceil(Random.Range(PlayerAttackMin, PlayerAttackMax));

        EnemyHP -= damage;

        DialogText.text = $"{damage}ダメージ!";

        UpdateUI();

        yield return new WaitForSeconds(1f);

        if (EnemyHP <= 0f)//体力を削り切れたら
        {

            Victory();
        }

        else//そうじゃなかったら
        {
            StartCoroutine(EnemyTurn());

        }

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


    private System.Collections.IEnumerator TryEscape()
    {
        //Random.valueは0～1の間の値をランダムに返してくれます
        bool success = Random.value < 0.5f;
    if(success)
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
        isPlayerTurn =false;
        SetMenuState(BattleMenuState.Busy);
        DialogText.text = "キュア";
        yield return new WaitForSeconds(0.6f);

        float heal = Mathf.CeilToInt(PlayerMaxHP * 0.25f) + 2;
        //
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
        DialogText.text = "つよく切り付けた";
        yield return new WaitForSeconds(0.6f);

        var damage =
           Mathf.Ceil(Random.Range(PlayerAttackMin, PlayerAttackMax)*1.6f +2);

        EnemyHP -= damage;

        DialogText.text = $"{damage}ダメージ!";

        UpdateUI();

        yield return new WaitForSeconds(0.8f);

        if (EnemyHP <= 0f)//体力を削り切れたら
        {

            Victory();
        }

        else//そうじゃなかったら
        {
            StartCoroutine(EnemyTurn());

        }




    }

    void CreateButton(Transform root, string label,
        System.Action onClick)
    {
        if (MenuButtonPrefab == null || root == null)
        {
            return;

        }

        var btn = Instantiate(MenuButtonPrefab, root);
        btn.Setup(label, onClick);

    }

    void ClearChildren(Transform root)
    {
        if(root == null)
        {
            return;

        }

        for(int i=root.childCount -1;i>=0;i--)
        {
            Destroy(root.GetChild(i).gameObject);

        }

    }




    /// <summary>
    /// 敵のVisualを生成します
    /// </summary>
    private void SpawnEnemyModel()
    {
        if (EnemyModelRoot == null)
        {
            return;

        }

        if (currentEnemy == null)

        {
            return;

        }

        if (currentEnemy.ModelPrefab == null)

        {
            return;

        }
        if (enemyModelInstance != null)
        {

            Destroy(enemyModelInstance);

        }

        enemyModelInstance = Instantiate(currentEnemy.ModelPrefab, EnemyModelRoot);

        enemyModelInstance.transform.localPosition =
            currentEnemy.ModelPosition;
        enemyModelInstance.transform.localEulerAngles =
            currentEnemy.ModelRotation;
        enemyModelInstance.transform.localScale =
            currentEnemy.ModelScale;


    }

    /// <summary>
    /// AttackButtonの設定
    /// </summary>
    public void OnAttackButton()
    {
        if (!isPlayerTurn)
        {
            return;

        }



        StartCoroutine(PlayerAttack());

    }

    private System.Collections.IEnumerator PlayerAttack()
    {
        isPlayerTurn = false;

        DialogText.text = "プレイヤーの攻撃!";

        yield return new WaitForSeconds(1f);

        var damage =
            Mathf.Ceil(Random.Range(PlayerAttackMin, PlayerAttackMax));

        EnemyHP -= damage;

        DialogText.text = $"{damage}ダメージ!";

        UpdateUI();

        yield return new WaitForSeconds(1f);

        if (EnemyHP <= 0f)//体力を削り切れたら
        {

            Victory();
        }

        else//そうじゃなかったら
        {
            StartCoroutine(EnemyTurn());

        }

    }

    private System.Collections.IEnumerator EnemyTurn()
    {
        DialogText.text = $"{currentEnemy.DisplayName}の攻撃!";
        yield return new WaitForSeconds(1f);

        var damage =
            Mathf.Ceil(
                Random.Range(currentEnemy.AttackMin,
                currentEnemy.AttackMax));

        if(isGuarding)
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
            GameOver(); //敗北

        }



        else
        {
            isPlayerTurn = true;
            SetMenuState(BattleMenuState.Root);
            DialogText.text = "どうする";

        }
    }

    private void UpdateUI()
    {
        PlayerHPText.text = $"HP:{PlayerHP}/{PlayerMaxHP}";
        if (currentEnemy != null)
        {
            EnemyNameText.text = currentEnemy.DisplayName;
            EnemyHPText.text = $"HP:{EnemyHP}/{currentEnemy.MaxHP}";

        }

        else
        {
            EnemyNameText.text = "Enemy";
            EnemyHPText.text = $"HP:{EnemyHP}";

        }
    }

    private void Victory()
    {
        DialogText.text = "勝利";
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

}
