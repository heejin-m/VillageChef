using System;

[Serializable]
public partial class Product
{
    /// <summary>
    /// Id
    /// </summary>
    public int id;
    /// <summary>
    /// 판매물품의 인벤토리 데이터 ID
    /// </summary>
    public int inventoryItemId;
    /// <summary>
    /// 판매 가격
    /// </summary>
    public int price;
    /// <summary>
    /// 판매 수량
    /// </summary>
    public int amount;
    /// <summary>
    /// 구매 제한 수량
    /// </summary>
    public bool canBuyCnt;
    /// <summary>
    /// 사용가능한 데이터인지
    /// </summary>
    public bool isVaild;
}