namespace AtmCashDispenser.Application.DispenseCash
{
    public abstract record DispenseUseCaseResult
    {
        private DispenseUseCaseResult() { }
        public sealed record Success(Dictionary<int, int> DispenseDetails) : DispenseUseCaseResult;
        public sealed record Failure(string Reason) : DispenseUseCaseResult;
    }
}
