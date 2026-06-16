using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WantSellItemInfo
{
    /// <summary>
    /// 판매하려는 아이템의 정보
    /// </summary>
    public InventoryItemInfo info;
    /// <summary>
    /// ID
    /// </summary>
    public int id => info.ID;
    /// <summary>
    /// 수량
    /// </summary>
    public int cnt;
}

public class ShopSellPage : MonoBehaviour
{
    #region Insepctor

    public LoopGridScrollRectCustom scrollRect;
    public LoopVerticalScrollRectCustom sellScrollRect;
    public UITabController typeTab;
    public GameObject emptyObj;

    public Button sellButton;
    public GameObject enableButton;
    public GameObject disableButton;

    #endregion

    /// <summary>
    /// NPC 대사 이벤트
    /// </summary>
    private System.Action<eNPCTalk> _onTalkNPC;
    /// <summary>
    /// 가지고 있는 인벤토리 아이템 리스트
    /// </summary>
    private List<InventoryItemInfo> _haveItemInfos = null;
    /// <summary>
    /// 판매하려고 하는 아이템 리스트
    /// </summary>
    private List<WantSellItemInfo> _wantSellItemInfos = null;
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

        sellButton.SetOnClickEvent(OnClickSellButton);
        typeTab.onChangeTabIndex += OnChangeTabIndex;
        scrollRect.OnProvideData = OnProvideData;
        sellScrollRect.OnProvideData = OnProvideSellData;
        _isInitialized = true;
    }

    public void StartProcess(System.Action<eNPCTalk> onTalkNPC)
    {
        Initialize();

        _onTalkNPC = onTalkNPC;
        _currentTab = eInventoryItemType.Ingredient;
        _wantSellItemInfos = new List<WantSellItemInfo>();

        typeTab.SetTab((short)_currentTab);
        SetData();
        UpdateUI();
        SetScrollview(true);
        SetSellScrollview(true);
    }

    public void CloseProcess()
    {

    }

    private void OnChangeTabIndex(ushort index)
    {
        _currentTab = (eInventoryItemType)index;
        SetData();
        UpdateUI();
        SetScrollview(true);
    }

    private void SetData()
    {
        _haveItemInfos = ModelCenter.Inventory.GetHaveItemListByType(_currentTab);
    }

    private void UpdateUI()
    {
        emptyObj.SetActive(_haveItemInfos == null || _haveItemInfos.Count <= 0);
        UpdateButtonUI();
    }

    private void UpdateButtonUI()
    {
        enableButton.SetActive(_wantSellItemInfos != null && _wantSellItemInfos.Count > 0);
        disableButton.SetActive(_wantSellItemInfos == null || _wantSellItemInfos.Count == 0);
    }

    /// <summary>
    /// 스크롤뷰 리스트 세팅
    /// </summary>
    private void SetScrollview(bool isRefill)
    {
        // 스크롤뷰 세팅
        scrollRect.totalCount = _haveItemInfos?.Count ?? 0;
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
    /// 스크롤뷰 리스트 세팅
    /// </summary>
    private void SetSellScrollview(bool isRefill)
    {
        // 스크롤뷰 세팅
        sellScrollRect.totalCount = _wantSellItemInfos?.Count ?? 0;
        if (isRefill)
        {
            sellScrollRect.RefillCells();
        }
        else
        {
            sellScrollRect.RefreshCells();
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

        ShopSellItem listItem = transform.GetComponent<ShopSellItem>();
        if (listItem != null && _haveItemInfos != null && idx < _haveItemInfos.Count)
        {
            listItem.gameObject.SetActive(true);
            InventoryItemInfo itemInfo = _haveItemInfos[idx];
            listItem.Set(itemInfo, OnClickSellItem, IsWantSellItem(itemInfo));
        }
        else
        {
            transform.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 스크롤뷰 세팅
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="idx"></param>
    private void OnProvideSellData(Transform transform, int idx)
    {
        if (transform == null) return;

        ShopWantSellItem listItem = transform.GetComponent<ShopWantSellItem>();
        if (listItem != null && _wantSellItemInfos != null && idx < _wantSellItemInfos.Count)
        {
            listItem.gameObject.SetActive(true);
            listItem.Set(_wantSellItemInfos[idx], OnClickWantSellItem);
        }
        else
        {
            transform.gameObject.SetActive(false);
        }
    }

    private void OnClickSellItem(InventoryItemInfo itemInfo)
    {
        if (itemInfo == null) return;

        _wantSellItemInfos ??= new();
        var item = _wantSellItemInfos.Find(d => d.info != null && d.info.ID == itemInfo.ID);
        if (item == null)
        {
            _wantSellItemInfos.Add(new WantSellItemInfo
            {
                info = itemInfo,
                cnt = 1,
            });
        }
        else
        {
            // 가지고 있는 아이템 이상으로는 클릭 불가능
            var haveItem = _haveItemInfos?.Find(d => d.ID == itemInfo.ID);
            if (haveItem == null) return;
            if (haveItem.Cnt < item.cnt + 1) return;

            item.cnt++;
        }

        SetSellScrollview(false);
        SetScrollview(false);
        UpdateButtonUI();
    }

    private void OnClickWantSellItem(WantSellItemInfo itemInfo)
    {
        if (_wantSellItemInfos == null) return;
        if (itemInfo == null) return;
        if (itemInfo.info == null) return;

        var item = _wantSellItemInfos.Find(d => d.info != null && d.info.ID == itemInfo.id);
        if (item == null) return;

        int prevCount = _wantSellItemInfos.Count;
        int prevLastVisibleItem = -1;
        if (sellScrollRect != null)
        {
            float offset;
            prevLastVisibleItem = sellScrollRect.GetLastItem(out offset);
        }

        bool isRemovedItem = false;
        if (item.cnt > 1)
        {
            --item.cnt;
        }
        else
        {
            _wantSellItemInfos.Remove(item);
            isRemovedItem = true;
        }

        if (isRemovedItem && prevLastVisibleItem >= prevCount - 1)
        {
            RefillSellScrollviewFromEnd();
        }
        else
        {
            SetSellScrollview(false);
        }

        SetScrollview(false);
        UpdateButtonUI();
    }

    private void RefillSellScrollviewFromEnd()
    {
        if (sellScrollRect == null) return;

        sellScrollRect.totalCount = _wantSellItemInfos?.Count ?? 0;
        sellScrollRect.velocity = Vector2.zero;
        if (sellScrollRect.totalCount <= 0)
        {
            sellScrollRect.ClearCells();
            return;
        }

        sellScrollRect.RefillCellsFromEnd();
    }

    private bool IsWantSellItem(InventoryItemInfo itemInfo)
    {
        if (itemInfo == null || _wantSellItemInfos == null) return false;

        return _wantSellItemInfos.Exists(d => d.info != null && d.info.ID == itemInfo.ID);
    }

    private void OnClickSellButton()
    {
        _onTalkNPC?.Invoke(eNPCTalk.Buy_Talk);
    }
}
