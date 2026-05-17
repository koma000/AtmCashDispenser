using AtmCashDispenser.Domain.Shared;

namespace AtmCashDispenser.Domain.Dispensing
{
    /// <summary>
    /// 払い出しプランを計算するサービスクラス
    /// </summary>
    public class CashDispenseCalculator
    {
        /// <summary>
        /// 払い出しプランを計算する
        /// Greedyアルゴリズム。最適解を保証しない場合があることに注意
        /// </summary>
        /// <param name="amount">金額</param>
        /// <returns>払い出しプラン</returns>
        public DispenseResult Calculate(CashInventory inventory, Money amount)
        {
            var details = new Dictionary<Denomination, int>();
            var remaining = amount;

            foreach (var denom in Denomination.Descending)
            {
                var available = inventory.GetCount(denom);
                if (available <= 0)
                {
                    continue;
                }

                var needed = remaining.Amount / denom.Value;
                var use = Math.Min(needed, available);
                if (use > 0)
                {
                    details[denom] = use;
                    remaining = remaining.Subtract(Money.Create(use * denom.Value));
                }
            }
            if (!remaining.IsZero())
            {
                return new DispenseResult.Failure("払い出し不可能な金額です");
            }
            return new DispenseResult.Success(new DispensePlan(details));
        }
    }
}
