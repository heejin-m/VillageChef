public class InventoryItemInfo
{
    /// <summary>
    /// ID
    /// </summary>
    public int ID { get; private set; }
    /// <summary>
    /// 저장 정보
    /// </summary>
    public InventoryItemSaveInfo SaveInfo { get; private set; }
    /// <summary>
    /// 데이터
    /// </summary>
    public InventoryItem InventoryItem { get; private set; }
    /// <summary>
    /// 보유 개수
    /// </summary>
    public int Cnt => SaveInfo != null ? SaveInfo.cnt : 0;
    /// <summary>
    /// 보유 여부
    /// </summary>
    public bool IsHave => Cnt > 0;

    #region ## Construct ##

    public InventoryItemInfo(int id, InventoryItemSaveInfo saveInfo) : base()
    {
        var data = DataManager.Instance.GetData<InventoryItemData>();

        this.ID = id;
        this.SaveInfo = saveInfo;
        this.InventoryItem = data.GetData(id);
    }

    #endregion
}