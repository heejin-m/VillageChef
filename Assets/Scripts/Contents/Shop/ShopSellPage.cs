using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopSellPage : MonoBehaviour
{
    #region Insepctor

    public InfinityScrollRect scrollRect;
    public UITabController typeTab;
    public Button sellButton;

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
    }

    private void OnChangeTabIndex(ushort index)
    {
        _currentTab = (eInventoryItemType)index;
        SetData();
    }

    public void StartProcess(System.Action<eNPCTalk> onTalkNPC)
    {
        _onTalkNPC = onTalkNPC;
        _currentTab = eInventoryItemType.Ingredient;

        typeTab.SetTab((short)_currentTab);
        SetData();
        SetScrollRect();
    }

    public void CloseProcess()
    {

    }

    private void SetData()
    {
        _haveItemInfos = ModelCenter.Inventory.GetHaveItemListByType(_currentTab);
    }

    private void SetScrollRect()
    {
        scrollRect.onUpdateItem.AddListener((item, index) =>
        {
            item.GetComponent<ShopSellItem>().Set(_haveItemInfos[index]);
        });

        scrollRect.SetTotalCount(_haveItemInfos.Count);
    }

    private void OnClickSellButton()
    {
        _onTalkNPC?.Invoke(eNPCTalk.Buy_Talk);
    }
}