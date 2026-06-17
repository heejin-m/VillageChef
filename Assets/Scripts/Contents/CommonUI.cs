using UnityEngine;
using UnityEngine.UI;

public class CommonUI : MonoBehaviour
{
    #region Inspector

    public Button inventoryButton;

    #endregion

    public void Awake()
    {
        inventoryButton.SetOnClickEvent(OnClickInventoryButton);
    }

    private async void OnClickInventoryButton()
    {
        _ = await PopupManager.Instance.OpenPopup<InventoryPopup>(ePopup.InventoryPopup);
    }
}