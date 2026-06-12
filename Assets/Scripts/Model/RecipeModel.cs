using System.Collections.Generic;

public class RecipeModel : AbstractModel
{
    public Dictionary<ushort, RecipeInfo> _recipeInfoDict = new Dictionary<ushort, RecipeInfo>();

    public void Set(List<RecipeSaveInfo> saveInfos)
    {
        _recipeInfoDict.Clear();
        var recipeData = DataManager.Instance.GetData<RecipeData>();
        foreach (var data in recipeData.Datas)
        {
            RecipeSaveInfo saveInfo = saveInfos?.Find(d => d.id == data.Key);
            _recipeInfoDict.Add(data.Key, new RecipeInfo(data.Key, saveInfo));
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
            return info.IsHave;
        }

        return false;
    }
}