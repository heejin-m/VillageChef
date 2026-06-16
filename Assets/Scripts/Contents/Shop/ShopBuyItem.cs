using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopBuyItem : MonoBehaviour
{
    #region Inspector

    public ItemUI itemUI;
    public TMP_Text title;
    public TMP_Text price;

    public GameObject selectedObj;

    #endregion

    private Button _button;
    private ProductInfo _Info = null;
    private System.Action<ShopBuyItem> _onClick = null;
    private bool _isSelected = false;

    public bool IsSelected => _isSelected;

    private void Awake()
    {
        _button = this.GetComponent<Button>();
        _button.SetOnClickEvent(OnClick);
    }

    public void Set(ProductInfo info, System.Action<ShopBuyItem> onClick)
    {
        _Info = info;
        _onClick = onClick;

        var inventoryInfo = ModelCenter.Inventory.GetItemById(info.InventoryItemID);
        itemUI.Set(inventoryInfo);
        itemUI.SetCnt(info.Amount);
        title.text = inventoryInfo.Name;
        price.text = string.Format("{0:n0}", info.Price);

        SetSelected(false);
    }

    public void SetSelected(bool isSelected)
    {
        _isSelected = isSelected;
        selectedObj.SetActive(_isSelected);
    }

    private void OnClick()
    {
        _onClick?.Invoke(this);
    }
}