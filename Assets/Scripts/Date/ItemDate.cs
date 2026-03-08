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

    //任意ですが、UIで使うアイコン
    public Sprite Icon;

    [Header("効果の値")]
    public float Power; //回復量など
}
