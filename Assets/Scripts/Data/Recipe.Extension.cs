using System.Collections.Generic;

public partial class Recipe
{
    public string Name => GetInventoryItem().name;
    public string Description => GetInventoryItem().description;
    public string ResourceName => GetInventoryItem().resourceName;

    /// <summary>
    /// 인벤토리 아이템 데이터 가져오기
    /// </summary>
    /// <returns></returns>
    public InventoryItem GetInventoryItem()
    {
        var inventoryItemData = DataManager.Instance.GetData<InventoryItemData>();
        return inventoryItemData.GetData(this.inventoryItemId);
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