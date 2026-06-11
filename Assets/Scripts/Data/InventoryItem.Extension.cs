public enum eInventoryItemType
{
    Ingredient = 0,
    Food,
    Tool,
    Quest,
    Seed,
    Material,
    Consumable,
}

public enum eInventoryItemCategory
{
    Fruit = 0,
    Vegetable,
    Meat,
    Seafood,
    Dairy,
    Sweet,
    Special,
}

public partial class InventoryItem
{
    public eInventoryItemType ItemType => (eInventoryItemType)this.type;
    public eInventoryItemCategory Category => (eInventoryItemCategory)this.category;
    public bool IsCanUse => isCanUse == 1 ? true : false;
    public bool IsDiscardable => isDiscardable == 1 ? true : false;
}