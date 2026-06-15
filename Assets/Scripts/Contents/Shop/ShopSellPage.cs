using UnityEngine;
using UnityEngine.UI;

public class ShopSellPage : MonoBehaviour
{
    #region Insepctor

    public UITabController typeTab;
    public Button sellButton;

    #endregion

    /// <summary>
    /// NPC 대사 이벤트
    /// </summary>
    private System.Action<eNPCTalk> _onTalkNPC;

    public enum eType
    {
        Ingredients = 0,
        Dish,
    }

    public void Awake()
    {
        sellButton.SetOnClickEvent(OnClickSellButton);
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

    private void OnClickSellButton()
    {

    }
}