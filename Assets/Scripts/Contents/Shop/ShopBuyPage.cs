using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopBuyPage : MonoBehaviour
{
    #region Insepctor

    public LoopVerticalScrollRectCustom scrollRect;
    public UITabController typeTab;
    public Button buyButton;
    public GameObject enableButton;
    public GameObject disableButton;

    #endregion

    /// <summary>
    /// NPC 대사
    /// </summary>
    private System.Action<eNPCTalk> _onTalkNPC;
    /// <summary>
    /// 판매 상품 데이터 리스트
    /// </summary>
    private List<ProductInfo> _productInfos = null;
    /// <summary>
    /// 현재 탭
    /// </summary>
    private eInventoryItemType _currentTab = eInventoryItemType.Ingredient;
    /// <summary>
    /// 현재 선택된 아이템
    /// </summary>
    private ShopBuyItem _selectedItem = null;
    /// <summary>
    /// 초기화 여부
    /// </summary>
    private bool _isInitialized = false;

    public void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        if (_isInitialized) return;

        buyButton.SetOnClickEvent(OnClickBuyButton);
        typeTab.onChangeTabIndex += OnChangeTabIndex;
        scrollRect.OnProvideData = OnProvideData;
        _isInitialized = true;
    }

    public void StartProcess(System.Action<eNPCTalk> onTalkNPC)
    {
        Initialize();

        _onTalkNPC = onTalkNPC;
        _currentTab = eInventoryItemType.Ingredient;
        Unselect();

        typeTab.SetTab((short)_currentTab);
        SetData();
        SetScrollview(true);
        UpdateButtonUI();
    }

    public void CloseProcess()
    {

    }

    private void OnChangeTabIndex(ushort index)
    {
        _currentTab = (eInventoryItemType)index;
        Unselect();
        SetData();
        SetScrollview(true);
        UpdateButtonUI();
    }

    private void SetData()
    {
        _productInfos = ModelCenter.Product.GetProductListByType(_currentTab);
    }

    private void UpdateButtonUI()
    {
        enableButton.SetActive(_selectedItem != null);
        disableButton.SetActive(_selectedItem == null);
    }

    /// <summary>
    /// 스크롤뷰 리스트 세팅
    /// </summary>
    private void SetScrollview(bool isRefill)
    {
        if (isRefill)
        {
            Unselect();
        }

        // 스크롤뷰 세팅
        scrollRect.totalCount = _productInfos.Count;
        if (isRefill)
        {
            scrollRect.RefillCells();
        }
        else
        {
            scrollRect.RefreshCells();
        }
    }

    /// <summary>
    /// 스크롤뷰 세팅
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="idx"></param>
    private void OnProvideData(Transform transform, int idx)
    {
        if (transform == null) return;

        ShopBuyItem listItem = transform.GetComponent<ShopBuyItem>();
        if (listItem != null && _productInfos != null && idx < _productInfos.Count)
        {
            listItem.gameObject.SetActive(true);
            listItem.Set(_productInfos[idx], OnClickItem);
        }
        else
        {
            transform.gameObject.SetActive(false);
        }
    }

    private void OnClickBuyButton()
    {
        if (_selectedItem == null) return;

        _onTalkNPC?.Invoke(eNPCTalk.Sell_Talk);
    }

    private void OnClickItem(ShopBuyItem item)
    {
        if (item == null) return;

        if (_selectedItem == item && item.IsSelected)
        {
            Unselect();
            return;
        }

        Unselect();
        _selectedItem = item;
        _selectedItem.SetSelected(true);
        UpdateButtonUI();
    }

    private void Unselect()
    {
        if (_selectedItem != null)
        {
            _selectedItem.SetSelected(false);
            _selectedItem = null;

        }
        UpdateButtonUI();
    }
}