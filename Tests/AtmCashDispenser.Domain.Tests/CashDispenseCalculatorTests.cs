using AtmCashDispenser.Domain.Dispensing;
using AtmCashDispenser.Domain.Shared;

namespace AtmCashDispenser.Domain.Tests
{
    public class CashDispenseCalculatorTests
    {
        /// <summary>
        /// テストごとにクリーンな十分な在庫を生成するヘルパーメソッド
        /// </summary>
        private CashInventory SetupSufficientInventory()
        {
            var initial = new Dictionary<Denomination, int>
            {
                { Denomination.TenThousand, 10 },  // 10万円分
                { Denomination.FiveThousand, 10 }, // 5万円分
                { Denomination.Thousand, 10 },     // 1万円分
                { Denomination.FiveHundred, 10 },  // 5000円分
                { Denomination.Hundred, 10 }       // 1000円分
            };

            var inventory = new CashInventory(initial);
            return inventory;
        }

        [Fact]
        public void Calculate_在庫が十分な場合_最も枚数が少なくなる最適な組み合わせでプランが作成されること()
        {
            // Arrange
            var inventory = SetupSufficientInventory();
            var calculator = new CashDispenseCalculator();
            var amount = Money.Create(16600);

            // Act
            var result = calculator.Calculate(inventory, amount);

            // Assert
            var successResult = Assert.IsType<DispenseResult.Success>(result);
            Assert.Equal(1, successResult.Plan.DispenseDetails[Denomination.TenThousand]);
            Assert.Equal(1, successResult.Plan.DispenseDetails[Denomination.FiveThousand]);
            Assert.Equal(1, successResult.Plan.DispenseDetails[Denomination.Thousand]);
            Assert.Equal(1, successResult.Plan.DispenseDetails[Denomination.FiveHundred]);
            Assert.Equal(1, successResult.Plan.DispenseDetails[Denomination.Hundred]);
        }

        [Fact]
        public void Calculate_高額紙幣の在庫が切れている場合_存在する下位の金種を組み合わせてプランが作成されること()
        {
            // Arrange
            var initial = new Dictionary<Denomination, int>
            {
                { Denomination.TenThousand, 0 },
                { Denomination.FiveThousand, 10 },
                { Denomination.Thousand, 10 },
            };
            var inventory = new CashInventory(initial);
            var calculator = new CashDispenseCalculator();
            var amount = Money.Create(12000);

            // Act
            var result = calculator.Calculate(inventory, amount);

            // Assert
            var successResult = Assert.IsType<DispenseResult.Success>(result);
            Assert.False(successResult.Plan.DispenseDetails.ContainsKey(Denomination.TenThousand));
            Assert.Equal(2, successResult.Plan.DispenseDetails[Denomination.FiveThousand]);
            Assert.Equal(2, successResult.Plan.DispenseDetails[Denomination.Thousand]);
        }

        [Fact]
        public void Calculate_払い出し不可能な金額の場合_失敗結果が返されること()
        {
            // Arrange
            var inventory = SetupSufficientInventory();
            var calculator = new CashDispenseCalculator();
            var amount = Money.Create(12345);

            // Act
            var result = calculator.Calculate(inventory, amount);

            // Assert
            var failureResult = Assert.IsType<DispenseResult.Failure>(result);
            Assert.Contains("払い出し不可能な金額です", failureResult.Reason);
        }
    }
}
