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

    /// <summary>
    /// 수량 구매 제한이 존재하는지
    /// </summary>
    public bool IsExistsBuyCntLimit => this.canBuyCnt != -1;
}