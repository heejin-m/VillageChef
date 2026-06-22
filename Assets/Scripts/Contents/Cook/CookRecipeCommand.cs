public class CookRecipeCommand : ICookingCommand
{
    private readonly int _recipeId;

    public CookRecipeCommand(int recipeId)
    {
        _recipeId = recipeId;
    }

    public bool CanExecute()
    {
        Recipe recipe = DataManager.Instance.GetData<RecipeData>().GetData(_recipeId);
        if (recipe == null)
        {
            return false;
        }

        var ingredients = recipe.GetIngredientIdList();
        foreach (var ingredient in ingredients)
        {
            InventoryItemInfo item = ModelCenter.Inventory.GetItemById(ingredient);
            if (item == null || item.Cnt < 1)
            {
                return false;
            }
        }

        return true;
    }

    public CookingResult Execute()
    {
        if (!CanExecute())
        {
            return CookingResult.Fail("재료가 부족합니다.");
        }

        Recipe recipe = DataManager.Instance.GetData<RecipeData>().GetData(_recipeId);

        var ingredients = recipe.GetIngredientIdList();
        foreach (var ingredient in ingredients)
        {
            ModelCenter.Inventory.UseItem(ingredient, 1);
        }

        ModelCenter.Inventory.AddItem(recipe.dish_inventoryItemId, 1);

        return CookingResult.Success(recipe.dish_inventoryItemId, 1);
    }
}