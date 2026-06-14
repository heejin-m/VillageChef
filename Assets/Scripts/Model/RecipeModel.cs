using System.Collections.Generic;

public class RecipeModel : AbstractModel
{
    public Dictionary<int, RecipeInfo> _recipeInfoDict = new Dictionary<int, RecipeInfo>();

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
    public bool IsHave(int id)
    {
        if (_recipeInfoDict.TryGetValue(id, out RecipeInfo info))
        {
            return info.IsHave;
        }

        return false;
    }

    /// <summary>
    /// 레시피 추가
    /// </summary>
    public void AddRecipe(int id)
    {
        RecipeSaveInfo saveInfo = new RecipeSaveInfo
        {
            id = id
        };

        SaveManager.Save(saveInfo);
        Set(ModelCenter.StartInfoSetData.recipeSaveInfos);
    }
}