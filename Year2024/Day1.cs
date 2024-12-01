namespace Moyba.AdventOfCode.Year2024
{
    public class Day1 : IPuzzle
    {
        private readonly long[] _left;
        private readonly long[] _right;

        public Day1(string[] data)
        {
            var split = data.Select(_ => _.Split(' ', StringSplitOptions.RemoveEmptyEntries));
            _left = split.Select(_ => Int64.Parse(_[0])).Order().ToArray();
            _right = split.Select(_ => Int64.Parse(_[1])).Order().ToArray();
        }

        [PartOne()]
        [PartTwo()]
        public async IAsyncEnumerable<string?> ComputeAsync()
        {
            var distance = Enumerable.Range(0, _left.Length).Select(_ => Int64.Abs(_left[_] - _right[_])).Sum();

            yield return $"{distance}";

            var groups = _right.GroupBy(_ => _).ToDictionary(_ => _.Key, _ => _.Count());
            var similarity = _left.Sum(_ => groups.ContainsKey(_) ? _ * groups[_] : 0);

            yield return $"{similarity}";

            await Task.CompletedTask;
        }
    }
}
