using AtmCashDispenser.Domain.Dispensing;
using AtmCashDispenser.Domain.Shared;

namespace AtmCashDispenser.Domain.Tests
{
    public class CashDispenseCalculatorTests
    {
        /// <summary>
        /// テストごとにクリーンな十分な在庫を生成するヘルパーメソッド
        /// </summary>
        private IReadOnlyDictionary<Denomination, int> SetupSufficientInventory()
        {
            var initial = new Dictionary<Denomination, int>
            {
                { Denomination.TenThousand, 10 },  // 10万円分
                { Denomination.FiveThousand, 10 }, // 5万円分
                { Denomination.Thousand, 10 },     // 1万円分
                { Denomination.FiveHundred, 10 },  // 5000円分
                { Denomination.Hundred, 10 }       // 1000円分
            };

            return initial;
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
            var total = successResult.Plan.DispenseDetails.Sum(kvp => kvp.Key.Value * kvp.Value);
            Assert.Equal(amount.Amount, total);
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
            var calculator = new CashDispenseCalculator();
            var amount = Money.Create(12000);

            // Act
            var result = calculator.Calculate(initial, amount);

            // Assert
            var successResult = Assert.IsType<DispenseResult.Success>(result);
            Assert.False(successResult.Plan.DispenseDetails.ContainsKey(Denomination.TenThousand));
            Assert.Equal(2, successResult.Plan.DispenseDetails[Denomination.FiveThousand]);
            Assert.Equal(2, successResult.Plan.DispenseDetails[Denomination.Thousand]);
        }

        [Fact]
        public void Calculate_上位の金種で払いきれない場合_下位の金種を組み合わせてプランが作成されること()
        {
            // Arrange
            var initial = new Dictionary<Denomination, int>
            {
                { Denomination.FiveThousand, 1 },
                { Denomination.TwoThousand, 3 },
            };
            var calculator = new CashDispenseCalculator();
            var amount = Money.Create(6000);

            // Act
            var result = calculator.Calculate(initial, amount);

            // Assert
            var successResult = Assert.IsType<DispenseResult.Success>(result);
            Assert.False(successResult.Plan.DispenseDetails.ContainsKey(Denomination.FiveThousand));
            Assert.Equal(3, successResult.Plan.DispenseDetails[Denomination.TwoThousand]);
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
            Assert.Equal(DispenseFailureReason.InsufficientCombination, failureResult.Reason);
        }

        [Fact]
        public void Calculate_在庫の辞書データに特定の金種のキー自体が含まれていない場合_その金種は0枚として扱われること()
        {
            // Arrange
            // 全ての金種のキーが欠落している、空の在庫スナップショット
            var emptyInventory = new Dictionary<Denomination, int>();
            var calculator = new CashDispenseCalculator();
            var amount = Money.Create(2000);

            // Act
            var result = calculator.Calculate(emptyInventory, amount);

            // Assert
            // KeyNotFoundException でクラッシュせず、ビジネスロジック上のエラーとしてハンドリングされていること
            var failureResult = Assert.IsType<DispenseResult.Failure>(result);
            Assert.Equal(DispenseFailureReason.InsufficientCombination, failureResult.Reason);
        }
    }
}
