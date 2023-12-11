namespace Moyba.AdventOfCode.Year2023
{
    using GalaxyCoord = (int x, int y, int xGap, int yGap);

    public class Day11(string[] _data) : IPuzzle
    {
        private readonly GalaxyCoord[] _galaxies = Enumerable.Range(0, _data.Length)
            .SelectMany(y => Enumerable.Range(0, _data[0].Length)
                .Where(x => _data[y][x] == '#')
                .Select<int, GalaxyCoord>(x => (x, y, 0, 0)))
            .ToArray();

        [PartOne("9684228")]
        [PartTwo("483844716556")]
        public async IAsyncEnumerable<string?> ComputeAsync()
        {
            var columnsWithGalaxies = _galaxies.Select(_ => _.x).ToHashSet();
            var rowsWithGalaxies = _galaxies.Select(_ => _.y).ToHashSet();

            for (var y = _data.Length - 2; y > 0; y--)
            {
                if (rowsWithGalaxies.Contains(y)) continue;
                for (var index = 0; index < _galaxies.Length; index++)
                {
                    if (_galaxies[index].y < y) continue;
                    _galaxies[index].yGap++;
                }
            }

            for (var x = _data[0].Length - 2; x > 0; x--)
            {
                if (columnsWithGalaxies.Contains(x)) continue;
                for (var index = 0; index < _galaxies.Length; index++)
                {
                    if (_galaxies[index].x < x) continue;
                    _galaxies[index].xGap++;
                }
            }

            yield return null;

            yield return $"{_SumDistancesBetweenGalaxies(1)}";

            yield return $"{_SumDistancesBetweenGalaxies(999_999)}";

            await Task.CompletedTask;
        }

        private long _SumDistancesBetweenGalaxies(int scale)
        {
            var sumOfDistances = 0L;
            for (var index1 = 0; index1 < _galaxies.Length; index1++)
            {
                var galaxy1 = _galaxies[index1];
                var x1 = galaxy1.x + scale * galaxy1.xGap;
                var y1 = galaxy1.y + scale * galaxy1.yGap;
                for (var index2 = index1 + 1; index2 < _galaxies.Length; index2++)
                {
                    var galaxy2 = _galaxies[index2];
                    var x2 = galaxy2.x + scale * galaxy2.xGap;
                    var y2 = galaxy2.y + scale * galaxy2.yGap;
                    sumOfDistances += Math.Abs(y2 - y1) + Math.Abs(x2 - x1);
                }
            }

            return sumOfDistances;
        }
    }
}
