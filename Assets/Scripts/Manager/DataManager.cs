public class DataManager : SingletonBehaviour<DataManager>
{
    public RecipeData RecipeData { get; private set; }

    public void Initialize()
    {
        RecipeData = new RecipeData();
        RecipeData.Initialize();
    }
}