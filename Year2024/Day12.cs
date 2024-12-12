namespace Moyba.AdventOfCode.Year2024
{
    using Coordinate = (int x, int y);

    public class Day12(string[] _data) : IPuzzle
    {
        private static readonly Coordinate
            _Up    = (  0, -1 ),
            _Down  = (  0,  1 ),
            _Left  = ( -1,  0 ),
            _Right = (  1,  0 );
        private static readonly IEnumerable<Coordinate> _Orthogonals = [ _Up, _Down, _Left, _Right ];

        private readonly char[][] _plants = _data
            .Select(_ => $".{_}.".ToCharArray())
            .Prepend(Enumerable.Repeat('.', _data[0].Length + 2).ToArray())
            .Append(Enumerable.Repeat('.', _data[0].Length + 2).ToArray())
            .ToArray();

        private readonly int _height = _data.Length + 2, _width = _data[0].Length + 2;

        [PartOne("1370258")]
        [PartTwo("805814")]
        public async IAsyncEnumerable<string?> ComputeAsync()
        {
            var visited = new HashSet<Coordinate>();
            var regions = new List<Region>();

            for (var y = 1; y < _height - 1; y++)
            {
                for (var x = 1; x < _width - 1; x++)
                {
                    var c = (x, y);
                    if (visited.Contains(c)) continue;

                    var region = new Region(_plants[y][x]);
                    region.TraverseFrom(c, _plants);
                    foreach (var location in region.Locations) visited.Add(location);

                    regions.Add(region);
                }
            }

            var part1 = regions.Sum(_ => _.Locations.Count * _.Perimeter);

            yield return $"{part1}";

            foreach (var region in regions) region.CalculateSides();
            var part2 = regions.Sum(_ => _.Locations.Count * _.Sides);

            yield return $"{part2}";

            await Task.CompletedTask;
        }

        private class Region(char _plant)
        {
            public char Plant => _plant;

            public HashSet<Coordinate> Locations { get; } = new HashSet<Coordinate>();
            public int Perimeter { get; set; }
            public int Sides { get; set; }

            public void TraverseFrom(Coordinate c, char[][] plants)
            {
                if (this.Locations.Contains(c)) return;
                this.Locations.Add(c);

                foreach (var adjacent in _Orthogonals.Select(_ => _Add(c, _)))
                {
                    if (plants[adjacent.y][adjacent.x] == this.Plant) this.TraverseFrom(adjacent, plants);
                    else this.Perimeter++;
                }
            }

            public void CalculateSides()
            {
                foreach (var location in this.Locations)
                {
                    if (this.IsStartOfSide(location, _Up, _Left)) this.Sides++;
                    if (this.IsStartOfSide(location, _Left, _Up)) this.Sides++;
                    if (this.IsStartOfSide(location, _Down, _Right)) this.Sides++;
                    if (this.IsStartOfSide(location, _Right, _Down)) this.Sides++;
                }
            }

            private bool IsStartOfSide(Coordinate c, Coordinate direction, Coordinate continuation)
            {
                if (this.Locations.Contains(_Add(c, direction))) return false;

                var previous = _Add(c, continuation);
                return !this.Locations.Contains(previous)
                    || this.Locations.Contains(_Add(previous, direction));
            }
        }

        private static Coordinate _Add(Coordinate a, Coordinate b)
            => (a.x + b.x, a.y + b.y);
    }
}
