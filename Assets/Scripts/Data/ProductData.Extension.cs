using System.Collections.Generic;

public partial class ProductData
{
    private Dictionary<int, Product> _dataByInventoryId = null;

    public void SetDictionaryData()
    {
        _dataByInventoryId ??= new();

        foreach (var data in Datas)
        {
            if (!_dataByInventoryId.ContainsKey(data.Value.inventoryItemId))
            {
                _dataByInventoryId.Add(data.Value.inventoryItemId, data.Value);
            }
        }
    }

    /// <summary>
    /// 인벤토리 ID로 상품 데이터 가져오기
    /// </summary>
    /// <param name="id">인덱스</param>
    /// <returns></returns>
    public Product GetDataByInventoryId(int id)
    {
        if (_dataByInventoryId != null && _dataByInventoryId.TryGetValue(id, out var data))
        {
            return data;
        }

        return null;
    }

    /// <summary>
    /// 데이터 가져오기
    /// </summary>
    /// <param name="index">인덱스</param>
    /// <returns></returns>
    public Product GetData(int index)
    {
        if (Datas != null && Datas.TryGetValue(index, out var data))
        {
            return data;
        }

        return null;
    }
}