using System.Threading.Tasks;
using TMPro;
using UnityEngine;
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

    public override async Task<bool> OpenReady()
    {
        await AtlasLoadManager.LoadSpriteAtlasAsync(eAtlas.ShopUI);
        return await base.OpenReady();
    }

    public override void StartProcess()
    {
        base.StartProcess();

        Initialize();
        SetNPCTalk(eNPCTalk.Hello_Talk);
        shopBuyPage.StartProcess(OnTalkNPC);
    }

    public override void CloseProcess()
    {
        base.CloseProcess();
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
        SetNPCTalk(eNPCTalk);

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

    private void SetNPCTalk(eNPCTalk eNPCTalk)
    {
        if (npcTalkSO == null)
        {
            Debug.LogWarning("ShopNPCTalk is not assigned.");
            return;
        }

        if (npcTalkSO.TryGetRandomTalk(eNPCTalk, out var talk))
        {
            npc_talk.text = talk;
        }
    }
}
