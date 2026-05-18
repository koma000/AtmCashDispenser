namespace AtmCashDispenser.Api.Responses
{
    public record DispenseResponse(Guid TransactionId, IReadOnlyList<DispenseItem> Items);
    public record DispenseItem(int Denomination, int Count);
}
