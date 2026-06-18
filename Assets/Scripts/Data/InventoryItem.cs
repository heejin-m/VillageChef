using System;

[Serializable]
public partial class InventoryItem
{
    /// <summary>
    /// ID
    /// </summary>
    public int id;
    /// <summary>
    /// 이름
    /// </summary>
    public string name;
    /// <summary>
    /// 설명
    /// </summary>
    public string description;
    /// <summary>
    /// 타입
    /// </summary>
    public byte type;
    /// <summary>
    /// UI 분류 카테고리
    /// </summary>
    public byte category;
    /// <summary>
    /// 리소스 이름
    /// </summary>
    public string resourceName;
    /// <summary>
    /// 최대 소유 수량
    /// </summary>
    public ushort maxStack;
    /// <summary>
    /// 인벤토리에서 즉시 사용 가능 여부
    /// </summary>
    public bool isCanUse;
    /// <summary>
    /// 정렬 순서
    /// </summary>
    public byte sortOrder;
    /// <summary>
    /// 판매 가격
    /// </summary>
    public int sellPrice;
    /// <summary>
    /// 구매 가격
    /// </summary>
    public int buyPrice;
    /// <summary>
    /// 버리기 가능 여부
    /// </summary>
    public bool isDiscardable;
}
