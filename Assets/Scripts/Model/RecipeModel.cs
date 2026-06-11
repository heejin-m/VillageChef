using System.Collections.Generic;

public class RecipeModel : AbstractModel
{
    public Dictionary<ushort, RecipeInfo> _recipeInfoDict = new Dictionary<ushort, RecipeInfo>();

    public void Set(List<RecipeInfo> recipeInfos)
    {
        _recipeInfoDict.Clear();
        var recipeData = DataManager.Instance.GetData<RecipeData>();
        foreach (var data in recipeData.Datas)
        {
            if (recipeInfos != null)
            {
                var info = recipeInfos.Find(d => d.id == data.Key);
                if (info == null)
                {
                    _recipeInfoDict.Add(data.Key, null);
                }
                else
                {
                    _recipeInfoDict.Add(info.id, info);
                }
            }
            else
            {
                _recipeInfoDict.Add(data.Key, null);
            }
        }
    }

    /// <summary>
    /// 소유하고 있는 레시피인지
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public bool IsHave(ushort id)
    {
        if (_recipeInfoDict.TryGetValue(id, out RecipeInfo info))
        {
            return info != null;
        }

        return false;
    }
}