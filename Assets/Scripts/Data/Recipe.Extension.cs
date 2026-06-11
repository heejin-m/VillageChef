using System.Collections.Generic;

public partial class Recipe
{
    public int GetIngredientCount()
    {
        int cnt = 0;

        OnPlus(ingredientId1);
        OnPlus(ingredientId2);
        OnPlus(ingredientId3);
        OnPlus(ingredientId4);
        OnPlus(ingredientId5);
        OnPlus(ingredientId6);
        OnPlus(ingredientId7);
        OnPlus(ingredientId8);
        OnPlus(ingredientId9);
        OnPlus(ingredientId10);

        return cnt;

        void OnPlus(byte ingredientId)
        {
            if (ingredientId != 0) ++cnt;
        }
    }

    public List<byte> GetIngredientIdList()
    {
        List<byte> ingredientData = new();

        OnPlus(ingredientId1);
        OnPlus(ingredientId2);
        OnPlus(ingredientId3);
        OnPlus(ingredientId4);
        OnPlus(ingredientId5);
        OnPlus(ingredientId6);
        OnPlus(ingredientId7);
        OnPlus(ingredientId8);
        OnPlus(ingredientId9);
        OnPlus(ingredientId10);

        return ingredientData;

        void OnPlus(byte ingredientId)
        {
            if (ingredientId != 0)
            {
                ingredientData.Add(ingredientId);
            }
        }
    }
}