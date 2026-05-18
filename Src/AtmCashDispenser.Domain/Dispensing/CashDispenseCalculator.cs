using AtmCashDispenser.Domain.Shared;

namespace AtmCashDispenser.Domain.Dispensing
{
    /// <summary>
    /// 払い出しプランを計算するサービスクラス
    /// </summary>
    public class CashDispenseCalculator : ICashDispenseCalculator
    {
        /// <summary>
        /// 払い出しプランを計算する
        /// バックトラックを使用して、在庫の組み合わせを探索し、指定された金額を払い出すための最適なプランを見つける
        /// </summary>
        /// <param name="amount">金額</param>
        /// <returns>払い出しプラン</returns>
        public DispenseResult Calculate(IReadOnlyDictionary<Denomination, int> inventorySnapShot, Money amount)
        {
            int targetAmount = amount.Amount;
            var denoms = Denomination.Descending.ToList();

            int minBills = int.MaxValue;
            Dictionary<Denomination, int>? bestPlan = null;

            var currentPlan = new Dictionary<Denomination, int>();

            void Dfs(int index, int remaining, int currentBills)
            {
                if (remaining == 0)
                {
                    if (currentBills < minBills)
                    {
                        minBills = currentBills;
                        bestPlan = new Dictionary<Denomination, int>(currentPlan);
                    }
                    return;
                }

                if (index >= denoms.Count)
                {
                    return;
                }

                var denom = denoms[index];

                if (currentBills + (remaining / denom.Value) >= minBills)
                {
                    return;
                }

                int available = inventorySnapShot.TryGetValue(denom, out int count) ? count : 0;
                int maxUse = Math.Min(remaining / denom.Value, available);

                for (int use = maxUse; use >= 0; --use)
                {
                    if (use > 0)
                    {
                        currentPlan[denom] = use;
                    }
                    
                    Dfs(index + 1, remaining - use * denom.Value, currentBills + use);

                    if (use > 0)
                    {
                        currentPlan.Remove(denom);
                    }
                }
            }

            Dfs(0, targetAmount, 0);

            if (minBills == int.MaxValue || bestPlan == null)
            {
                return new DispenseResult.Failure(DispenseFailureReason.InsufficientCombination);
            }
            return new DispenseResult.Success(new DispensePlan(bestPlan));
        }
    }
}
