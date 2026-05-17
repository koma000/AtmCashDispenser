using AtmCashDispenser.Domain.Dispensing;
using AtmCashDispenser.Domain.Shared;

namespace AtmCashDispenser.Application.DispenseCash
{
    public class DispenseCashUseCase
    {
        private readonly CashInventory _inventory;

        public DispenseCashUseCase(CashInventory inventory)
        {
            _inventory = inventory;
        }

        public DispenseUseCaseResult Execute(int amount)
        {
            // 入力をドメインに変換
            var money = Money.Create(amount);

            // 払い出しプランを計算
            var calculator = new CashDispenseCalculator();

            var domainResult = calculator.Calculate(_inventory, money);

            return domainResult switch
            {
                DispenseResult.Success success => ExecuteDispense(success.Plan),
                DispenseResult.Failure failure => new DispenseUseCaseResult.Failure(failure.Reason),
                _ => throw new InvalidOperationException("未定義の型です")
            };
        }

        private DispenseUseCaseResult ExecuteDispense(DispensePlan plan)
        {
            _inventory.Dispense(plan);
            var primiteveDetails = plan.DispenseDetails.ToDictionary(kv => kv.Key.Value, kv => kv.Value);
            return new DispenseUseCaseResult.Success(primiteveDetails);
        }
    }
}
