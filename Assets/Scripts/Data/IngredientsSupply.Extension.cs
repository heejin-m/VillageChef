public enum eIngredientSupplyType
{
    /// <summary>
    /// 과수원
    /// </summary>
    Orchard = 1,
    /// <summary>
    /// 밭
    /// </summary>
    Field,
}

public partial class IngredientsSupply
{
    public eIngredientSupplyType SupplyType => (eIngredientSupplyType)this.supplyType;
}