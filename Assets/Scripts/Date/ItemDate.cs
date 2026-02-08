using UnityEngine;

[CreateAssetMenu(menuName = "DQ-Like/Items/ItemDate",
    fileName = "item_")]
public class ItemDate : ScriptableObject
{
    public string ItemName;

    [TextArea(2,4)]
    public string Description;

    //任意ですが、UIで使うアイコン
    public Sprite Icon;

}
