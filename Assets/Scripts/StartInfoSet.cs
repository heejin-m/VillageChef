using System.Collections.Generic;

[System.Serializable]
public class StartInfoSet
{
    /// <summary>
    /// 플레이어 정보 저장 데이터
    /// </summary>
    public PlayerSaveInfo playerSaveInfo = new();
    /// <summary>
    /// 가지고 있는 레시피 저장 데이터 리스트
    /// </summary>
    public List<RecipeSaveInfo> recipeSaveInfos = new();
    /// <summary>
    /// 인벤토리 저장 데이터 리스트
    /// </summary>
    public List<InventoryItemSaveInfo> inventoryItemSaveInfo = new();
    /// <summary>
    /// 상품 저장 데이터 리스트
    /// </summary>
    public List<ProductSaveInfo> productSaveInfo = new();
}
