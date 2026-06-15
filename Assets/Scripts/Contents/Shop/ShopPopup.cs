using System.Collections.Generic;
using TMPro;

public class ShopPopup : PopupWindow
{
    #region Inspector

    public UITabController shopTab;
    public ShopBuyPage shopBuyPage;
    public ShopSellPage shopSellPage;

    public ShopNPCTalk npcTalkSO;
    public TMP_Text npc_talk;

    #endregion

    /// <summary>
    /// Shop NPC 대사 컬렉션
    /// </summary>
    private Dictionary<eNPCTalk, List<string>> _shopNPCTalkDict = new();

    public enum eShopTab
    {
        /// <summary>
        /// 상점 구매
        /// </summary>
        ShopBuy = 0,
        /// <summary>
        /// 상점 판매
        /// </summary>
        ShopSell,
    }

    public override void Awake()
    {
        base.Awake();
        shopTab.onChangeTabIndex += OnChangeTabIndex;
    }

    public override void StartProcess()
    {
        base.StartProcess();
        SetData();
    }

    public override void CloseProcess()
    {
        base.CloseProcess();
    }

    private void SetData()
    {
        _shopNPCTalkDict.Clear();
        foreach (var item in npcTalkSO.talkList)
        {
            if (_shopNPCTalkDict.TryGetValue(item.eNPCTalk, out var list))
            {
                list ??= new();
                list.Add(item.talk);
            }

            _shopNPCTalkDict.Add(item.eNPCTalk, list);
        }
    }

    private void OnChangeTabIndex(ushort index)
    {
        shopBuyPage.CloseProcess();
        shopSellPage.CloseProcess();

        switch ((eShopTab)index)
        {
            case eShopTab.ShopBuy:
                shopBuyPage.StartProcess(OnTalkNPC);
                break;
            case eShopTab.ShopSell:
                shopSellPage.StartProcess(OnTalkNPC);
                break;
        }
    }

    private void OnTalkNPC(eNPCTalk eNPCTalk)
    {
        if (_shopNPCTalkDict.TryGetValue(eNPCTalk, out var list))
        {
            if (list == null || list.Count <= 0)
            {
                return;
            }

            npc_talk.text = RandomSelect(list);
        }
    }

    private string RandomSelect(List<string> list)
    {
        int choice = UnityEngine.Random.Range(0, list.Count);
        return list[choice];
    }
}