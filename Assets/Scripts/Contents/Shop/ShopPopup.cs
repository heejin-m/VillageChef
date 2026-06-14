using JetBrains.Annotations;
using UnityEngine.UI;

public class ShopPopup : PopupWindow
{
    #region Inspector

    public Button testButton;
    public int testId;

    #endregion

    public override void Awake()
    {
        testButton.SetOnClickEvent(OnClickTestButton);
        base.Awake();
    }

    private void OnClickTestButton()
    {
        //ModelCenter.Recipe.AddRecipe(testId);
        //ModelCenter.Inventory.TESTSAVEItem(testId, 1);
    }
}