public partial class IngredientData
{
    /// <summary>
    /// 데이터 가져오기
    /// </summary>
    /// <param name="index">인덱스</param>
    /// <returns></returns>
	public Ingredient GetData(ushort index)
    {
        if (Datas != null && Datas.TryGetValue(index, out var data))
        {
            return data;
        }

        return null;
    }
}