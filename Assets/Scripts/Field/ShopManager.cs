using UnityEngine;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour
{

[Header("このお店で売っているアイテム")]
public List<ItemDate>ShopItems; 
public List<EquipmentData>ShopEquipments;

public void BuyItem(ItemDate itemToBuy)
{

if(itemToBuy ==null)
{
return;
}

if(PlayerState.Instance.ConsumeGold(itemToBuy.Price))
{

    InventryManager.Instance.Add(itemToBuy,1);
    DialogUI.Instance.ShowSimpleMessage($"{itemToBuy.ItemName}を1こ買った");

}

else
{

DialogUI.Instance.ShowSimpleMessage("お金が足りません");


}

this.gameObject.SetActive(false);
}


public void BuyEquipments(EquipmentData EquipmentToBuy)
{

if(EquipmentToBuy ==null)
{
return;
}

if(PlayerState.Instance.ConsumeGold(EquipmentToBuy.Price))
{

    EquipmentManager.Instance.AddEquipment(EquipmentToBuy);
    EquipmentManager.Instance.Equip(EquipmentToBuy);
    DialogUI.Instance.ShowSimpleMessage($"{EquipmentToBuy.DisplayName}を買い、装備した");

}

else
{

DialogUI.Instance.ShowSimpleMessage("お金が足りません");


}

this.gameObject.SetActive(false);
}



}