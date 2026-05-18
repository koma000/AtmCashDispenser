using AtmCashDispenser.Domain.Dispensing;
using AtmCashDispenser.Domain.Shared;

namespace AtmCashDispenser.Domain.Tests
{
    public class CashInventoryTests
    {
        [Fact]
        public void GetCount_存在する金種の場合_正しい枚数が返ること()
        {
            // Arrange
            var initial = new Dictionary<Denomination, int>
            {
                { Denomination.TenThousand, 5 }
            };
            var inventory = new CashInventory(initial);

            // Act
            var countTenThousand = inventory.GetCount(Denomination.TenThousand);

            // Assert
            Assert.Equal(5, countTenThousand);
        }

        [Fact]
        public void GetCount_存在しない金種の場合_0が返ること()
        {
            // Arrange
            var initial = new Dictionary<Denomination, int>
            {
                { Denomination.TenThousand, 5 }
            };
            var inventory = new CashInventory(initial);

            // Act
            var countFiveThousand = inventory.GetCount(Denomination.FiveThousand);

            // Assert
            Assert.Equal(0, countFiveThousand);
        }

        [Fact]
        public void CalcDispense_在庫が十分な場合_在庫が減ること()
        {
            // Arrange
            var initial = new Dictionary<Denomination, int>
            {
                { Denomination.TenThousand, 5 },
                { Denomination.FiveThousand, 5 }
            };
            var inventory = new CashInventory(initial);
            var calculator = new CashDispenseCalculator();
            var amount = Money.Create(15000);

            // Act
            var result = inventory.CalcDispense(amount, calculator);

            // Assert
            var sucessResult = Assert.IsType<DispenseResult.Success>(result);
            Assert.Equal(1, sucessResult.Plan.DispenseDetails[Denomination.TenThousand]);
            Assert.Equal(1, sucessResult.Plan.DispenseDetails[Denomination.FiveThousand]);
        }

        [Fact]
        public void CalcDispense_在庫が不足している場合_失敗結果が返されること()
        {
            // Arrange
            var initial = new Dictionary<Denomination, int>
            {
                { Denomination.TenThousand, 1 }
            };
            var inventory = new CashInventory(initial);
            var calculator = new CashDispenseCalculator();
            var amount = Money.Create(20000);

            // Act
            var result = inventory.CalcDispense(amount, calculator);

            // Assert
            var failureResult = Assert.IsType<DispenseResult.Failure>(result);
            Assert.Equal(DispenseFailureReason.InsufficientCombination, failureResult.Reason);
        }

        [Fact]
        public void ApplyDispense_成功結果を適用した場合_在庫が減ること()
        {
            // Arrange
            var initial = new Dictionary<Denomination, int>
            {
                { Denomination.TenThousand, 5 },
                { Denomination.FiveThousand, 5 }
            };
            var inventory = new CashInventory(initial);
            var dispensePlan = new DispensePlan(new Dictionary<Denomination, int>
            {
                { Denomination.TenThousand, 1 },
                { Denomination.FiveThousand, 1 }
            });
            var result = new DispenseResult.Success(dispensePlan);

            // Act
            inventory.ApplyDispense(dispensePlan);

            // Assert
            Assert.Equal(4, inventory.GetCount(Denomination.TenThousand));
            Assert.Equal(4, inventory.GetCount(Denomination.FiveThousand));
        }

        [Fact]
        public void ToString_在庫の内容が正しく表示されること()
        {
            // Arrange
            var initial = new Dictionary<Denomination, int>
            {
                { Denomination.TenThousand, 5 },
                { Denomination.FiveThousand, 3 }
            };
            var inventory = new CashInventory(initial);

            // Act
            var inventoryString = inventory.ToString();

            // Assert
            Assert.Equal("10000円: 5枚, 5000円: 3枚", inventoryString);
        }
    }
}
