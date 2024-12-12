namespace Moyba.AdventOfCode.Year2024
{
    public class Day11(string[] _data) : IPuzzle
    {
        private readonly long[] _stones = _data
            .Single()
            .Split(' ')
            .Select(Int64.Parse)
            .ToArray();

        private readonly IDictionary<long, IEnumerable<long>> _behavior = new Dictionary<long, IEnumerable<long>>();
        private readonly IDictionary<long, IDictionary<int, long>> _blink = new Dictionary<long, IDictionary<int, long>>();

        [PartOne("209412")]
        [PartTwo("248967696501656")]
        public async IAsyncEnumerable<string?> ComputeAsync()
        {
            var part1 = _stones.Sum(_ => this.Blink(_, 25));

            yield return $"{part1}";

            var part2 = _stones.Sum(_ => this.Blink(_, 75));

            yield return $"{part2}";

            await Task.CompletedTask;
        }

        private long Blink(long value, int repetitions)
        {
            if (!_blink.ContainsKey(value)) _blink.Add(value, new Dictionary<int, long> { { 0, 1 }});

            var lookup = _blink[value];
            return lookup.ContainsKey(repetitions)
                ? lookup[repetitions]
                : (lookup[repetitions] = this.GetBehavior(value).Sum(_ => this.Blink(_, repetitions - 1)));
        }

        private IEnumerable<long> GetBehavior(long value)
            => _behavior.ContainsKey(value) ? _behavior[value] : (_behavior[value] = _CalculateBehavior(value));

        private static IEnumerable<long> _CalculateBehavior(long value)
        {
            if (value == 0) return [ 1 ];

            var numberOfDigits = (int)Math.Ceiling(Math.Log10(value + 1));
            if (numberOfDigits % 2 == 1) return [ 2024 * value ];

            var halfOfTheDigits = (long)Math.Pow(10, numberOfDigits >> 1);
            return [ value / halfOfTheDigits, value % halfOfTheDigits ];
        }
    }
}
