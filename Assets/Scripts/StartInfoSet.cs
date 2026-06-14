using System.Collections.Generic;

[System.Serializable]
public class StartInfoSet
{
    /// <summary>
    /// 가지고 있는 레시피 데이터 리스트
    /// </summary>
    public List<RecipeSaveInfo> recipeSaveInfos = new();
    /// <summary>
    /// 인벤토리 데이터 리스트
    /// </summary>
    public List<InventoryItemSaveInfo> inventoryItemSaveInfo = new();
}
