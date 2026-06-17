public class PlayerInfo
{
    /// <summary>
    /// 가진 골드 재화 수량
    /// </summary>
    public long Gold { get; private set; }
    /// <summary>
    /// 골드 획득
    /// </summary>
    /// <param name="amount"></param>
    public void AddGold(long amount) => this.Gold += amount;
    /// <summary>
    /// 골드 사용
    /// </summary>
    /// <param name="amount"></param>
    public void UseGold(long amount) => this.Gold -= amount;

    #region ## Constructor ##

    public PlayerInfo(PlayerSaveInfo saveInfo) : base()
    {
        this.Gold = saveInfo.gold;
    }

    #endregion
}