using System.Linq;

public partial class RecipeData
{
    public void SetDictionaryData()
    {
    }

    /// <summary>
    /// 마지막 데이터 가져오기
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
