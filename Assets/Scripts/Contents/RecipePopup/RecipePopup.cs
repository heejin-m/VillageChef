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

    #endregion

    /// <summary>
    /// 풀에 미리 생성할 개수
    /// Recipe 데이터 상의 재료 최대개수와 동일함
    /// </summary>
    private const int PREVIOUS_MAKE_POOL_CNT = 10; 
    /// <summary>
    /// 레시피 데이터
    /// </summary>
    private RecipeData _recipeData = null;
    private Recipe lastData = null;
    /// <summary>
    /// 현재 페이지 인덱스
    /// </summary>
    private int _currentIndex = 1;

    public override void Awake()
    {
        pool.Create(PREVIOUS_MAKE_POOL_CNT);

        leftArrow.SetOnClickEvent(OnClickLeftButton);
        rightArrow.SetOnClickEvent(OnClickRightButton);
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

        title.text = data.DishName;
        desc.text = data.DishDescription;
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
        _currentIndex = _currentIndex - 1 <= 0 ? 1 : _currentIndex - 1;
        UpdateUI();
    }

    private void OnClickRightButton()
    {
        _currentIndex = _currentIndex + 1 >= lastData.id ? lastData.id : _currentIndex + 1;
        UpdateUI();
    }
}