using Moyba.AdventOfCode.Utility;

namespace Moyba.AdventOfCode.Year2024
{
    public class Day20 : IPuzzle
    {
        private static readonly IDictionary<int, IEnumerable<Coordinate>> _Cheats = Enumerable
            .Range(2, 19)
            .ToDictionary(_ => _, _ => (IEnumerable<Coordinate>)_GenerateCheatOffsets(_).ToArray());

        private readonly HashSet<Coordinate> _track = new HashSet<Coordinate>();
        private readonly Coordinate _start, _end;

        public Day20(string[] data)
        {
            for (var y = 0; y < data.Length; y++)
            {
                var line = data[y];
                for (var x = 0; x < line.Length; x++)
                {
                    switch (line[x])
                    {
                        case '.':
                            _track.Add(new Coordinate(x, y));
                            break;

                        case 'S':
                            _start = new Coordinate(x, y);
                            _track.Add(_start);
                            break;

                        case 'E':
                            _end = new Coordinate(x, y);
                            _track.Add(_end);
                            break;

                        case '#':
                            break;

                        default:
                            throw new NotSupportedException($"Unsupported map character ({line[x]}).");
                    }
                }
            }
        }

        [PartOne("1369")]
        [PartTwo("979012")]
        public async IAsyncEnumerable<string?> ComputeAsync()
        {
            var timeAtPosition = new Dictionary<Coordinate, long> { { _start, 0 } };

            var queue = new PriorityQueue<Coordinate, long>();
            queue.Enqueue(_start, 0);
            while (queue.Count > 0)
            {
                var position = queue.Dequeue();
                var time = timeAtPosition[position];
                foreach (var nextPosition in position.Orthogonal)
                {
                    if (!_track.Contains(nextPosition)) continue;

                    if (timeAtPosition.ContainsKey(nextPosition)) continue;

                    timeAtPosition.Add(nextPosition, time + 1);
                    queue.Enqueue(nextPosition, time + 1);
                }
            }

            var part1 = 0;
            var maxTime = timeAtPosition[_end];
            foreach (var position in timeAtPosition.Keys)
            {
                var targetTime = timeAtPosition[position] + 102;
                if (targetTime > maxTime) continue;

                foreach (var cheat in _Cheats[2])
                {
                    var cheatDestination = position + cheat;
                    if (!timeAtPosition.Keys.Contains(cheatDestination)) continue;
                    if (targetTime > timeAtPosition[cheatDestination]) continue;

                    part1++;
                }
            }

            yield return $"{part1}";

            var part2 = 0;
            foreach (var position in timeAtPosition.Keys)
            {
                for (var steps = 2; steps <= 20; steps++)
                {
                    var targetTime = timeAtPosition[position] + 100 + steps;
                    foreach (var cheat in _Cheats[steps])
                    {
                        var destination = position + cheat;
                        if (!timeAtPosition.ContainsKey(destination)) continue;

                        if (targetTime > timeAtPosition[destination]) continue;

                        part2++;
                    }
                }
            }

            yield return $"{part2}";

            await Task.CompletedTask;
        }

        private static IEnumerable<Coordinate> _GenerateCheatOffsets(int size)
        {
            for (var i = 0; i < size; i++)
            {
                yield return new Coordinate(i, i - size);  // 0, -2 :: 1, -1
                yield return new Coordinate(size - i, i);  // 2, 0  :: 1, 1
                yield return new Coordinate(-i, size - i); // 0, 2  :: -1, 1
                yield return new Coordinate(i - size, -i); // -2, 0 :: -1, -1
            }
        }
    }
}
