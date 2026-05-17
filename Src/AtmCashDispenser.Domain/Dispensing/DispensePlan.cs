using AtmCashDispenser.Domain.Shared;
using System.Collections.ObjectModel;

namespace AtmCashDispenser.Domain.Dispensing
{
    /// <summary>
    /// 払い出しプランを表す値オブジェクト
    /// </summary>
    public class DispensePlan
    {
        public IReadOnlyDictionary<Denomination, int> DispenseDetails { get; }

        public DispensePlan(Dictionary<Denomination, int> dispenseDetails)
        {
            if (dispenseDetails.Any(x => x.Value <= 0))
            {
                throw new ArgumentException("払い出し枚数は1以上でなければなりません。", nameof(dispenseDetails));
            }
            DispenseDetails = new ReadOnlyDictionary<Denomination, int>(dispenseDetails);
        }

        public override string ToString()
        {
            var details = DispenseDetails.Select(kvp => $"{kvp.Key.Value}円 x {kvp.Value}枚");
            return string.Join(", ", details);
        }
    }
}
