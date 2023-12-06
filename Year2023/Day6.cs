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

        private long _productOfOptions = 1;
        private long _longerOptions;

        public Task ComputeAsync()
        {
            foreach (var race in _races)
            {
                _productOfOptions *= _FindWinningOptions(race.time, race.distance);
            }

            var time = Int64.Parse(String.Join("", _races.Select(_ => _.time)));
            var distance = Int64.Parse(String.Join("", _races.Select(_ => _.distance)));
            _longerOptions = _FindWinningOptions(time, distance);

            return Task.CompletedTask;
        }

        [Solution("393120")]
        public string PartOne => $"{_productOfOptions}";

        [Solution()]
        public string PartTwo => $"{_longerOptions}";

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
