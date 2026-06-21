using UnityEngine.UI;

public class FieldSupplyPopup : PopupWindow
{
    #region Inspector

    public Button supplyButton;

    #endregion

    private readonly FieldSupplyStrategy _fieldStrategy = new();

    public override void Awake()
    {
        supplyButton.SetOnClickEvent(OnClickSupplyButton);
    }

    public void OnClickSupplyButton()
    {
        var infos = ModelCenter.Supply.GetInfosByType(eIngredientSupplyType.Field);
        _fieldStrategy.Supply(infos);
    }
}