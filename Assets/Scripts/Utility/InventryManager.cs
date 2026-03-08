using System.Collections.Generic;
using UnityEngine;

public class InventryManager : MonoBehaviour
{
    //どこからでも呼べるようにstatic修飾子をつける
    public static InventryManager Instance;

    private List<InventryEntry> items = new List<InventryEntry>();

    private void Awake()
    {
        //シーンを跨いで使える設定
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
    public void Add(ItemDate item, int amount)
    {
        if (item == null || amount <= 0)
        {

            return;

        }

        var entry = items.Find(
            x => x.Item == item

            );

        if (entry != null)
        {
            entry.Count += amount;

        }

        else
        {

            items.Add(new InventryEntry

            {
                Item = item,
                Count = amount
            });

            //アイテムデータをインベントリーに追加

            Debug.Log($"[インベントリー]アイテム追加:{item.ItemName}");

        }

    }
    public bool Has(ItemDate item)
    {
        var entry = items.Find(
            x => x.Item == item);
        return entry != null;

    }

    /// <summary>
    /// アイテムの名称でItemDataを取得する
    /// </summary>
    /// <param name="itemName"></param>
    /// <returns></returns>
    public ItemDate GetItemDate(string itemName)
    {
        var entry = items.Find(
   x => x.Item. ItemName== itemName) ;
        return entry.Item;

    }


    public IReadOnlyList<InventryEntry > GetAll()
    {
        return items;

    }

    /// <summary>
    ///Itemを使用する 
    /// </summary>
    /// <returns></returns>
    public bool UseItem(ItemDate item)
    {
        var entry = items.Find(
    x => x.Item == item);
    
        if(entry ==null || entry.Count <= 0)
        {
            return false;

        }

        entry.Count--;
        return true;
    }

    public bool UseItem(string itemName)
    {
    var entry = items.Find(
    x => x.Item.ItemName == itemName);

        if (entry == null || entry.Count <= 0)
        {
            return false;

        }

        entry.Count--;
        return true;

    }


    public int GetCount(ItemDate item)
    {
        var entry = items.Find(
x => x.Item == item);
        if(entry ==null)
        {

            return 0;
        }

        return entry.Count;

    }

}
