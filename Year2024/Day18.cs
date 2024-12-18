using Moyba.AdventOfCode.Utility;

namespace Moyba.AdventOfCode.Year2024
{
    public class Day18(string[] _data) : IPuzzle
    {
        private static readonly Coordinate _End = new Coordinate(70, 70);
        private static readonly Coordinate[] _CardinalAndIntercardinal = [
            new Coordinate(0, -1),
            new Coordinate(1, -1),
            new Coordinate(1, 0),
            new Coordinate(1, 1),
            new Coordinate(0, 1),
            new Coordinate(-1, 1),
            new Coordinate(-1, 0),
            new Coordinate(-1, -1),
        ];

        private readonly Coordinate[] _drops = _data
            .Select(_ => _.Split(','))
            .Select(_ => new Coordinate(_[0], _[1]))
            .ToArray();

        private readonly HashSet<Coordinate> _dropped = new HashSet<Coordinate>();

        [PartOne("294")]
        [PartTwo("31,22")]
        public async IAsyncEnumerable<string?> ComputeAsync()
        {
            var stepsTo = new Dictionary<Coordinate, int>() { { Coordinate.Zero, 0 } };

            var queue = new PriorityQueue<Coordinate, long>();
            queue.Enqueue(Coordinate.Zero, _GetPriority(Coordinate.Zero, 0));

            for (var i = 0; i < 1024; i++) _dropped.Add(_drops[i]);

            while (!stepsTo.ContainsKey(_End))
            {
                var position = queue.Dequeue();
                var nextSteps = stepsTo[position] + 1;
                foreach (var orthogonal in Coordinate.Orthogonals)
                {
                    var nextPosition = position + orthogonal;
                    if (nextPosition.x < 0 || nextPosition.x > 70 ||
                        nextPosition.y < 0 || nextPosition.y > 70) continue;

                    if (_dropped.Contains(nextPosition)) continue;
                    if (stepsTo.ContainsKey(nextPosition)) continue;

                    stepsTo[nextPosition] = nextSteps;

                    queue.Enqueue(nextPosition, _GetPriority(nextPosition, nextSteps));
                }
            }

            yield return $"{stepsTo[_End]}";

            // for speed, jump ahead to near the final answer
            for (var i = 1024; i < 3038; i++) _dropped.Add(_drops[i]);

            for (var i = 3038; i < _drops.Length; i++)
            {
                var drop = _drops[i];
                _dropped.Add(drop);

                var test = new HashSet<Coordinate>();
                test.Add(drop);

                var wallQueue = new Queue<Coordinate>();
                wallQueue.Enqueue(drop);
                while (wallQueue.Count > 0)
                {
                    var wall = wallQueue.Dequeue();
                    foreach (var direction in _CardinalAndIntercardinal)
                    {
                        var next = wall + direction;
                        if (!_dropped.Contains(next)) continue;
                        if (test.Contains(next)) continue;

                        test.Add(next);
                        wallQueue.Enqueue(next);
                    }
                }

                if (test.Any(_ => _.x == 0 || _.y == 70) &&
                    test.Any(_ => _.x == 70 || _.y == 0))
                {
                    yield return $"{drop.x},{drop.y}";
                    break;
                }
            }

            await Task.CompletedTask;
        }

        private static long _GetPriority(Coordinate position, int steps)
            => steps + (70 - position.x) + (70 - position.y);
    }
}
