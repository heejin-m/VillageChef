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

    private void Awake()
    {
        _button = this.GetComponent<Button>();
        _button?.SetOnClickEvent(OnClick);
    }

    public void Set(InventoryItemInfo infos)
    {
        itemUI.Set(infos);

        _isSelected = false;
        UpdateUI();
    }

    private void UpdateUI()
    {
        selectedObj.SetActive(selectedObj);
    }

    public void OnClick()
    {
        _isSelected = !_isSelected;
        UpdateUI();
    }
}