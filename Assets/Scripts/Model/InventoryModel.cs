using System.Collections.Generic;

public class InventoryModel : AbstractModel
{
    /// <summary>
    /// key : ID 딕셔너리
    /// </summary>
    private Dictionary<int, InventoryItemInfo> _itemInfoDictByID = new Dictionary<int, InventoryItemInfo>();
    /// <summary>
    /// key : Type 딕셔너리
    /// </summary>
    private Dictionary<eInventoryItemType, List<InventoryItemInfo>> _itemInfoDictByType = new Dictionary<eInventoryItemType, List<InventoryItemInfo>>();
    /// <summary>
    /// key : Category 딕셔너리 
    /// </summary>
    private Dictionary<eInventoryItemCategory, List<InventoryItemInfo>> _itemInfoDictByCategory = new Dictionary<eInventoryItemCategory, List<InventoryItemInfo>>();

    public void Set(List<InventoryItemSaveInfo> saveInfos)
    {
        _itemInfoDictByID.Clear();
        _itemInfoDictByType.Clear();
        _itemInfoDictByCategory.Clear();

        var inventoryItemData = DataManager.Instance.GetData<InventoryItemData>();
        foreach (var data in inventoryItemData.Datas)
        {
            // _ItemInfoDictByID 구성
            InventoryItemSaveInfo saveInfo = saveInfos?.Find(d => d.id == data.Key);
            InventoryItemInfo itemInfo = new InventoryItemInfo(data.Key, saveInfo);
            _itemInfoDictByID.Add(data.Key, itemInfo);

            // _ItemInfoDictByType 구성
            eInventoryItemType type = itemInfo.InventoryItem.ItemType;
            if (!_itemInfoDictByType.TryGetValue(type, out List<InventoryItemInfo> typeList))
            {
                typeList = new List<InventoryItemInfo>();
                _itemInfoDictByType.Add(type, typeList);
            }
            typeList.Add(itemInfo);

            // _ItemInfoDictByCategory 구성
            eInventoryItemCategory category = itemInfo.InventoryItem.Category;
            if (!_itemInfoDictByCategory.TryGetValue(category, out List<InventoryItemInfo> categoryList))
            {
                categoryList = new List<InventoryItemInfo>();
                _itemInfoDictByCategory.Add(category, categoryList);
            }
            categoryList.Add(itemInfo);
        }
    }

    public InventoryItemInfo GetItemListById(int id)
    {
        if (_itemInfoDictByID.TryGetValue(id, out var info))
        {
            return info;
        }

        return null;
    }

    public List<InventoryItemInfo> GetItemListByType(eInventoryItemType type)
    {
        if (_itemInfoDictByType.TryGetValue(type, out var list))
        {
            return list;
        }

        return null;
    }

    public List<InventoryItemInfo> GetItemListByCategory(eInventoryItemCategory category)
    {
        if (_itemInfoDictByCategory.TryGetValue(category, out var list))
        {
            return list;
        }

        return null;
    }
}
