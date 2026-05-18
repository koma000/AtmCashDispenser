using AtmCashDispenser.Domain.Shared;

namespace AtmCashDispenser.Domain.Dispensing
{
    /// <summary>
    /// 在庫管理クラス
    /// </summary>
    public class CashInventory
    {
        private readonly Dictionary<Denomination, int> _inventory;
        private readonly object _lock = new();

        public CashInventory(Dictionary<Denomination, int> initial)
        {
            _inventory = new Dictionary<Denomination, int>(initial);
        }

        public DispenseResult CalcDispense(Money amount, ICashDispenseCalculator calculator)
        {
            lock(_lock)
            {
                return calculator.Calculate(_inventory.AsReadOnly(), amount);
            }
        }

        public void ApplyDispense(DispensePlan plan)
        {
            lock(_lock)
            {
                foreach (var (denom, count) in plan.DispenseDetails)
                {
                  _inventory[denom] -= count;
                }
            }
        }

        public int GetCount(Denomination denomination)
        {
            lock(_lock)
            {
                return _inventory.TryGetValue(denomination, out var count) ? count : 0;
            }
        }

        public override string ToString()
        {
            var inventoryDetails = _inventory.Select(kvp => $"{kvp.Key.Value}円: {kvp.Value}枚");
            return string.Join(", ", inventoryDetails);
        }
    }
}
