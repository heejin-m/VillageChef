using System.Collections.Generic;

public class InventoryModel : AbstractModel
{
    /// <summary>
    /// key : ID 딕셔너리
    /// </summary>
    public Dictionary<int, InventoryItemInfo> _ItemInfoDictByID = new Dictionary<int, InventoryItemInfo>();
    /// <summary>
    /// key : Type 딕셔너리
    /// </summary>
    public Dictionary<eInventoryItemType, List<InventoryItemInfo>> _ItemInfoDictByType = new Dictionary<eInventoryItemType, List<InventoryItemInfo>>();
    /// <summary>
    /// key : Category 딕셔너리 
    /// </summary>
    public Dictionary<eInventoryItemCategory, List<InventoryItemInfo>> _ItemInfoDictByCategory = new Dictionary<eInventoryItemCategory, List<InventoryItemInfo>>();

    public void Set(List<InventoryItemSaveInfo> saveInfos)
    {
        _ItemInfoDictByID.Clear();
        _ItemInfoDictByType.Clear();
        _ItemInfoDictByCategory.Clear();

        var inventoryItemData = DataManager.Instance.GetData<InventoryItemData>();
        foreach (var data in inventoryItemData.Datas)
        {
            // _ItemInfoDictByID 구성
            InventoryItemSaveInfo saveInfo = saveInfos?.Find(d => d.id == data.Key);
            InventoryItemInfo itemInfo = new InventoryItemInfo(data.Key, saveInfo);
            _ItemInfoDictByID.Add(data.Key, itemInfo);

            // _ItemInfoDictByType 구성
            eInventoryItemType type = itemInfo.InventoryItem.ItemType;
            if (!_ItemInfoDictByType.TryGetValue(type, out List<InventoryItemInfo> typeList))
            {
                typeList = new List<InventoryItemInfo>();
                _ItemInfoDictByType.Add(type, typeList);
            }
            typeList.Add(itemInfo);

            // _ItemInfoDictByCategory 구성
            eInventoryItemCategory category = itemInfo.InventoryItem.Category;
            if (!_ItemInfoDictByCategory.TryGetValue(category, out List<InventoryItemInfo> categoryList))
            {
                categoryList = new List<InventoryItemInfo>();
                _ItemInfoDictByCategory.Add(category, categoryList);
            }
            categoryList.Add(itemInfo);
        }
    }
}
