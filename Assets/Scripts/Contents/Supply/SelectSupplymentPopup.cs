using UnityEngine.UI;

public class SelectSupplymentPopup : PopupWindow
{
    #region Inspector

    public Button orchardButton;
    public Button fieldButton;

    #endregion

    public override void Awake()
    {
        base.Awake();
        orchardButton.SetOnClickEvent(OnClickOrchardButton);
        fieldButton.SetOnClickEvent(OnClickFieldButton);
    }

    private async void OnClickOrchardButton()
    {
        _ = await PopupManager.Instance.OpenPopup<OrcharSupplyPopup>(ePopup.OrcharSupplyPopup);
    }

    private async void OnClickFieldButton()
    {
        _ = await PopupManager.Instance.OpenPopup<FieldSupplyPopup>(ePopup.FieldSupplyPopup);
    }
}