using JetBrains.Annotations;

public class PlayerModel : AbstractModel
{
    #region ## Events ##

    public delegate void Refresh();
    private Refresh _onRefreshGold;
    public Refresh OnRefreshGold
    {
        get
        {
            return _onRefreshGold;
        }
        set
        {
            _onRefreshGold = null;
            _onRefreshGold += value;
        }
    }

    #endregion

    /// <summary>
    /// 플레이어 정보
    /// </summary>
    private PlayerInfo PlayerInfo = null;

    /// <summary>
    /// 골드 가져오기
    /// </summary>
    /// <returns></returns>
    public long GetGold() => PlayerInfo.Gold;

    /// <summary>
    /// 세팅
    /// </summary>
    /// <param name="saveInfo"></param>
    public void Set(PlayerSaveInfo saveInfo)
    {
        this.PlayerInfo = new PlayerInfo(saveInfo);
    }

    /// <summary>
    /// 골드 사용 가능한지
    /// </summary>
    /// <param name="amount"></param>
    public bool IsCanUseGold(long amount)
    {
        if (amount <= 0) return false;

        if (PlayerInfo.Gold < amount) return false;

        return true;
    }

    /// <summary>
    /// 골드 사용
    /// </summary>
    /// <param name="amount"></param>
    public bool UseGold(long amount)
    {
        if (IsCanUseGold(amount))
        {
            PlayerInfo.UseGold(amount);
            SaveManager.Save(PlayerInfo);
            _onRefreshGold?.Invoke();

            return true;
        }

        return false;
    }

    /// <summary>
    /// 골드 획득
    /// </summary>
    /// <param name="amount"></param>
    public void AddGold(long amount)
    {
        if (amount <= 0) return;

        PlayerInfo.AddGold(amount);
        SaveManager.Save(PlayerInfo);
        _onRefreshGold?.Invoke();
    }
}