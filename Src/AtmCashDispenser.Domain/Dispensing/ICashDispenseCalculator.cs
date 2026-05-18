using AtmCashDispenser.Domain.Shared;

namespace AtmCashDispenser.Domain.Dispensing
{
    public interface ICashDispenseCalculator
    {
        DispenseResult Calculate(IReadOnlyDictionary<Denomination, int> snapShot, Money amount);
    }
}
