using UnityEngine.UI;

public class HomeWindow : BaseWindow
{
    #region Insepctor

    public Button tesetbutton;

    #endregion

    public void Awake()
    {
        tesetbutton.onClick.AddListener(Test);
    }

    public void Test()
    {
        PopupManager.Instance.OpenPopup(PopupEnum.ePopup.RecipePopup);
    }
}