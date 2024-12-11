namespace Moyba.AdventOfCode.Year2024
{
    using Coordinate = (int x, int y);
    using Topography = (int x, int y, int height);

    public class Day10(string[] _data) : IPuzzle
    {
        private static readonly HashSet<Coordinate> _Orthogonals = [
            (  1,  0 ),
            ( -1,  0 ),
            (  0,  1 ),
            (  0, -1 ),
        ];

        private readonly int[][] _map = _data
            .Select(line => line.Select(_ => _ - '0').ToArray())
            .ToArray();

        [PartOne("798")]
        [PartTwo("1816")]
        public async IAsyncEnumerable<string?> ComputeAsync()
        {
            Dictionary<Topography, HashSet<Topography>> trails = _map
                .SelectMany((row, y) => row.Select((_, x) => (x, y, _)))
                .ToDictionary(_ => _, _ => new HashSet<Topography>());

            var byHeight = trails.Keys.GroupBy(_ => _.height).ToDictionary(_ => _.Key);
            foreach (var peak in byHeight[9])
            {
                trails[peak].Add(peak);
            }

            for (var height = 8; height >= 0; height--)
            {
                var atElevation = byHeight[height];
                foreach (var location in atElevation)
                {
                    foreach (var orthogonal in _Orthogonals)
                    {
                        Topography adjacent = (location.x + orthogonal.x, location.y + orthogonal.y, height + 1);
                        if (trails.ContainsKey(adjacent))
                        {
                            foreach (var peak in trails[adjacent]) trails[location].Add(peak);
                        }
                    }
                }
            }

            var part1 = byHeight[0].Sum(_ => trails[_].Count);

            yield return $"{part1}";

            Dictionary<Topography, int> ratings = _map
                .SelectMany((row, y) => row.Select((_, x) => (x, y, _)))
                .ToDictionary(_ => _, _ => 0);

            foreach (var peak in byHeight[9])
            {
                ratings[peak] = 1;
            }

            for (var height = 8; height >= 0; height--)
            {
                var atElevation = byHeight[height];
                foreach (var location in atElevation)
                {
                    foreach (var orthogonal in _Orthogonals)
                    {
                        Topography adjacent = (location.x + orthogonal.x, location.y + orthogonal.y, height + 1);
                        if (ratings.ContainsKey(adjacent)) ratings[location] += ratings[adjacent];
                    }
                }
            }

            var part2 = byHeight[0].Sum(_ => ratings[_]);

            yield return $"{part2}";

            await Task.CompletedTask;
        }
    }
}
