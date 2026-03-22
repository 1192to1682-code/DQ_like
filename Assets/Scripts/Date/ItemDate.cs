using UnityEngine;

[CreateAssetMenu(menuName = "DQ-Like/Items/ItemDate",
    fileName = "item_")]
public class ItemDate : ScriptableObject
{
    public enum ItemType
    { 
    
    HealHP,
    BuffAttack,
    Revive
    }
    public string ItemName;

    public ItemType Type;

    [TextArea(2,4)]
    public string Description;

    //�C�ӂł����AUI�Ŏg���A�C�R��
    public Sprite Icon;

    [Header("���ʂ̒l")]
    public float Power; //�񕜗ʂȂ�
    

    [Header("Shop用のデータ")]
    public int Price;
}
