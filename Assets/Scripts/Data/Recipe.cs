using System;

[Serializable]
public partial class Recipe
{
    /// <summary>
    /// Id
    /// </summary>
    public int id;
    /// <summary>
    /// 완성품 인벤토리 데이터 ID
    /// </summary>
    public int dish_inventoryItemId;
    /// <summary>
    /// 레시피 인벤토리 데이터 ID
    /// </summary>
    public int recipe_inventoryItemId;
    /// <summary>
    /// 재료 01 ~ 10 ID
    /// </summary>
    public byte ingredientId1;
    public byte ingredientId2;
    public byte ingredientId3;
    public byte ingredientId4;
    public byte ingredientId5;
    public byte ingredientId6;
    public byte ingredientId7;
    public byte ingredientId8;
    public byte ingredientId9;
    public byte ingredientId10;
}
