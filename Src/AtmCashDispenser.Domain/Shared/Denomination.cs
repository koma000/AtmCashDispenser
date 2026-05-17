namespace AtmCashDispenser.Domain.Shared
{
    /// <summary>
    /// 日本円の金種（硬貨・紙幣）を表す値オブジェクト
    /// 定義された金種のみを許可する
    /// </summary>
    public sealed class Denomination : IEquatable<Denomination>
    {
        public int Value { get; }

        public static readonly Denomination One = new(1);
        public static readonly Denomination Five = new(5);
        public static readonly Denomination Ten = new(10);
        public static readonly Denomination Fifty = new(50);
        public static readonly Denomination Hundred = new(100);
        public static readonly Denomination FiveHundred = new(500);
        public static readonly Denomination Thousand = new(1000);
        public static readonly Denomination TwoThousand = new(2000);
        public static readonly Denomination FiveThousand = new(5000);
        public static readonly Denomination TenThousand = new(10000);

        /// <summary>
        /// 外部からのインスタンス化を防止する
        /// </summary>
        /// <param name="value">金種値</param>
        /// <exception cref="ArgumentException">金種値が正の整数でない場合にスローする</exception>
        private Denomination(int value)
        {
            if (value <= 0)
            {
                throw new ArgumentException("金種値は正の整数でなければなりません。", nameof(value));
            }
            Value = value;
        }

        /// <summary>
        /// 全金種（昇順）
        /// </summary>
        private static readonly IReadOnlyCollection<Denomination> _all = 
            new[] { One, Five, Ten, Fifty, Hundred, FiveHundred, Thousand, TwoThousand, FiveThousand, TenThousand }
            .OrderBy(d => d.Value).ToArray();

        public static IReadOnlyCollection<Denomination> All => _all;

        /// <summary>
        /// 全金種（降順）。これにより、金種を大きい順に処理する際に便利
        /// </summary>
        public static IEnumerable<Denomination> Descending => _all.Reverse();

        /// <summary>
        /// 金種値から対応する金種を取得するためのマップ
        /// </summary>
        private static readonly IReadOnlyDictionary<int, Denomination> _map = _all.ToDictionary(d => d.Value);

        /// <summary>
        /// 指定された金額から対応する <see cref="Denomination"/> を取得する
        /// </summary>
        /// <param name="value">金額（1, 5, 10...）</param>
        /// <returns>金種インスタンス</returns>
        /// <exception cref="ArgumentException">日本円として定義されていない数値の場合にスローする</exception>
        public static Denomination FromValue(int value)
        {
            if (!_map.TryGetValue(value, out var denomination))
            {
                throw new ArgumentException($"{value}円は、有効な金種ではありません。", nameof(value));
            }
            return denomination;
        }

        public bool IsBill => Value >= 1000;

        public bool IsCoin => !IsBill;

        public override bool Equals(object? obj) => obj is Denomination other && Value == other.Value;

        public override int GetHashCode() => Value.GetHashCode();

        public bool Equals(Denomination? other) => other is not null && Value == other.Value;

        public static bool operator ==(Denomination? left, Denomination? right)
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }
            if (left is null || right is null)
            {
                return false;
            }
            return left.Value == right.Value;
        }

        public static bool operator !=(Denomination? left, Denomination? right) => !(left == right);

        public override string ToString() => $"{Value}円";        
    }
}