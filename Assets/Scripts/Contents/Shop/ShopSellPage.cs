using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopSellPage : MonoBehaviour
{
    #region Insepctor

    public LoopGridScrollRectCustom scrollRect;
    public UITabController typeTab;
    public Button sellButton;
    public GameObject emptyObj;

    #endregion

    /// <summary>
    /// NPC 대사 이벤트
    /// </summary>
    private System.Action<eNPCTalk> _onTalkNPC;
    /// <summary>
    /// 가지고 있는 인벤토리 아이템
    /// </summary>
    private List<InventoryItemInfo> _haveItemInfos = null;
    /// <summary>
    /// 현재 탭
    /// </summary>
    private eInventoryItemType _currentTab = eInventoryItemType.Ingredient;

    public void Awake()
    {
        sellButton.SetOnClickEvent(OnClickSellButton);
        typeTab.onChangeTabIndex += OnChangeTabIndex;
        scrollRect.OnProvideData = OnProvideData;
    }

    public void StartProcess(System.Action<eNPCTalk> onTalkNPC)
    {
        _onTalkNPC = onTalkNPC;
        _currentTab = eInventoryItemType.Ingredient;

        typeTab.SetTab((short)_currentTab);
        SetData();
        UpdateUI();
        SetScrollview(true);
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
    }

    /// <summary>
    /// 스크롤뷰 리스트 세팅
    /// </summary>
    private void SetScrollview(bool isRefill)
    {
        // 스크롤뷰 세팅
        scrollRect.totalCount = _haveItemInfos.Count;
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

        ShopSellItem listItem = transform.GetComponent<ShopSellItem>();
        if (listItem != null && _haveItemInfos != null && idx < _haveItemInfos.Count)
        {
            listItem.gameObject.SetActive(true);
            listItem.Set(_haveItemInfos[idx]);
        }
        else
        {
            transform.gameObject.SetActive(false);
        }
    }

    private void OnClickSellButton()
    {
        _onTalkNPC?.Invoke(eNPCTalk.Buy_Talk);
    }
}