using Present = (long maxDimension, long minArea, long minPerimeter);

namespace Moyba.AdventOfCode.Year2015
{
    public class Day2(IEnumerable<string> data) : IPuzzle
    {
        private readonly Present[] _presents = data
            .Select(_ => _.Split('x').Select(Int64.Parse).Order().ToArray())
            .Select(_ => (_[2], _[0] * _[1], _[0] + _[1]))
            .ToArray();

        private long _paper;
        private long _ribbon;

        public Task ComputeAsync()
        {
            foreach ((var maxDimension, var minArea, var minPerimeter) in _presents)
            {
                _paper += 3 * minArea + 2 * minPerimeter * maxDimension;
                _ribbon += 2 * minPerimeter + minArea * maxDimension;
            }

            return Task.CompletedTask;
        }

        [Solution("1598415")]
        public string SolvePartOne() => $"{_paper}";

        [Solution("3812909")]
        public string SolvePartTwo() => $"{_ribbon}";
    }
}
