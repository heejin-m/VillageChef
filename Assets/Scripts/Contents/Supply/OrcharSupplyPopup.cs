using UnityEngine.UI;

public class OrcharSupplyPopup : PopupWindow
{
    #region Inspector

    public Button supplyButton;

    #endregion

    private readonly OrchardSupplyStrategy _orchardStrategy = new();

    public override void Awake()
    {
        supplyButton.SetOnClickEvent(OnClickSupplyButton);
    }

    public void OnClickSupplyButton()
    {
        var infos = ModelCenter.Supply.GetInfosByType(eIngredientSupplyType.Orchard);
        _orchardStrategy.Supply(infos);
    }
}