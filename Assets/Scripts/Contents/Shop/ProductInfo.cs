public class ProductInfo
{
    /// <summary>
    /// ID
    /// </summary>
    public int Id { get; private set; }
    /// <summary>
    /// 상품 데이터
    /// </summary>
    public Product ProductData { get; private set; }
    /// <summary>
    /// 인벤토리 아이템 데이터
    /// </summary>
    public InventoryItem InventoryItem { get; private set; }
    /// <summary>
    /// 인벤토리 아이템 ID
    /// </summary>
    public int InventoryItemID => InventoryItem.id;
    /// <summary>
    /// 판매 수량
    /// </summary>
    public int Amount => this.ProductData.amount;
    /// <summary>
    /// 판매 가격
    /// </summary>
    public int Price => this.ProductData.price;
    /// <summary>
    /// 사용 가능한 데이터인지
    /// </summary>
    public bool IsVaild => ProductData.isVaild;

    #region ## Constructor ##

    public ProductInfo(int id, ProductSaveInfo saveInfo) : base()
    {
        var productData = DataManager.Instance.GetData<ProductData>();
        this.ProductData = productData.GetData(id);
        this.InventoryItem = this.ProductData.GetInventoryItem();
    }

    #endregion
}