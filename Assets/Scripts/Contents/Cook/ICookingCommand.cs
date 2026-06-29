public interface ICookingCommand
{
    void SetRecipeId(int recipeId);
    bool CanExecute();
    CookingResult Execute();
}