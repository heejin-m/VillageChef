using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopBuyPage : MonoBehaviour
{
    #region Insepctor

    public InfinityScrollRect scrollRect;
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

    public void Awake()
    {
        buyButton.SetOnClickEvent(OnClickBuyButton);
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
        _productInfos = ModelCenter.Product.GetProductListByType(_currentTab);
    }

    private void SetScrollRect()
    {
        scrollRect.onUpdateItem.AddListener((item, index) =>
        {
            item.GetComponent<ShopBuyItem>().Set(_productInfos[index]);
        });

        scrollRect.SetTotalCount(_productInfos.Count);
    }

    private void OnClickBuyButton()
    {
        _onTalkNPC?.Invoke(eNPCTalk.Sell_Talk);
    }
}