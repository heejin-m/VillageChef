using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RecipePopup : PopupWindow
{
    #region Inspector

    public GameObject knownObj;
    public GameObject unknownObj;

    public TMP_Text title;
    public TMP_Text desc;
    public TMP_Text index;

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
        pool.Create();

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
        var isHave = ModelCenter.Recipe.IsHave(data.id);

        knownObj.SetActive(isHave);
        unknownObj.SetActive(!isHave);

        title.text = data.Name;
        desc.text = data.Description;
        index.text = _currentIndex.ToString();

        UpdateIngredientUI(data);
    }

    private void UpdateIngredientUI(Recipe data)
    {
        pool.HideAll();

        var ingredientList = data.GetIngredientIdList();
        foreach (var ingredient in ingredientList)
        {
            var go = pool.Get<RecipeItem>();
            go.transform.SetParent(pool.transform);
            go.transform.Initialize();
            go.Set(ingredient);
            go.gameObject.SetActive(true);
        }
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