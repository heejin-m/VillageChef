using UnityEngine;
using UnityEngine.UI;

public class ShopWantSellItem : MonoBehaviour
{
    #region Inspector

    public ItemUI itemUI;

    #endregion

    private Button _button = null;
    private WantSellItemInfo _info = null;
    private System.Action<WantSellItemInfo> _onClick = null;

    private void Awake()
    {
        _button = this.GetComponent<Button>();
        _button?.SetOnClickEvent(OnClick);
    }

    public void Set(WantSellItemInfo sellInfo, System.Action<WantSellItemInfo> onClick)
    {
        _info = sellInfo;
        _onClick = onClick;
        if (sellInfo == null) return;

        itemUI?.Set(sellInfo.info);
        itemUI?.SetCnt(sellInfo.cnt);
    }

    public void OnClick()
    {
        _onClick?.Invoke(_info);
    }
}
