using UnityEngine.UI;

public class RecipePopup : PopupWindow
{
    #region Inspector

    public Button closeButton;

    #endregion
    
    /// <summary>
    /// 레시피 데이터
    /// </summary>
    private RecipeData _recipeData = null;

    public override void Awake()
    {
        closeButton.SetOnClickEvent(OnClickCloseButton);
    }

    public override void StartProcess()
    {
        base.StartProcess();

        SetData();
        UpdateUI();
    }

    private void OnClickCloseButton()
    {
        UISystemManager.Instance.BackPress();
    }

    private void SetData()
    {
        _recipeData = DataManager.Instance.GetData<RecipeData>();
    }

    private void UpdateUI()
    {

    }
}