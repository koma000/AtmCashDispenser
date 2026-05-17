using AtmCashDispenser.Domain.Shared;

namespace AtmCashDispenser.Domain.Tests
{
    public class MoneyTests
    {
        [Theory]
        [InlineData(0)]
        [InlineData(10000)]
        public void Create_金額が0以上の場合_正常にインスタンスが作成されること(int amount)
        {
            // Act
            var money = Money.Create(amount);

            // Assert
            Assert.Equal(amount, money.Amount);
        }

        [Fact]
        public void Create_金額がマイナスの場合_ArgumentExceptionがスローされること()
        {
            // Arrange
            int negativeAmount = -1;

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => Money.Create(negativeAmount));
            Assert.Contains("金額は0以上でなければなりません", ex.Message);
        }

        [Fact]
        public void Zero_呼び出した場合_金額が0のインスタンスが取得できること()
        {
            // Act
            var money = Money.Zero;

            // Assert
            Assert.Equal(0, money.Amount);
            Assert.True(money.IsZero());
        }

        [Fact]
        public void IsZero_金額が0かどうかを正しく判定できること()
        {
            // Arrange & Act
            var zeroMoney = Money.Create(0);
            var positiveMoney = Money.Create(1000);

            // Assert
            Assert.True(zeroMoney.IsZero());
            Assert.False(positiveMoney.IsZero());
        }

        [Fact]
        public void Subtract_残高が足りる減算の場合_引き算された新しいインスタンスが返ること()
        {
            // Arrange
            var source = Money.Create(5000);
            var target = Money.Create(2000);

            // Act (メソッドと-演算子の両方をテスト)
            var resultMethod = source.Subtract(target);
            var resultOperator = source - target;

            // Assert
            Assert.Equal(3000, resultMethod.Amount);
            Assert.Equal(3000, resultOperator.Amount);

            // 値オブジェクトなので、元のインスタンスが書き換わっていないことも念のため確認
            Assert.Equal(5000, source.Amount);
        }

        [Fact]
        public void Subtract_計算結果がマイナスになる場合_InvalidOperationExceptionがスローされること()
        {
            // Arrange
            var source = Money.Create(1000);
            var target = Money.Create(2000);

            // Act & Assert
            var ex =Assert.Throws<InvalidOperationException>(() => source.Subtract(target));
            Assert.Throws<InvalidOperationException>(() => source - target);

            Assert.Contains("減算後の金額は0以上でなければなりません。", ex.Message);
        }

        [Fact]
        public void Add_金額を加算した場合_足し算された新しいインスタンスが返ること()
        {
            // Arrange
            var a = Money.Create(1000);
            var b = Money.Create(2000);

            // Act
            var resultMethod = a.Add(b);
            var resultOperator = a + b;

            // Assert
            Assert.Equal(3000, resultMethod.Amount);
            Assert.Equal(3000, resultOperator.Amount);

            // 値オブジェクトなので、元のインスタンスが書き換わっていないことも念のため確認
            Assert.Equal(1000, a.Amount);
        }

        [Fact]
        public void 大小比較演算子_金額の大小を正しく判定できること()
        {
            // Arrange
            var small = Money.Create(1000);
            var large = Money.Create(2000);

            // Act & Assert
            Assert.True(large > small);
            Assert.False(small > large);

            Assert.True(small < large);
            Assert.False(large < small);
        }

        [Fact]
        public void ToString_呼び出した場合_円付きの文字列が返ること()
        {
            // Arrange
            var money = Money.Create(10000);

            // Act
            var result = money.ToString();

            // Assert
            Assert.Equal("10000円", result);
        }
    }
}
