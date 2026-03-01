using UnityEngine;

public class LevelSystem : MonoBehaviour
{
    [Header("参照")]
    public PlayerStatus Status;
    public ExpTable ExpTable;

    [Header("レベルが上がったときの加算値")]
    public float HpUpPerLevel = 5;
    public float AttackMinUpPerLevel = 1;
    public float AttackMaxUpPerLevel = 1;

    public int AddExp(int amount)
    { 
    
    if(Status ==null ||ExpTable ==null)
        {

            return 0;

        }

    if(amount <=0)
        {
            return 0;

        }
    //データに対して経験値を加算
        Status.Exp += amount;
        int levelUpCount = 0;

        //複数回レベルアップに対応
        while(true)
        {
            int needExp = ExpTable.GetNeedExpToNext(Status.Level);
            
            //プレイヤーの経験値がレベルアップに
            //必要な経験値に足りていない場合は
            if(Status.Exp < needExp)
                {
                break;
            }
            Status.Exp -= needExp;
            Status.Level += 1;
            levelUpCount++;
            //Todo:レベルアップで能力を伸ばす
            ApplyGrowthOnece();
        }
        return levelUpCount;
    }

    private void ApplyGrowthOnece()
    {
        Status.MaxHP += HpUpPerLevel;
        Status.AttackMin += AttackMinUpPerLevel;
        Status.AttackMax += AttackMaxUpPerLevel;

    }

}
