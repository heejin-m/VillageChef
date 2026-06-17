using UnityEngine;
using UnityEngine.UI;

public class ShopSellItem : MonoBehaviour
{
    #region Inspector

    public ItemUI itemUI;
    public GameObject selectedObj;

    #endregion

    private Button _button = null;
    private bool _isSelected = false;
    private InventoryItemInfo _info = null;
    private System.Action<InventoryItemInfo> _onClick = null;

    private void Awake()
    {
        _button = this.GetComponent<Button>();
        _button?.SetOnClickEvent(OnClick);
    }

    public void Set(InventoryItemInfo info, System.Action<InventoryItemInfo> onClick, bool isSelected)
    {
        _info = info;
        _onClick = onClick;

        itemUI.Set(info);
        itemUI.SetCnt(info.Cnt);

        _isSelected = isSelected;
        UpdateUI();
    }

    public void SetSelected(bool isSelected)
    {
        _isSelected = isSelected;
        UpdateUI();
    }

    private void UpdateUI()
    {
        selectedObj?.SetActive(_isSelected);
    }

    public void OnClick()
    {
        _onClick?.Invoke(_info);
    }
}
