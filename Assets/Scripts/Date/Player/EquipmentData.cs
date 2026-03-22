using UnityEngine;
[CreateAssetMenu(menuName ="DQ-Like/Equipment/EquipmentData",
    fileName="Equip")]
public class EquipmentData : ScriptableObject
{
   
    public enum EquipmentType
    {
        Weapon,//����
        Armor//�h��

    }

    public string DisplayName;
    public EquipmentType Type;

    [Header("装備の性能")]
    public float BonusHp;
    public float BonusAttack;
    public float BonusDefence;

[Header("Shop用のデータ")]
public int Price;

}
