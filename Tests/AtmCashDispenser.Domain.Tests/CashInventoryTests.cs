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
        public void Dispense_在庫が十分な場合_在庫が減ること()
        {
            // Arrange
            var initial = new Dictionary<Denomination, int>
            {
                { Denomination.TenThousand, 5 },
                { Denomination.FiveThousand, 5 }
            };
            var inventory = new CashInventory(initial);
            var plan = new DispensePlan(new Dictionary<Denomination, int>
            {
                { Denomination.TenThousand, 2 },
                { Denomination.FiveThousand, 1 }
            });

            // Act
            inventory.Dispense(plan);

            // Assert
            Assert.Equal(3, inventory.GetCount(Denomination.TenThousand));
            Assert.Equal(4, inventory.GetCount(Denomination.FiveThousand));
        }

        [Fact]
        public void Dispense_在庫が不足している場合_InvalidOperationExceptionがスローされること()
        {
            // Arrange
            var initial = new Dictionary<Denomination, int>
            {
                { Denomination.TenThousand, 1 }
            };
            var inventory = new CashInventory(initial);
            var plan = new DispensePlan(new Dictionary<Denomination, int>
            {
                { Denomination.TenThousand, 2 }
            });

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => inventory.Dispense(plan));
            Assert.Contains("在庫不足", ex.Message);

            // 在庫は変更されないことを確認
            Assert.Equal(1, inventory.GetCount(Denomination.TenThousand));
        }

        [Fact]
        public void Dispense_システムに存在しない金種を含む場合_InvalidOperationExceptionがスローされること()
        {
            // Arrange
            var initial = new Dictionary<Denomination, int>
            {
                { Denomination.TenThousand, 5 }
            };
            var inventory = new CashInventory(initial);
            var plan = new DispensePlan(new Dictionary<Denomination, int>
            {
                { Denomination.TwoThousand, 1 }
            });
            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => inventory.Dispense(plan));
            Assert.Contains("システムに存在しない金種", ex.Message);
            // 在庫は変更されないことを確認
            Assert.Equal(5, inventory.GetCount(Denomination.TenThousand));
        }
    }
}
