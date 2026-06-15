using UnityEngine;

public partial class Product
{
    public InventoryItem GetInventoryItem() => InventoryItem == null ? MakeInventoryItemData() : InventoryItem;
    public eInventoryItemType ItemType => GetInventoryItem().ItemType;
    public eInventoryItemCategory Category => GetInventoryItem().Category;


    private InventoryItem InventoryItem = null;
    private InventoryItem MakeInventoryItemData()
    {
        var inventoryItemData = DataManager.Instance.GetData<InventoryItemData>();
        this.InventoryItem = inventoryItemData.GetData(this.inventoryItemId);
        return InventoryItem;
    }
}