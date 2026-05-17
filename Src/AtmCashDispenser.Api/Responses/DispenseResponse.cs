namespace AtmCashDispenser.Api.Responses
{
    public record DispenseResponse(List<DispenseItem> Items);
    public record DispenseItem(int Denomination, int Count);
}
