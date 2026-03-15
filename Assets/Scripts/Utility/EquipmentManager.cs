using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    public static EquipmentManager Instance;
    public EquipmentData EquipmentWeapon;
    public EquipmentData EquipmentArmor;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

        }
        else {

            Destroy(gameObject);
        
        }

    }

    public void Equip(EquipmentData equipment)
    {
        if(equipment == null)
        {
            return;

        }

        switch (equipment.Type)
        {
            case EquipmentData.EquipmentType.Weapon:
                EquipmentWeapon = equipment;
                break;
            case EquipmentData.EquipmentType.Armor:
                EquipmentArmor = equipment;
                break;

        }

    }

    public float GetBonusMaxHP()
    {
        float value = 0;
        if (EquipmentWeapon !=null)
        {
            value += EquipmentWeapon.BonusHp;

        }

        if(EquipmentArmor !=null)
        {

            value += EquipmentArmor.BonusHp;
        }

        return value;
    }
    //BonusAttackÇ∆BonuwDefenceÇï‘Ç∑ÉÅÉ\ÉbÉh

    public float GetBonusAttack()

    {
        float value = 0;
        if (EquipmentWeapon != null)
        {
            value += EquipmentWeapon.BonusAttack;

        }

        if (EquipmentArmor != null)
        {

            value += EquipmentArmor.BonusAttack;
        }

        return value;
    }


    public float GetBonusDefence()
    {
        float value = 0;
        if (EquipmentWeapon != null)
        {
            value += EquipmentWeapon.BonusDefence;

        }

        if (EquipmentArmor != null)
        {

            value += EquipmentArmor.BonusDefence;
        }

        return value;
    }

}



