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
    /// 저장 데이터
    /// </summary>
    public ProductSaveInfo SaveInfo { get; private set; }
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
    public int sellPrice => this.ProductData.sellPrice;
    /// <summary>
    /// 구매 가격
    /// </summary>
    public int BuyPrice => this.ProductData.buyPrice;
    /// <summary>
    /// 사용 가능한 데이터인지
    /// </summary>
    public bool IsVaild => ProductData.isVaild;
    /// <summary>
    /// 구매 가능한 상태인지 판단
    /// 조건 1. 구매 제한 수량을 넘기지 않은 경우
    /// </summary>
    public bool IsCanBuy => ProductData.IsExistsBuyCntLimit ? SaveInfo != null ? SaveInfo.buyCnt < ProductData.canBuyCnt : true : true;

    #region ## Constructor ##

    public ProductInfo(int id, ProductSaveInfo saveInfo) : base()
    {
        var productData = DataManager.Instance.GetData<ProductData>();
        this.Id = id;
        this.ProductData = productData.GetData(id);
        this.InventoryItem = this.ProductData.GetInventoryItem();
        this.SaveInfo = saveInfo;
    }

    #endregion
}