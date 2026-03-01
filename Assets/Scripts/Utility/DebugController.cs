using UnityEngine;
using TMPro;

public class DebugController : MonoBehaviour
{

    public PlayerStatus PlayerStatus;
    public ExpTable ExpTable;

    [Header("UI")]
    public TextMeshProUGUI InfoText;

    [Header("ÉIÉvÉVÉáÉì")]
    public BattleManager BattleManager;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        Refresh();


    }

    public void OnLvPlus()
    {
        SetLevel(PlayerStatus.Level + 1);

    }

    public void OnLvMinus()
    {
        SetLevel(PlayerStatus.Level - 1);

    }


    public void OnReset()
    {
        if(PlayerStatus ==null)
        {
            return;

        }
        PlayerStatus.ResetToDefault();
        Refresh();
        ApplyToBattleIfPossible();
    }

    private void SetLevel(int newLevel)
    {
        if (PlayerStatus == null ||
            ExpTable == null)
        {
            return;
        
        }

        if(newLevel <1)
        {

            newLevel = 1;

        }

        if(newLevel >99)
        {
            newLevel = 99;

        }

        float baseHp = 30;
        float baseMin = 3;
        float baseMax = 6;

        PlayerStatus.Level = newLevel;
        PlayerStatus.Exp = 0;

        PlayerStatus.MaxHP = baseHp + (newLevel - 1) * 5;
        PlayerStatus.AttackMin = baseMin + (newLevel - 1) * 1;
        PlayerStatus.AttackMax = baseMax + (newLevel - 1) * 1;

        Refresh();
        //Todo:êÌì¨íÜÇ…îΩâfÇµÇΩÇ¢èÍçá

        ApplyToBattleIfPossible();
    }

    private void ApplyToBattleIfPossible()
    {
        if (BattleManager ==null) 
         {

            return;
        }
        BattleManager.ApplyPlayerStatus();
        BattleManager.UpdateUI();


    }

    private void Refresh()
    {
        if (InfoText == null ||
        PlayerStatus == null || ExpTable == null)

        {
            return;

        }

        int needExp = ExpTable.GetNeedExpToNext(PlayerStatus.Level);
        InfoText.text =
            $"[Debug]\n" +
            $"Level:{PlayerStatus.Level}\n" +
            $"Exp:{PlayerStatus.Exp}/{needExp}\n" +
            $"MaxHP:{PlayerStatus.MaxHP}\n"+
            $"ATK:{PlayerStatus.AttackMin}-{PlayerStatus.AttackMax}";
    }

    
}
