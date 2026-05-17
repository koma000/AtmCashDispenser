using AtmCashDispenser.Application.DispenseCash;
using AtmCashDispenser.Domain.Dispensing;
using AtmCashDispenser.Domain.Shared;

namespace AtmCashDispenser.Application.Tests
{
    public class DispenseCashUseCaseTests
    {
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
            var useCase = new DispenseCashUseCase(inventory);
            int amount = 16000;

            // Act
            var result = useCase.Execute(amount);

            // Assert: ① 戻り値(DTO) の検証
            var successResult = Assert.IsType<DispenseUseCaseResult.Success>(result);
            Assert.Equal(1, successResult.DispenseDetails[10000]);
            Assert.Equal(1, successResult.DispenseDetails[5000]);
            Assert.Equal(1, successResult.DispenseDetails[1000]);

            // Assert: ② 在庫の検証
            Assert.Equal(9, inventory.GetCount(Denomination.TenThousand));
            Assert.Equal(9, inventory.GetCount(Denomination.FiveThousand));
            Assert.Equal(9, inventory.GetCount(Denomination.Thousand));
        }

        [Fact]
        public void Execute_金額がマイナスの場合_ArgumentExceptionがスローされること()
        {
            // Arrange
            var inventory = CreateInitialInventory();
            var useCase = new DispenseCashUseCase(inventory);
            int amount = -5000;

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => useCase.Execute(amount));
            Assert.Contains("金額は0以上でなければなりません。", exception.Message);

            // 在庫は変更されないこと
            Assert.Equal(10, inventory.GetCount(Denomination.TenThousand));
            Assert.Equal(10, inventory.GetCount(Denomination.FiveThousand));
            Assert.Equal(10, inventory.GetCount(Denomination.Thousand));
        }

        [Fact]
        public void Execute_払い出し不可能な金額の場合_失敗結果が返されること()
        {
            // Arrange
            var inventory = CreateInitialInventory();
            var useCase = new DispenseCashUseCase(inventory);
            int amount = 12345;

            // Act
            var result = useCase.Execute(amount);

            // Assert
            var failureResult = Assert.IsType<DispenseUseCaseResult.Failure>(result);
            Assert.Equal("払い出し不可能な金額です", failureResult.Reason);

            // 在庫は変更されないこと
            Assert.Equal(10, inventory.GetCount(Denomination.TenThousand));
            Assert.Equal(10, inventory.GetCount(Denomination.FiveThousand));
            Assert.Equal(10, inventory.GetCount(Denomination.Thousand));
        }
    }
}
