namespace Moyba.AdventOfCode.Year2023
{
    public class Day14(string[] _data) : IPuzzle
    {
        private const char _RoundRock = 'O';
        private const char _CubeRock = '#';
        private const char _EmptySpace = '.';
        private const long _MaxCycles = 1_000_000_000;

        private readonly char[][] _map = _data.Select(_ => _.ToCharArray()).ToArray();
        private readonly int _width = _data[0].Length;
        private readonly int _height = _data.Length;

        [PartOne("109939")]
        [PartTwo("101010")]
        public async IAsyncEnumerable<string?> ComputeAsync()
        {
            this.TiltNorth();

            yield return $"{this.CalculateTotalLoad(_map)}";

            var pastMaps = new Dictionary<string, long>();
            for (var iteration = 0L; iteration < _MaxCycles; iteration++)
            {
                this.TiltWest();
                this.TiltSouth();
                this.TiltEast();

                var key = String.Join(' ', _map.Select(_ => new string(_)));
                if (pastMaps.ContainsKey(key))
                {
                    var previousIteration = pastMaps[key];
                    var cycleLength = iteration - previousIteration;
                    var offset = (_MaxCycles - previousIteration - 1) % cycleLength;
                    var targetIteration = previousIteration + offset;
                    var map = pastMaps
                        .Where(_ => _.Value == targetIteration)
                        .Select(_ => _.Key)
                        .Single()
                        .Split(' ')
                        .Select(_ => _.ToCharArray())
                        .ToArray();
                    yield return $"{this.CalculateTotalLoad(map)}";
                    break;
                }

                pastMaps.Add(key, iteration);

                this.TiltNorth();
            }

            await Task.CompletedTask;
        }

        private long CalculateTotalLoad(char[][] map)
        {
            var total = 0L;
            for (var y = 0; y < _height; y++)
            {
                var load = _height - y;
                for (var x = 0; x < _width; x++)
                {
                    if (map[y][x] != _RoundRock) continue;
                    total += load;
                }
            }

            return total;
        }

        private void TiltNorth()
        {
            for (var x = 0; x < _width; x++)
            {
                var available = 0;
                for (var y = available; y < _height; y++)
                {
                    switch (_map[y][x])
                    {
                        case _RoundRock:
                            _map[y][x] = _EmptySpace;
                            _map[available++][x] = _RoundRock;
                            break;

                        case _CubeRock:
                            available = y + 1;
                            break;
                    }
                }
            }
        }

        private void TiltSouth()
        {
            for (var x = 0; x < _width; x++)
            {
                var available = _height - 1;
                for (var y = available; y >= 0; y--)
                {
                    switch (_map[y][x])
                    {
                        case _RoundRock:
                            _map[y][x] = _EmptySpace;
                            _map[available--][x] = _RoundRock;
                            break;

                        case _CubeRock:
                            available = y - 1;
                            break;
                    }
                }
            }
        }

        private void TiltWest()
        {
            for (var y = 0; y < _height; y++)
            {
                var available = 0;
                for (var x = available; x < _width; x++)
                {
                    switch (_map[y][x])
                    {
                        case _RoundRock:
                            _map[y][x] = _EmptySpace;
                            _map[y][available++] = _RoundRock;
                            break;

                        case _CubeRock:
                            available = x + 1;
                            break;
                    }
                }
            }
        }

        private void TiltEast()
        {
            for (var y = 0; y < _height; y++)
            {
                var available = _width - 1;
                for (var x = available; x >= 0; x--)
                {
                    switch (_map[y][x])
                    {
                        case _RoundRock:
                            _map[y][x] = _EmptySpace;
                            _map[y][available--] = _RoundRock;
                            break;

                        case _CubeRock:
                            available = x - 1;
                            break;
                    }
                }
            }
        }
    }
}
