using System;

[Serializable]
public partial class Product
{
    /// <summary>
    /// ID
    /// </summary>
    public int id;
    /// <summary>
    /// 판매 물품의 인벤토리 데이터 ID
    /// </summary>
    public int inventoryItemId;
    /// <summary>
    /// 판매 가격
    /// </summary>
    public int sellPrice;
    /// <summary>
    /// 구매 가격
    /// </summary>
    public int buyPrice;
    /// <summary>
    /// 판매 수량
    /// </summary>
    public int amount;
    /// <summary>
    /// 구매 제한 수량
    /// </summary>
    public short canBuyCnt;
    /// <summary>
    /// 사용 가능한 데이터인지
    /// </summary>
    public bool isVaild;
}
