namespace AtmCashDispenser.Domain.Dispensing
{
    public abstract record DispenseResult
    {
        private DispenseResult() { }

        public sealed record Success(DispensePlan Plan) : DispenseResult;
        public sealed record Failure(string Reason) : DispenseResult;
    }
}
