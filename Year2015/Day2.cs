using Present = (long maxDimension, long minArea, long minPerimeter);

namespace Moyba.AdventOfCode.Year2015
{
    public class Day2(string[] data) : IPuzzle
    {
        private readonly Present[] _presents = data
            .Select(_ => _.Split('x').Select(Int64.Parse).Order().ToArray())
            .Select(_ => (_[2], _[0] * _[1], _[0] + _[1]))
            .ToArray();

        [PartOne("1598415")]
        [PartTwo("3812909")]
        public async IAsyncEnumerable<string> ComputeAsync()
        {
            var paper = 0L;
            var ribbon = 0L;

            foreach ((var maxDimension, var minArea, var minPerimeter) in _presents)
            {
                paper += 3 * minArea + 2 * minPerimeter * maxDimension;
                ribbon += 2 * minPerimeter + minArea * maxDimension;
            }

            yield return $"{paper}";

            yield return $"{ribbon}";

            await Task.CompletedTask;
        }
    }
}
