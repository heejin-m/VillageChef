using System;

[Serializable]
public partial class Recipe
{
    /// <summary>
    /// ID
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
    /// 재료 01 ID
    /// </summary>
    public byte ingredientId1;
    /// <summary>
    /// 재료 02 ID
    /// </summary>
    public byte ingredientId2;
    /// <summary>
    /// 재료 03 ID
    /// </summary>
    public byte ingredientId3;
    /// <summary>
    /// 재료 04 ID
    /// </summary>
    public byte ingredientId4;
    /// <summary>
    /// 재료 05 ID
    /// </summary>
    public byte ingredientId5;
    /// <summary>
    /// 재료 06 ID
    /// </summary>
    public byte ingredientId6;
    /// <summary>
    /// 재료 07 ID
    /// </summary>
    public byte ingredientId7;
    /// <summary>
    /// 재료 08 ID
    /// </summary>
    public byte ingredientId8;
    /// <summary>
    /// 재료 09 ID
    /// </summary>
    public byte ingredientId9;
    /// <summary>
    /// 재료 10 ID
    /// </summary>
    public byte ingredientId10;
    /// <summary>
    /// 요리에 걸리는 시간
    /// </summary>
    public int cookTimeSec;
}
