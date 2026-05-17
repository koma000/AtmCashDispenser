using AtmCashDispenser.Domain.Shared;

namespace AtmCashDispenser.Domain.Tests
{
    public class DenominationTests
    {
        [Theory]
        [InlineData(1)]
        [InlineData(100)]
        [InlineData(1000)]
        public void FromValue_定義された金種の値を渡した場合_対応するインスタンスが返ること(int value)
        {
            // Act
            var denomination = Denomination.FromValue(value);

            // Assert
            Assert.Equal(value, denomination.Value);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(150)]
        [InlineData(-500)]
        public void FromValue_定義されていない金種の値を渡した場合_ArgumentExceptionがスローされること(int value)
        {
            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => Denomination.FromValue(value));
            Assert.Contains("有効な金種ではありません", ex.Message);
        }

        [Theory]
        [InlineData(1000)]
        [InlineData(2000)]
        [InlineData(5000)]
        [InlineData(10000)]
        public void IsBill_IsCoin_1000円以上の場合_紙幣と判定され硬貨ではないこと(int value)
        {
            // Arrange
            var denomination = Denomination.FromValue(value);

            // Act
            var isBill = denomination.IsBill;
            var isCoin = denomination.IsCoin;

            // Assert
            Assert.True(isBill);
            Assert.False(isCoin);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(5)]
        [InlineData(10)]
        [InlineData(50)]
        [InlineData(100)]
        [InlineData(500)]
        public void IsBill_IsCoin_1000円未満の場合_硬貨と判定され紙幣ではないこと(int value)
        {
            // Arrange
            var denomination = Denomination.FromValue(value);

            // Act
            var isBill = denomination.IsBill;
            var isCoin = denomination.IsCoin;

            // Assert
            Assert.False(isBill);
            Assert.True(isCoin);
        }

        [Fact]
        public void All_呼び出した場合_全10金種が昇順で取得できること()
        {
            // Act
            var all = Denomination.All;

            // Assert
            Assert.Equal(10, all.Count);
            Assert.Equal(new[] { 1, 5, 10, 50, 100, 500, 1000, 2000, 5000, 10000 }, all.Select(d => d.Value));
        }

        [Fact]
        public void Descending_呼び出した場合_全10金種が降順で取得できること()
        {
            // Act
            var descending = Denomination.Descending;

            // Assert
            Assert.Equal(10, descending.Count());
            Assert.Equal(new[] { 10000, 5000, 2000, 1000, 500, 100, 50, 10, 5, 1 }, descending.Select(d => d.Value));
        }

        [Fact]
        public void Equals_および比較演算子_同じ金種であれば同一と判定されること()
        {
            // Arrange
            var denom1 = Denomination.FromValue(100);
            var denom2 = Denomination.FromValue(100);
            var differentDenom = Denomination.FromValue(500);

            // Act & Assert
            Assert.True(denom1.Equals(denom2));
            Assert.True(denom1 == denom2);
            Assert.False(denom1 != denom2);
            Assert.False(denom1 == differentDenom);
            Assert.True(denom1 != differentDenom);
            Assert.Equal(denom1.GetHashCode(), denom2.GetHashCode());
        }

        [Fact]
        public void ToString_呼び出した場合_金種値を表す文字列が返ること()
        {
            // Arrange
            var denomination = Denomination.Thousand;

            // Act
            var str = denomination.ToString();

            // Assert
            Assert.Equal("1000円", str);
        }
    }
}