using UnityEngine;
[CreateAssetMenu(menuName ="DQ-Like/Equipment/EquipmentData",
    fileName="Equip")]
public class EquipmentData : ScriptableObject
{
   
    public enum EquipmentType
    {
        Weapon,//•Ší
        Armor//–h‹ï

    }

    public string DisplayName;
    public EquipmentType Type;

    [Header("”\—Í•â³")]
    public float BonusHp;
    public float BonusAttack;
    public float BonusDefence;

}
