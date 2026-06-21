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

    /// <summary>
    /// ID로 InventoryItemInfo 가져오기
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public InventoryItemInfo GetItemById(int id)
    {
        if (_itemInfoDictByID.TryGetValue(id, out var info))
        {
            return info;
        }

        return null;
    }

    /// <summary>
    /// 타입으로 InventoryItemInfo 리스트 가져오기
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public List<InventoryItemInfo> GetItemListByType(eInventoryItemType type)
    {
        if (_itemInfoDictByType.TryGetValue(type, out var list))
        {
            return new List<InventoryItemInfo>(list);
        }

        return null;
    }

    /// <summary>
    /// 타입으로 소유하고 있는 InventoryItemInfo 리스트 가져오기
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public List<InventoryItemInfo> GetHaveItemListByType(eInventoryItemType type)
    {
        if (_itemInfoDictByType.TryGetValue(type, out var list))
        {
            if (list != null)
            {
                return list.FindAll(d => d.IsHave);
            }
        }

        return null;
    }

    /// <summary>
    /// 카테고리로 InventoryItemInfo 리스트 가져오기
    /// </summary>
    /// <param name="category"></param>
    /// <returns></returns>
    public List<InventoryItemInfo> GetItemListByCategory(eInventoryItemCategory category)
    {
        if (_itemInfoDictByCategory.TryGetValue(category, out var list))
        {
            if (list != null)
            {
                return new List<InventoryItemInfo>(list);
            }
        }

        return null;
    }

    /// <summary>
    /// 카테고리로 소유하고 있는 InventoryItemInfo 리스트 가져오기
    /// </summary>
    /// <param name="category"></param>
    /// <returns></returns>
    public List<InventoryItemInfo> GetHaveItemListByCategory(eInventoryItemCategory category)
    {
        if (_itemInfoDictByCategory.TryGetValue(category, out var list))
        {
            if (list != null)
            {
                return list.FindAll(d => d.IsHave);
            }
        }

        return null;
    }

    /// <summary>
    /// 아이템 추가
    /// </summary>
    public bool AddItem(int id, int amount)
    {
        if (amount <= 0) return false;

        if (!_itemInfoDictByID.TryGetValue(id, out var info))
        {
            return false;
        }

        InventoryItemSaveInfo saveInfo = info.SaveInfo;
        saveInfo ??= new InventoryItemSaveInfo();
        saveInfo.id = id;
        saveInfo.cnt += amount;

        SaveManager.Save(saveInfo);
        this.Set(ModelCenter.StartInfoSetData.inventoryItemSaveInfos);
        return true;
    }

    /// <summary>
    /// 아이템 삭제
    /// </summary>
    public void UseItem(int id, int amount)
    {
        if (amount <= 0) return;

        if (!_itemInfoDictByID.TryGetValue(id, out var info)) return;

        InventoryItemSaveInfo saveInfo = info.SaveInfo;
        if (saveInfo == null || saveInfo.cnt < amount) return;

        saveInfo.cnt -= amount;

        SaveManager.Save(saveInfo);
        this.Set(ModelCenter.StartInfoSetData.inventoryItemSaveInfos);
    }
}
