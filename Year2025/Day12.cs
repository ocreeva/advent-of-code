using Moyba.AdventOfCode.Utility;

namespace Moyba.AdventOfCode.Year2025
{
    using Shape = Coordinate[];
    using Region = (int width, int height, int[] Count);

    public class Day12 : IPuzzle
    {
        private readonly Shape[] _shapes;
        private readonly Region[] _regions;

        public Day12(string[] data)
        {
            var clusters = data.Cluster().ToArray();

            _shapes = clusters[..^1]
                .Select(_ => _.Skip(1).SelectMany((row, y) => row.Select((_, x) => (_, new Coordinate(x, y))).Where(_ => _.Item1 == '#').Select(_ => _.Item2)).ToArray())
                .ToArray();

            _regions = clusters[^1]
                .Select(_ => _.Split(' ', 'x', ':').Where(_ => !String.IsNullOrEmpty(_)).Select(Int32.Parse).ToArray())
                .Select(_ => (_[0], _[1], _[2..]))
                .ToArray();
        }

        [PartOne("497")]
        public async IAsyncEnumerable<string?> ComputeAsync()
        {
            var fitPrecise = 0;

            foreach (var region in _regions)
            {
                var size = region.width * region.height;
                var spacePrecise = 0;
                for (var index = 0; index < _shapes.Length; index++)
                {
                    spacePrecise += region.Count[index] * _shapes[index].Length;
                }

                if (spacePrecise <= size) fitPrecise++;
            }

            yield return $"{fitPrecise}";

            await Task.CompletedTask;
        }
    }
}
