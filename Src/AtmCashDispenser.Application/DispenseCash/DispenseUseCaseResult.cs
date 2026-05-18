namespace AtmCashDispenser.Application.DispenseCash
{
    public abstract record DispenseUseCaseResult
    {
        private DispenseUseCaseResult() { }
        public sealed record Success(TransactionId TransactionId, IReadOnlyList<DispenseItemDto> DispenseDetails) : DispenseUseCaseResult;
        public sealed record Failure(UseCaseError Error) : DispenseUseCaseResult;
    }
}
