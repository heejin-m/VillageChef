using TMPro;
using UnityEngine.UI;

public class RecipePopup : PopupWindow
{
    #region Inspector

    public TMP_Text title;
    public TMP_Text desc;

    public ObjectPool pool;

    public Button leftArrow;
    public Button rightArrow;
    public Button closeButton;

    #endregion
    
    /// <summary>
    /// 레시피 데이터
    /// </summary>
    private RecipeData _recipeData = null;
    private Recipe lastData = null;
    /// <summary>
    /// 현재 페이지 인덱스
    /// </summary>
    private ushort _currentIndex = 1;

    public override void Awake()
    {
        leftArrow.SetOnClickEvent(OnClickLeftButton);
        rightArrow.SetOnClickEvent(OnClickRightButton);
        closeButton.SetOnClickEvent(OnClickCloseButton);
    }

    public override void StartProcess()
    {
        base.StartProcess();

        SetData();
        UpdateUI();
    }

    private void SetData()
    {
        _recipeData = DataManager.Instance.GetData<RecipeData>();
        lastData = _recipeData.GetLastData();
    }

    private void UpdateUI()
    {
        var data = _recipeData.GetData(_currentIndex);
        title.text = data.name;
        desc.text = data.description;
    }

    private void OnClickLeftButton()
    {
        _currentIndex = _currentIndex - 1 <= 0 ? (ushort)1 : (ushort)(_currentIndex - 1);
        UpdateUI();
    }

    private void OnClickRightButton()
    {
        _currentIndex = _currentIndex + 1 >= lastData.id ? lastData.id : (ushort)(_currentIndex + 1);
        UpdateUI();
    }

    private void OnClickCloseButton()
    {
        UISystemManager.Instance.BackPress();
    }
}