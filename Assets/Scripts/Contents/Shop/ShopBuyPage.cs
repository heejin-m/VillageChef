using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopBuyPage : MonoBehaviour
{
    #region Insepctor

    public LoopVerticalScrollRectCustom scrollRect;
    public UITabController typeTab;
    public Button buyButton;

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

        typeTab.SetTab((short)_currentTab);
        SetData();
        SetScrollview(true);
    }

    public void CloseProcess()
    {

    }

    private void OnChangeTabIndex(ushort index)
    {
        _currentTab = (eInventoryItemType)index;
        SetData();
        SetScrollview(true);
    }

    private void SetData()
    {
        _productInfos = ModelCenter.Product.GetProductListByType(_currentTab);
    }

    /// <summary>
    /// 스크롤뷰 리스트 세팅
    /// </summary>
    private void SetScrollview(bool isRefill)
    {
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
            listItem.Set(_productInfos[idx]);
        }
        else
        {
            transform.gameObject.SetActive(false);
        }
    }

    private void OnClickBuyButton()
    {
        _onTalkNPC?.Invoke(eNPCTalk.Sell_Talk);
    }
}
