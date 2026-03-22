using UnityEngine;

public class PlayerState: MonoBehaviour
{

    public static PlayerState Instance;
   
    [Header("��b�X�e�[�^�X")]
    public PlayerStatus PlayerStatus;

[Header ("現在の所持金")]
public int CurrentGold = 500;

///所持金を増やす
public void AddGold(int amount)
{

CurrentGold += amount;

}

public bool ConsumeGold(int amount)
{

if(CurrentGold >=amount)
{
CurrentGold -=amount;
return true;
}
return false;

}

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        else 
        {
            Destroy(gameObject);
        }
    }

    public float MaxHP
    {
        get
        {
            float bonus = 0;
            if (EquipmentManager.Instance != null)
            {
                bonus = EquipmentManager.Instance.GetBonusMaxHP();

            }
            return PlayerStatus.MaxHP + bonus;

        }
    }
    public float AttackMax
    {
        get
        {
            float bonus = 0;
            if (EquipmentManager.Instance != null)
            {
                bonus = EquipmentManager.Instance.GetBonusAttack();

            }
            return PlayerStatus.AttackMax + bonus;

        }
    }

    public float AttackMin
    {
        get
        {
            float bonus = 0;
            if (EquipmentManager.Instance != null)
            {
                bonus = EquipmentManager.Instance.GetBonusAttack();

            }
            return PlayerStatus.AttackMin + bonus;

        }
    }

    public float Defence
    {
        get
        {
            if (EquipmentManager.Instance != null)
            {

                return EquipmentManager.Instance.GetBonusDefence();

            }
            return 0;

        }
    }
}
