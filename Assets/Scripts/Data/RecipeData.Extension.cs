using System.Linq;

public partial class RecipeData
{
    /// <summary>
    /// 데이터 가져오기
    /// </summary>
    /// <param name="index">인덱스</param>
    /// <returns></returns>
	public Recipe GetData(int index)
    {
        if (Datas != null && Datas.TryGetValue(index, out var data))
        {
            return data;
        }

        return null;
    }

    /// <summary>
    /// 데이터 가져오기
    /// </summary>
    /// <returns></returns>
	public Recipe GetLastData()
    {
        if (Datas != null)
        {
            return Datas.Last().Value;
        }

        return null;
    }
}