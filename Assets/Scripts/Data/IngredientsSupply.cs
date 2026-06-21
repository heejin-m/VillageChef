using System;

[Serializable]
public partial class IngredientsSupply
{
    /// <summary>
    /// id
    /// </summary>
    public int id;
    /// <summary>
    /// supplyType
    /// </summary>
    public int supplyType;
    /// <summary>
    /// inventoryItemId
    /// </summary>
    public int inventoryItemId;
    /// <summary>
    /// amount
    /// </summary>
    public int amount;
    /// <summary>
    /// coolTime
    /// </summary>
    public int coolTime;
    /// <summary>
    /// maxStack
    /// </summary>
    public int maxStack;
    /// <summary>
    /// unlockRecipeId
    /// </summary>
    public int unlockRecipeId;
    /// <summary>
    /// description
    /// </summary>
    public string description;
}
