namespace Moyba.AdventOfCode.Year2023
{
    public class Day6(string[] _data) : IPuzzle
    {
        private readonly (long time, long distance)[] _races = _data[0]
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Skip(1)
            .Select(Int64.Parse)
            .Zip(_data[1]
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Skip(1)
                .Select(Int64.Parse))
            .ToArray();

        [PartOne("393120")]
        [PartTwo("36872656")]
        public async IAsyncEnumerable<string> ComputeAsync()
        {
            var productOfOptions = 1L;
            foreach (var race in _races)
            {
                productOfOptions *= _FindWinningOptions(race.time, race.distance);
            }

            yield return $"{productOfOptions}";

            var time = Int64.Parse(String.Join("", _races.Select(_ => _.time)));
            var distance = Int64.Parse(String.Join("", _races.Select(_ => _.distance)));
            yield return $"{_FindWinningOptions(time, distance)}";

            await Task.CompletedTask;
        }

        private static long _FindWinningOptions(long time, long distance)
        {
            // lower bound
            //  x * (t - x) > d
            //  x > t/2 - sqrt(t^2/4 - d)
            // upper bound
            //  x' = t - x
            // options = x' - x + 1

            var halfTime = time / 2.0;
            var lowerBound = (long)Math.Ceiling(halfTime - Math.Sqrt(halfTime * halfTime - distance));
            return time + 1 - 2 * lowerBound;
        }
    }
}
