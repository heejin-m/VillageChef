using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CookingPopup : PopupWindow
{
    #region Insepctor

    public ObjectPool pool;
    public TMP_Text dishTitle;
    public Image dishIma;
    public TMP_Text dishDesc;
    public GameObject emptyObj;

    public GameObject cookButton;
    public GameObject cookDisableButton;
    public Button cookingButton;

    #endregion

    private ICookingCommand _command;
    private RecipeInfo _selectRecipe = null;

    public override void Awake()
    {
        cookingButton.SetOnClickEvent(OnClickCookingButton);
    }

    public override void StartProcess()
    {
        base.StartProcess();
        emptyObj.SetActive(true);
        UpdateUI();
    }

    public override void CloseProcess()
    {
        base.CloseProcess();
        pool.HideAll();
    }

    public void UpdateUI()
    {
        var list = ModelCenter.Recipe.GetHaveRecipeList();
        if (list == null) return;
            
        foreach (var info in list)
        {
            var item = pool.Get<CookingPopupRecipeItem>();
            item.transform.SetParent(pool.transform);
            item.transform.Initialize();
            item.gameObject.SetActive(true);
            item.Set(info.Id, info.Recipe.DishName, OnClickItem);
        }
    }

    public void SetCommand(int recipeId)
    {
        _command ??= new CookRecipeCommand(recipeId);
        _command.SetRecipeId(recipeId);
    }

    private void OnClickItem(int id)
    {
        emptyObj.SetActive(false);
        _selectRecipe = ModelCenter.Recipe.GetRecipe(id);
        dishTitle.text = _selectRecipe.Recipe.DishName;
        dishDesc.text = _selectRecipe.Recipe.DishDescription;
        AtlasLoadManager.SetImageSprite(dishIma, eAtlas.FoodUI, _selectRecipe.Recipe.DishResourceName);
        SetCommand(_selectRecipe.Id);

        //cookButton.SetActive(_command != null && _command.CanExecute());
        cookDisableButton.SetActive(_command == null || !_command.CanExecute());
    }

    private void OnClickCookingButton()
    {
        if (_command == null)
        {
            Debug.LogWarning("Cooking command is null.");
            return;
        }

        if (!_command.CanExecute())
        {
            Debug.Log("요리할 수 없습니다.");
            return;
        }

        CookingResult result = _command.Execute();
        if (result.IsSuccess)
        {
            Debug.Log($"요리 성공: ItemId={result.ResultItemId}, Amount={result.ResultAmount}");
            cookDisableButton.SetActive(!_command.CanExecute()); // 비활성 버튼 UI 갱신
        }
    }
}
