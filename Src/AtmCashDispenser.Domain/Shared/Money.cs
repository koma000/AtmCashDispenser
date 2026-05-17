namespace AtmCashDispenser.Domain.Shared
{
    /// <summary>
    /// 金額を表す値オブジェクト
    /// </summary>
    public sealed record Money
    {
        public int Amount { get; }

        private Money(int amount)
        {
            Amount = amount;
        }

        public static Money Create(int amount)
        {
            if (amount < 0)
            {
                throw new ArgumentException("金額は0以上でなければなりません。", nameof(amount));
            }
            return new Money(amount);
        }

        public static Money Zero => new(0);

        public bool IsZero() => Amount == 0;

        public Money Add(Money other) => new(Amount + other.Amount);

        public Money Subtract(Money other)
        {
            if (Amount < other.Amount)
            {
                throw new InvalidOperationException("減算後の金額は0以上でなければなりません。");
            }
            return new(Amount - other.Amount);
        }

        public bool IsGreaterThan(Money other) => Amount > other.Amount;

        public static Money operator +(Money a, Money b) => a.Add(b);
        public static Money operator -(Money a, Money b) => a.Subtract(b);
        public static bool operator >(Money a, Money b) => a.IsGreaterThan(b);
        public static bool operator <(Money a, Money b) => b.IsGreaterThan(a);

        public override string ToString() => $"{Amount}円";
    }
}
