using UnityEngine;
using UnityEngine.UI;

public class ShopBuyPage : MonoBehaviour
{
    #region Insepctor

    public UITabController typeTab;
    public Button buyButton;

    #endregion

    /// <summary>
    /// NPC 대사
    /// </summary>
    private System.Action<eNPCTalk> _onTalkNPC;

    public enum eType
    {
        Ingredients = 0,
        Dish,
    }

    public void Awake()
    {
        buyButton.SetOnClickEvent(OnClickBuyButton);
        typeTab.onChangeTabIndex += OnChangeTabIndex;
    }

    private void OnChangeTabIndex(ushort index)
    {
    }

    public void StartProcess(System.Action<eNPCTalk> onTalkNPC)
    {
        _onTalkNPC = onTalkNPC;
    }

    public void CloseProcess()
    {

    }

    private void OnClickBuyButton()
    {

    }
}