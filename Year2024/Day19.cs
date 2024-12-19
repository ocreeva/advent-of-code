namespace Moyba.AdventOfCode.Year2024
{
    public class Day19(string[] _data) : IPuzzle
    {
        private readonly string[] _towels = _data[0].Split(", ");
        private readonly string[] _designs = _data[2..];

        private readonly IDictionary<string, long> _combinations = new Dictionary<string, long> { { "", 1 } };

        [PartOne("206")]
        [PartTwo("622121814629343")]
        public async IAsyncEnumerable<string?> ComputeAsync()
        {
            var part1 = _designs.Count(_ => this.CountCombinations(_) > 0);

            yield return $"{part1}";

            var part2 = _designs.Sum(_ => this.CountCombinations(_));

            yield return $"{part2}";

            await Task.CompletedTask;
        }

        private long CountCombinations(string pattern)
        {
            if (_combinations.ContainsKey(pattern))
            {
                return _combinations[pattern];
            }

            var count = 0L;
            foreach (var towel in _towels)
            {
                if (!pattern.StartsWith(towel)) continue;

                count += this.CountCombinations(pattern[towel.Length..]);
            }

            _combinations[pattern] = count;

            return count;
        }
    }
}
