namespace Moyba.AdventOfCode.Year2023
{
    using Coord = (int x, int y);
    using Light = ((int x, int y) position, (int x, int y) velocity);

    public class Day16(string[] _data) : IPuzzle
    {
        private const char _UpRightMirror = '/';
        private const char _DownRightMirror = '\\';
        private const char _VerticalSplitter = '|';
        private const char _HorizontalSplitter = '-';

        private static readonly Coord _InvalidCoord = (-1, -1);

        private readonly int _height = _data.Length;
        private readonly int _width = _data[0].Length;

        [PartOne("6855")]
        [PartTwo("7513")]
        public async IAsyncEnumerable<string?> ComputeAsync()
        {
            var splitters = this.FindAllSplitters();

            var tiles = new Dictionary<Coord, HashSet<Coord>>();
            var next = new Dictionary<Coord, List<Coord>>();
            foreach (var splitter in splitters) this.TraceSplitter(splitter, tiles, next);
            foreach (var splitter in splitters) this.FinalizeSplitter(splitter, tiles, next);

            yield return null;

            var count = this.EnergizeTiles((-1, 0), (1, 0), tiles);
            yield return $"{count}";

            var startingLights = Enumerable.Range(0, _height)
                .SelectMany<int, Light>(y => [ ((-1, y), (1, 0)), ((_width, y), (-1, 0)) ])
                .Concat(Enumerable.Range(0, _width)
                    .SelectMany<int, Light>(x => [ ((x, -1), (0, 1)), ((x, _height), (0, -1)) ]));
            var max = startingLights.Max(light => this.EnergizeTiles(light.position, light.velocity, tiles));
            yield return $"{max}";

            await Task.CompletedTask;
        }

        private int EnergizeTiles(Coord position, Coord velocity, IDictionary<Coord, HashSet<Coord>> splitters)
        {
            var tiles = new HashSet<Coord>();
            var next = this.FollowLight(position, velocity, tiles, null, true);
            if (splitters.ContainsKey(next)) foreach (var tile in splitters[next]) tiles.Add(tile);
            return tiles.Count;
        }

        private ICollection<Coord> FindAllSplitters()
        {
            var result = new List<Coord>();
            for (var y = 0; y < _data.Length; y++)
            {
                var line = _data[y];
                for (var x = 0; x < line.Length; x++)
                {
                    switch (line[x])
                    {
                        case _HorizontalSplitter:
                        case _VerticalSplitter:
                            result.Add((x, y));
                            break;
                    }
                }
            }

            return result;
        }

        private void TraceSplitter(Coord position, IDictionary<Coord, HashSet<Coord>> tiles, IDictionary<Coord, List<Coord>> next)
        {
            if (tiles.ContainsKey(position)) return;

            Coord[] velocities;
            switch (_data[position.y][position.x])
            {
                case _HorizontalSplitter:
                    velocities = [ (-1, 0), (1, 0) ];
                    break;

                case _VerticalSplitter:
                    velocities = [ (0, -1), (0, 1) ];
                    break;

                default:
                    throw new Exception($"Position ({position}) is not a splitter ({_data[position.y][position.x]}).");
            }

            tiles.Add(position, new HashSet<Coord> { position });
            next.Add(position, new List<Coord>());
            var identical = new List<Coord>();
            foreach (var velocity in velocities)
            {
                var nextSplitter = this.FollowLight(position, velocity, tiles[position], identical);
                if (nextSplitter == position) break; // loop, no need to evaluate twice
                if (nextSplitter != _InvalidCoord) next[position].Add(nextSplitter);
            }

            foreach (var splitter in identical)
            {
                tiles.Add(splitter, tiles[position]);
                next.Add(splitter, next[position]);
            }
        }

        private Coord FollowLight(Coord start, Coord velocity, HashSet<Coord> tiles, ICollection<Coord>? identical, bool exitOnAnySplitter = false)
        {
            var position = start;
            do
            {
                position.x += velocity.x;
                if (position.x < 0 || position.x >= _width) return _InvalidCoord;

                position.y += velocity.y;
                if (position.y < 0 || position.y >= _height) return _InvalidCoord;

                switch (_data[position.y][position.x])
                {
                    case _UpRightMirror:
                        velocity = (-velocity.y, -velocity.x);
                        break;

                    case _DownRightMirror:
                        velocity = (velocity.y, velocity.x);
                        break;

                    case _HorizontalSplitter:
                        if (exitOnAnySplitter || velocity.y != 0) return position;
                        identical?.Add(position);
                        break;

                    case _VerticalSplitter:
                        if (exitOnAnySplitter || velocity.x != 0) return position;
                        identical?.Add(position);
                        break;
                }

                tiles.Add(position);
            }
            while (position != start);

            return position;
        }

        private void FinalizeSplitter(Coord position, IDictionary<Coord, HashSet<Coord>> tiles, IDictionary<Coord, List<Coord>> next)
        {
            var myTiles = tiles[position];
            var myNext = next[position];
            for (var index = 0; index < myNext.Count; index++)
            {
                var nextSplitter = myNext[index];
                if (myTiles.Contains(nextSplitter)) continue;

                var nextTiles = tiles[nextSplitter];
                foreach (var tile in nextTiles) myTiles.Add(tile);

                var nextNext = next[nextSplitter];
                foreach (var nextNextSplitter in nextNext) myNext.Add(nextNextSplitter);
            }
        }
    }
}
