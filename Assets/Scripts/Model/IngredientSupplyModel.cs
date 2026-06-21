using System;
using System.Collections.Generic;

public class IngredientSupplyModel : AbstractModel
{
    private readonly Dictionary<int, IngredientSupplyInfo> _infosById = new();
    private readonly Dictionary<eIngredientSupplyType, List<IngredientSupplyInfo>> _infosByType = new();

    public void Set(List<IngredientSupplySaveInfo> saveInfos)
    {
        _infosById.Clear();
        _infosByType.Clear();

        var ingredientsSupplyData = DataManager.Instance.GetData<IngredientsSupplyData>();

        foreach (var data in ingredientsSupplyData.Datas)
        {
            // _infosById
            IngredientSupplySaveInfo saveInfo = saveInfos?.Find(d => d.id == data.Key);
            IngredientSupplyInfo supplyInfo = new IngredientSupplyInfo(data.Key, saveInfo);
            _infosById.Add(data.Key, supplyInfo);

            // _ItemInfoDictByType 구성
            eIngredientSupplyType type = supplyInfo.IngredientsSupply.SupplyType;
            if (!_infosByType.TryGetValue(type, out List<IngredientSupplyInfo> typeList))
            {
                typeList = new List<IngredientSupplyInfo>();
                _infosByType.Add(type, typeList);
            }
            typeList.Add(supplyInfo);
        }

        // 최초 수급 시간 저장
        foreach (var info in _infosById.Values)
        {
            SaveManager.Save(info.SaveInfo);
        }
    }

    /// <summary>
    /// eIngredientSupplyType으로 수급정보 가져오기
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public List<IngredientSupplyInfo> GetInfosByType(eIngredientSupplyType type)
    {
        if (_infosByType.TryGetValue(type, out var list))
        {
            return new List<IngredientSupplyInfo>(list);
        }

        return null;
    }

    /// <summary>
    /// Id로 수급정보 가져오기
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public IngredientSupplyInfo GetInfoById(int id)
    {
        if (_infosById.TryGetValue(id, out var info))
        {
            return info;
        }

        return null;
    }

    /// <summary>
    /// 마지막 수급 시간 변경
    /// </summary>
    public void SetLastSupplyTime(int id)
    {
        var info = GetInfoById(id);
        info.SetLastSupplyTime(DateTime.Now);

        SaveManager.Save(info.SaveInfo);
    }
}