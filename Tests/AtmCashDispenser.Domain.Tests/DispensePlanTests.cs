using AtmCashDispenser.Domain.Dispensing;
using AtmCashDispenser.Domain.Shared;

namespace AtmCashDispenser.Domain.Tests
{
    public class DispensePlanTests
    {
        [Fact]
        public void Constructor_全ての枚数が1以上の場合_正常に作成されること()
        {
            // Arrange
            var details = new Dictionary<Denomination, int>
            {
                { Denomination.TenThousand, 1 },
                { Denomination.FiveThousand, 2 }
            };

            // Act
            var plan = new DispensePlan(details);

            // Assert
            Assert.Equal(1, plan.DispenseDetails[Denomination.TenThousand]);
            Assert.Equal(2, plan.DispenseDetails[Denomination.FiveThousand]);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Constructor_枚数が0以下の場合_ArgumentExceptionがスローされること(int count)
        {
            // Arrange
            var details = new Dictionary<Denomination, int>
            {
                { Denomination.TenThousand, count }
            };

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => new DispensePlan(details));
            Assert.Contains("払い出し枚数は1以上でなければなりません。", ex.Message);
        }

        [Fact]
        public void ToString_呼び出した場合_金種と枚数がカンマ区切りの文字列で返ること()
        {
            // Arrange
            var details = new Dictionary<Denomination, int>
            {
                { Denomination.TenThousand, 1 },
                { Denomination.FiveThousand, 2 }
            };
            var plan = new DispensePlan(details);

            // Act
            var result = plan.ToString();
            
            // Assert
            Assert.Equal("10000円 x 1枚, 5000円 x 2枚", result);
        }
    }
}
