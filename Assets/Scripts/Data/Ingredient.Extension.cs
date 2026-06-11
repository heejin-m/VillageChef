public partial class Ingredient
{
    public string Name => GetInventoryItem().name;
    public string Description => GetInventoryItem().description;
    public string ResourceName => GetInventoryItem().resourceName;

    /// <summary>
    /// 인벤토리 아이템 데이터 가져오기
    /// </summary>
    /// <returns></returns>
    public InventoryItem GetInventoryItem()
    {
        var inventoryItemData = DataManager.Instance.GetData<InventoryItemData>();
        return inventoryItemData.GetData(this.inventoryItemId);
    }
}