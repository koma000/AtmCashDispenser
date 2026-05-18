using AtmCashDispenser.Domain.Dispensing;
using AtmCashDispenser.Domain.Shared;

namespace AtmCashDispenser.Application.DispenseCash
{
    public class DispenseCashUseCase
    {
        private readonly CashInventory _inventory;
        private readonly ICashDispenseCalculator _calculator;

        private const int MAX_DISPENSE_AMOUNT = 100000;

        public DispenseCashUseCase(CashInventory inventory, ICashDispenseCalculator calculator)
        {
            _inventory = inventory;
            _calculator = calculator;
        }

        public DispenseUseCaseResult Execute(int amount)
        {
            if (amount <= 0)
            {
                return new DispenseUseCaseResult.Failure(UseCaseError.InvalidAmount);
            }
            if (amount > MAX_DISPENSE_AMOUNT)
            {
                return new DispenseUseCaseResult.Failure(UseCaseError.LimitExceeded);
            }

            // 入力をドメインに変換
            var money = Money.Create(amount);

            var domainResult = _inventory.CalcDispense(money, _calculator);

            return domainResult switch
            {
                DispenseResult.Success success => HandleSuccess(success),
                DispenseResult.Failure => 
                    new DispenseUseCaseResult.Failure(UseCaseError.NotDispensable),
                _ => throw new InvalidOperationException("未定義の型です")
            };
        }

        private DispenseUseCaseResult HandleSuccess(DispenseResult.Success success)
        {
            _inventory.ApplyDispense(success.Plan);

            return new DispenseUseCaseResult.Success(
                TransactionId.New(),
                success.Plan.DispenseDetails
                .Select(x => new DispenseItemDto(x.Key.Value, x.Value))
                .ToList());
        }
    }
}
