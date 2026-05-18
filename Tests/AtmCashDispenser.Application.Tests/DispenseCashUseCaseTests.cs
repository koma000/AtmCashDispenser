using AtmCashDispenser.Application.DispenseCash;
using AtmCashDispenser.Domain.Dispensing;
using AtmCashDispenser.Domain.Shared;

namespace AtmCashDispenser.Application.Tests
{
    public class DispenseCashUseCaseTests
    {
        private class FakeCashDispenseCalculator : ICashDispenseCalculator
        {
            // テストケースごとに、期待する戻り値を外から自由にセットできるようにする
            public DispenseResult ReturnsResult { get; set; } = new DispenseResult.Failure(DispenseFailureReason.InsufficientCombination);

            public DispenseResult Calculate(IReadOnlyDictionary<Denomination, int> inventorySnapShot, Money amount)
            {
                return ReturnsResult;
            }
        }

        /// <summary>
        /// クリーンな初期在庫を作成するためのヘルパーメソッド
        /// </summary>
        /// <returns>初期在庫を持つCashInventoryオブジェクト</returns>
        private CashInventory CreateInitialInventory()
        {
            var initial = new Dictionary<Denomination, int>
            {
                { Denomination.TenThousand, 10 },
                { Denomination.FiveThousand, 10 },
                { Denomination.Thousand, 10 }
            };
            return new CashInventory(initial);
        }

        [Fact]
        public void Execute_在庫が十分な場合_払い出しプランが計算され在庫が実際に減算されること()
        {
            // Arrange
            var inventory = CreateInitialInventory();
            var fakeCalculator = new FakeCashDispenseCalculator();
            var expectedPlan = new Dictionary<Denomination, int>
            {
                { Denomination.TenThousand, 1 },
                { Denomination.FiveThousand, 1 },
                { Denomination.Thousand, 1 }
            };
            fakeCalculator.ReturnsResult = new DispenseResult.Success(new DispensePlan(expectedPlan));
            var useCase = new DispenseCashUseCase(inventory, fakeCalculator);
            int amount = 16000;

            // Act
            var result = useCase.Execute(amount);

            // Assert: ① 戻り値 の検証
            var successResult = Assert.IsType<DispenseUseCaseResult.Success>(result);
            Assert.NotNull(successResult.TransactionId);
            var actual = successResult.DispenseDetails
                .OrderByDescending(x => x.Denomination)
                .Select(x => (x.Denomination, x.Count));

            var expected = new[]
            {
                (10000, 1),
                (5000, 1),
                (1000, 1)
            };
            Assert.Equal(expected, actual);

            // Assert: ② 在庫の検証
            Assert.Equal(9, inventory.GetCount(Denomination.TenThousand));
            Assert.Equal(9, inventory.GetCount(Denomination.FiveThousand));
            Assert.Equal(9, inventory.GetCount(Denomination.Thousand));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Execute_金額が0以下の場合_失敗結果InvalidAmountが返されること(int amount)
        {
            // Arrange
            var inventory = CreateInitialInventory();
            var fakeCalculator = new FakeCashDispenseCalculator();
            fakeCalculator.ReturnsResult = new DispenseResult.Failure(DispenseFailureReason.InsufficientCombination);
            var useCase = new DispenseCashUseCase(inventory, fakeCalculator);

            // Act
            var result = useCase.Execute(amount);

            // Assert
            var failureResult = Assert.IsType<DispenseUseCaseResult.Failure>(result);
            Assert.Equal(UseCaseError.InvalidAmount, failureResult.Error);

            // 在庫は変更されないこと
            Assert.Equal(10, inventory.GetCount(Denomination.TenThousand));
            Assert.Equal(10, inventory.GetCount(Denomination.FiveThousand));
            Assert.Equal(10, inventory.GetCount(Denomination.Thousand));
        }

        [Fact]
        public void Execute_金額が10万より大きい場合_失敗結果LimitExceededが返されること()
        {
            // Arrange
            var inventory = CreateInitialInventory();
            var fakeCalculator = new FakeCashDispenseCalculator();
            fakeCalculator.ReturnsResult = new DispenseResult.Failure(DispenseFailureReason.InsufficientCombination);
            var useCase = new DispenseCashUseCase(inventory, fakeCalculator);
            int amount = 100001;

            // Act
            var result = useCase.Execute(amount);
            
            // Assert
            var failureResult = Assert.IsType<DispenseUseCaseResult.Failure>(result);
            Assert.Equal(UseCaseError.LimitExceeded, failureResult.Error);
            
            // 在庫は変更されないこと
            Assert.Equal(10, inventory.GetCount(Denomination.TenThousand));
            Assert.Equal(10, inventory.GetCount(Denomination.FiveThousand));
            Assert.Equal(10, inventory.GetCount(Denomination.Thousand));
        }

        [Fact]
        public void Execute_払い出し不可能な金額の場合_失敗結果NotDispensableが返されること()
        {
            // Arrange
            var inventory = CreateInitialInventory();
            var fakeCalculator = new FakeCashDispenseCalculator();
            fakeCalculator.ReturnsResult = new DispenseResult.Failure(DispenseFailureReason.InsufficientCombination);
            var useCase = new DispenseCashUseCase(inventory, fakeCalculator);
            int amount = 12345;

            // Act
            var result = useCase.Execute(amount);

            // Assert
            var failureResult = Assert.IsType<DispenseUseCaseResult.Failure>(result);
            Assert.Equal(UseCaseError.NotDispensable, failureResult.Error);

            // 在庫は変更されないこと
            Assert.Equal(10, inventory.GetCount(Denomination.TenThousand));
            Assert.Equal(10, inventory.GetCount(Denomination.FiveThousand));
            Assert.Equal(10, inventory.GetCount(Denomination.Thousand));
        }
    }
}
