using System.Collections.Generic;
using UnityEngine;

public class InventryManager : MonoBehaviour
{
    //どこからでも呼べるようにstatic修飾子をつける
    public static InventryManager Instance;

    private List<ItemDate> items = new List<ItemDate>();

    private void Awake()
    {
        Instance = this;
    }

    public void Add(ItemDate item) {
        if (item == null)
        {

            return;
        
        }
        //アイテムデータをインベントリーに追加
        items.Add(item);
        Debug.Log($"[インベントリー]アイテム追加:{item.ItemName}");

    }

    public bool Has(ItemDate item)
    {
        return items.Contains(item);
    
    }

    public IReadOnlyList<ItemDate> GetAll()
    {
        return items;

    }


}
