public class RecipeInfo
{
    /// <summary>
    /// ID
    /// </summary>
    public int ID { get; private set; }
    /// <summary>
    /// 저장 정보
    /// </summary>
    public RecipeSaveInfo SaveInfo { get; private set; }
    /// <summary>
    /// 데이터
    /// </summary>
    public Recipe Recipe { get; private set; }
    /// <summary>
    /// 레시피 보유 여부
    /// </summary>
    public bool IsHave => SaveInfo != null;

    #region ## Construct ##

    public RecipeInfo(int id, RecipeSaveInfo info) : base()
    {
        var data = DataManager.Instance.GetData<RecipeData>();

        this.ID = id;
        this.Recipe = data.GetData(id);
        this.SaveInfo = info;
    }

    #endregion
}