public class CookingResult
{
    private bool _isSuccess;
    private int _resultItemId;
    private int _resultAmount;
    private string _failReason;

    public static CookingResult Success(int itemId, int amount)
    {
        return new CookingResult
        {
            _isSuccess = true,
            _resultItemId = itemId,
            _resultAmount = amount
        };
    }

    public static CookingResult Fail(string reason)
    {
        return new CookingResult
        {
            _isSuccess = false,
            _failReason = reason
        };
    }
}