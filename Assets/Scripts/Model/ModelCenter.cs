public class ModelCenter
{
    public static StartInfoSet StartInfoSetData = null;

    public static PlayerModel Player { get; private set; } = new();
    public static RecipeModel Recipe { get; private set; } = new();
    public static InventoryModel Inventory { get; private set; } = new();
    public static ProductModel Product { get; private set; } = new();
    public static IngredientSupplyModel Supply { get; private set; } = new();

    public static void ReleaseInstance()
    {
        Player = new();
        Recipe = new();
        Inventory = new();
        Product = new();
        Supply = new();
    }
}