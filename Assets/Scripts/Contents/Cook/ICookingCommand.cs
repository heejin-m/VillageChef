public interface ICookingCommand
{
    bool CanExecute();
    CookingResult Execute();
}