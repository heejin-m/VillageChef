using TMPro;
using UnityEngine;

public class ShopBuyItem : MonoBehaviour
{
    #region Inspector

    public ItemUI itemUI;
    public TMP_Text title;
    public TMP_Text price;

    #endregion

    public void Set(ProductInfo info)
    {
        var inventoryInfo = ModelCenter.Inventory.GetItemById(info.InventoryItemID);
        itemUI.Set(inventoryInfo);
        itemUI.SetCnt(info.Amount);
        title.text = inventoryInfo.Name;
        price.text = string.Format("{0:n0}", info.Price);
    }
}