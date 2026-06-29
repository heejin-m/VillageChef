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
    /// 해당 id의 레시피 정보 가져오기
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public RecipeInfo GetRecipe(int id)
    {
        if (_recipeInfoDict.TryGetValue(id, out var info))
        {
            return info;
        }

        return null;
    }

    /// <summary>
    /// 해당 인벤토리 id의 레시피 정보 가져오기
    /// </summary>
    /// <param name="inventoryId"></param>
    /// <returns></returns>
    public RecipeInfo GetRecipeByInventoryId(int inventoryId)
    {
        foreach (var item in _recipeInfoDict.Values)
        {
            if (inventoryId == item.Recipe.recipe_inventoryItemId)
            {
                return item;
            }
        }

        return null;
    }

    /// <summary>
    /// 소유하고 있는 모든 레시피 가져오기
    /// </summary>
    /// <returns></returns>
    public List<RecipeInfo> GetHaveRecipeList()
    {
        List<RecipeInfo> recipeInfos = null;
        foreach (var info in _recipeInfoDict.Values)
        {
            if (IsHave(info.Id))
            {
                recipeInfos ??= new();
                recipeInfos.Add(info);
            }
        }

        return recipeInfos;
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