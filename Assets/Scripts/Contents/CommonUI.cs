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
        ModelCenter.Inventory.TESTSAVEItem(1, 20);
        ModelCenter.Inventory.TESTSAVEItem(2, 20);
        ModelCenter.Inventory.TESTSAVEItem(3, 20);
        ModelCenter.Inventory.TESTSAVEItem(4, 20);
        ModelCenter.Inventory.TESTSAVEItem(5, 20);
        ModelCenter.Inventory.TESTSAVEItem(40, 10);
        ModelCenter.Inventory.TESTSAVEItem(41, 10);
        ModelCenter.Inventory.TESTSAVEItem(42, 10);

        _ = await PopupManager.Instance.OpenPopup<InventoryPopup>(ePopup.InventoryPopup);
    }
}