public partial class InventoryItemData
{
    /// <summary>
    /// 데이터 가져오기
    /// </summary>
    /// <param name="index">인덱스</param>
    /// <returns></returns>
	public InventoryItem GetData(int index)
    {
        if (Datas != null && Datas.TryGetValue(index, out var data))
        {
            return data;
        }

        return null;
    }
}