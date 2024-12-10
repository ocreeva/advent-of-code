namespace Moyba.AdventOfCode.Year2024
{
    using Antenna = (char value, (int x, int y) coordinate);
    using Coordinate = (int x, int y);
    using Antinode = ((int x, int y) coordinate, (int x, int y) offset);

    public class Day8 : IPuzzle
    {
        private readonly IGrouping<char, Antenna>[] _antennas;
        private readonly HashSet<Antinode> _antinodes = new HashSet<Antinode>();

        private readonly int _height, _width;

        public Day8(string[] data)
        {
            _antennas = data
                .SelectMany((line, y) => line.Select((value, x) => (value, (x, y))))
                .Where(_ => _.Item1 != '.')
                .GroupBy(_ => _.Item1)
                .ToArray();

            foreach (var grouping in _antennas)
            {
                var coordinate = grouping.Select(_ => _.coordinate).ToArray();
                for (var a = 0; a < coordinate.Length - 1; a++)
                {
                    var cA = coordinate[a];
                    for (var b = a + 1; b < coordinate.Length; b++)
                    {
                        var cB = coordinate[b];
                        _antinodes.Add((cA, _Subtract(cA, cB)));
                        _antinodes.Add((cB, _Subtract(cB, cA)));
                    }
                }
            }

            _height = data.Length;
            _width = data[0].Length;
        }

        [PartOne("318")]
        [PartTwo("1126")]
        public async IAsyncEnumerable<string?> ComputeAsync()
        {
            var coordinates = new HashSet<Coordinate>();
            foreach (var antinode in _antinodes)
            {
                var coordinate = _Add(antinode.coordinate, antinode.offset);
                if (this.IsInBounds(coordinate)) coordinates.Add(coordinate);
            }

            yield return $"{coordinates.Count}";

            foreach (var antinode in _antinodes)
            {
                coordinates.Add(antinode.coordinate);

                var coordinate = _Add(_Add(antinode.coordinate, antinode.offset), antinode.offset);
                while (this.IsInBounds(coordinate))
                {
                    coordinates.Add(coordinate);
                    coordinate = _Add(coordinate, antinode.offset);
                }
            }

            yield return $"{coordinates.Count}";

            await Task.CompletedTask;
        }

        private bool IsInBounds(Coordinate c)
        {
            return c.x >= 0
                && c.x < _width
                && c.y >= 0
                && c.y < _height;
        }

        private static Coordinate _Add(Coordinate a, Coordinate b)
            => (a.x + b.x, a.y + b.y);

        private static Coordinate _Subtract(Coordinate a, Coordinate b)
            => (a.x - b.x, a.y - b.y);
    }
}
