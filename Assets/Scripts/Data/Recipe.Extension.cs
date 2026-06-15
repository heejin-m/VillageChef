using System.Collections.Generic;

public partial class Recipe
{
    public string DishName => GetDishInventoryItem().name;
    public string DishDescription => GetDishInventoryItem().description;
    public string DishResourceName => GetDishInventoryItem().resourceName;

    public string RecipeName => GetRecipeInventoryItem().name;
    public string RecipeDescription => GetRecipeInventoryItem().description;
    public string RecipeResourceName => GetRecipeInventoryItem().resourceName;

    /// <summary>
    /// 완성품 인벤토리 아이템 데이터 가져오기
    /// </summary>
    /// <returns></returns>
    public InventoryItem GetDishInventoryItem()
    {
        var inventoryItemData = DataManager.Instance.GetData<InventoryItemData>();
        return inventoryItemData.GetData(this.dish_inventoryItemId);
    }

    /// <summary>
    /// 레시피 인벤토리 아이템 데이터 가져오기
    /// </summary>
    /// <returns></returns>
    public InventoryItem GetRecipeInventoryItem()
    {
        var inventoryItemData = DataManager.Instance.GetData<InventoryItemData>();
        return inventoryItemData.GetData(this.recipe_inventoryItemId);
    }

    public List<byte> GetIngredientIdList()
    {
        var ingredientData = new List<byte>(10);

        AddIngredientId(ingredientId1);
        AddIngredientId(ingredientId2);
        AddIngredientId(ingredientId3);
        AddIngredientId(ingredientId4);
        AddIngredientId(ingredientId5);
        AddIngredientId(ingredientId6);
        AddIngredientId(ingredientId7);
        AddIngredientId(ingredientId8);
        AddIngredientId(ingredientId9);
        AddIngredientId(ingredientId10);

        return ingredientData;

        #region ## 로컬 함수 ##

        void AddIngredientId(byte ingredientId)
        {
            if (ingredientId != 0)
            {
                ingredientData.Add(ingredientId);
            }
        }

        #endregion
    }
}
