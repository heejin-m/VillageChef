public class CookingResult
{
    public bool IsSuccess { get; private set; }
    public int ResultItemId { get; private set; }
    public int ResultAmount { get; private set; }
    public string FailReason { get; private set; }

    public static CookingResult Success(int itemId, int amount)
    {
        return new CookingResult
        {
            IsSuccess = true,
            ResultItemId = itemId,
            ResultAmount = amount
        };
    }

    public static CookingResult Fail(string reason)
    {
        return new CookingResult
        {
            IsSuccess = false,
            FailReason = reason
        };
    }
}