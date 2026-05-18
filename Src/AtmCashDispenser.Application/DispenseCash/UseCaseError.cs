namespace AtmCashDispenser.Application.DispenseCash
{
    public record UseCaseError(string Code, string Message)
    {
        public static readonly UseCaseError InvalidAmount = new("INVALID_AMOUNT", "金額は1以上");
        public static readonly UseCaseError LimitExceeded = new("LIMIT_EXCEEDS", "上限超過");
        public static readonly UseCaseError NotDispensable = new("NOT_DISPENSABLE", "払い出し不可");
    }
}
