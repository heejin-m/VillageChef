using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class ShopPopup : PopupWindow
{
    #region Inspector

    public UITabController shopTab;
    public ShopBuyPage shopBuyPage;
    public ShopSellPage shopSellPage;

    public ShopNPCTalk npcTalkSO;
    public TMP_Text npc_talk;
    public Image npc_portrait;

    #endregion

    /// <summary>
    /// Shop NPC 초상화 리소스
    /// </summary>
    private const string NPC_PORTRAIT_NORMAL = "ShopKeeperPortrait_01";
    private const string NPC_PORTRAIT_HAPPY = "ShopKeeperPortrait_01_Smile";

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

        if (_shopNPCTalkDict.TryGetValue(eNPCTalk.Hello_Talk, out var list))
        {
            if (list == null || list.Count <= 0)
            {
                return;
            }

            npc_talk.text = RandomSelect(list);
        }
    }

    public override void StartProcess()
    {
        base.StartProcess();
        SetData();

        Initialize();
        shopBuyPage.StartProcess(OnTalkNPC);
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
            if (!_shopNPCTalkDict.TryGetValue(item.eNPCTalk, out var list))
            {
                list = new List<string>();
                _shopNPCTalkDict.Add(item.eNPCTalk, list);
            }
            list.Add(item.talk);
        }
    }

    private void OnChangeTabIndex(ushort index)
    {
        Initialize();

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

    private void Initialize()
    {
        AtlasLoadManager.SetImageSprite(npc_portrait, eAtlas.ShopUI, NPC_PORTRAIT_NORMAL);
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

        switch (eNPCTalk)
        {
            case eNPCTalk.Sell_Talk:
            case eNPCTalk.Sad_Talk:
            case eNPCTalk.Hello_Talk:
                AtlasLoadManager.SetImageSprite(npc_portrait, eAtlas.ShopUI, NPC_PORTRAIT_NORMAL);
                break;
            case eNPCTalk.Happy_Talk:
            case eNPCTalk.Buy_Talk:
                AtlasLoadManager.SetImageSprite(npc_portrait, eAtlas.ShopUI, NPC_PORTRAIT_HAPPY);
                break;
        }
    }

    private string RandomSelect(List<string> list)
    {
        int choice = UnityEngine.Random.Range(0, list.Count);
        return list[choice];
    }
}