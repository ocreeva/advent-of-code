namespace Moyba.AdventOfCode.Year2023
{
    using Coord = (int x, int y);
    using Light = ((int x, int y) position, (int x, int y) velocity);

    public class Day16(string[] _data) : IPuzzle
    {
        private const char _EmptySpace = '.';
        private const char _UpRightMirror = '/';
        private const char _DownRightMirror = '\\';
        private const char _VerticalSplitter = '|';
        private const char _HorizontalSplitter = '-';

        private readonly int _height = _data.Length;
        private readonly int _width = _data[0].Length;

        private readonly HashSet<Coord> _upRightMirrors = _FindTilesMatching(_data, _UpRightMirror);
        private readonly HashSet<Coord> _downRightMirrors = _FindTilesMatching(_data, _DownRightMirror);
        private readonly HashSet<Coord> _verticalSplitters = _FindTilesMatching(_data, _VerticalSplitter);
        private readonly HashSet<Coord> _horizationSplitters = _FindTilesMatching(_data, _HorizontalSplitter);

        [PartOne("6855")]
        [PartTwo("7513")]
        public async IAsyncEnumerable<string?> ComputeAsync()
        {
            var count = this.EnergizeTiles(((0, 0), (1, 0))).Count;
            yield return $"{count}";

            var startingLights = Enumerable.Range(0, _height)
                .SelectMany<int, Light>(y => [ ((0, y), (1, 0)), ((_width - 1, y), (-1, 0)) ])
                .Concat(Enumerable.Range(0, _width)
                    .SelectMany<int, Light>(x => [ ((x, 0), (0, 1)), ((x, _height - 1), (0, -1)) ]));
            var max = startingLights.Select(light => this.EnergizeTiles(light)).Max(_ => _.Count);
            yield return $"{max}";

            await Task.CompletedTask;
        }

        private ICollection<Coord> EnergizeTiles(Light initialLight)
        {
            var visited = new Dictionary<Coord, HashSet<Coord>>();

            var pending = new Stack<Light>();
            pending.Push(initialLight);
            while (pending.TryPop(out var light))
            {
                if (light.position.x < 0 || light.position.x >= _width) continue;
                if (light.position.y < 0 || light.position.y >= _height) continue;

                if (!visited.ContainsKey(light.position)) visited.Add(light.position, new HashSet<Coord> { light.velocity });
                else if (!visited[light.position].Contains(light.velocity)) visited[light.position].Add(light.velocity);
                else continue; // avoid infinite loops

                if (_upRightMirrors.Contains(light.position)) light.velocity = (-light.velocity.y, -light.velocity.x);
                else if (_downRightMirrors.Contains(light.position)) light.velocity = (light.velocity.y, light.velocity.x);
                else if (_verticalSplitters.Contains(light.position))
                {
                    if (light.velocity.x != 0)
                    {
                        pending.Push(((light.position.x, light.position.y - 1), (0, -1)));
                        pending.Push(((light.position.x, light.position.y + 1), (0, 1)));
                        continue;
                    }
                }
                else if (_horizationSplitters.Contains(light.position))
                {
                    if (light.velocity.y != 0)
                    {
                        pending.Push(((light.position.x - 1, light.position.y), (-1, 0)));
                        pending.Push(((light.position.x + 1, light.position.y), (1, 0)));
                        continue;
                    }
                }

                light.position.x += light.velocity.x;
                light.position.y += light.velocity.y;
                pending.Push(light);
            }

            return visited.Keys;
        }

        private static HashSet<Coord> _FindTilesMatching(string[] data, char c)
        {
            return Enumerable.Range(0, data.Length)
                .SelectMany(y => Enumerable.Range(0, data[y].Length)
                    .Where(x => data[y][x] == c)
                    .Select(x => (x, y)))
                .ToHashSet();
        }
    }
}
