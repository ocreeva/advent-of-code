namespace Moyba.AdventOfCode.Year2023
{
    using Coord = (int x, int y);

    public class Day21(string[] _data) : IPuzzle
    {
        private static readonly Coord _North = (0, -1);
        private static readonly Coord _South = (0, 1);
        private static readonly Coord _West = (-1, 0);
        private static readonly Coord _East = (1, 0);
        private static readonly Coord[] _AllDirections = [ _North, _South, _West, _East ];

        private readonly int _height = _data.Length;
        private readonly int _width = _data[0].Length;
        private readonly Coord _startingPosition = Enumerable.Range(0, _data.Length)
            .SelectMany(y => Enumerable.Range(0, _data[0].Length).Select<int, Coord>(x => (x, y)))
            .Where(_ => _data[_.y][_.x] == 'S')
            .Single();

        [PartOne("3562")]
        [PartTwo("592723929260582")]
        public async IAsyncEnumerable<string?> ComputeAsync()
        {
            var visited = new HashSet<Coord> { _startingPosition };
            var plots = new HashSet<Coord> { _startingPosition };

            var queue = new Queue<Coord>();
            queue.Enqueue(_startingPosition);

            bool isEven = true;
            for (var iteration = 0; iteration < 65; iteration++, isEven = !isEven)
            {
                var nextQueue = new Queue<Coord>();
                while (queue.TryDequeue(out var position))
                {
                    foreach (var direction in _AllDirections)
                    {
                        var nextPosition = _ApplyVelocity(position, direction);
                        if (nextPosition.x < 0 || nextPosition.x >= _width) continue;
                        if (nextPosition.y < 0 || nextPosition.y >= _height) continue;
                        if (this.IsRock(nextPosition)) continue;
                        if (visited.Contains(nextPosition)) continue;

                        visited.Add(nextPosition);
                        if (!isEven) plots.Add(nextPosition);

                        nextQueue.Enqueue(nextPosition);
                    }
                }

                queue = nextQueue;
            }

            yield return $"{plots.Count}";

            plots.Clear();

            visited.Clear();
            visited.Add(_startingPosition);

            queue.Clear();
            queue.Enqueue(_startingPosition);

            long a = 0L, b = 0L, c = 0L;
            isEven = true;
            for (var iteration = 0; iteration < 327; iteration++, isEven = !isEven)
            {
                var nextQueue = new Queue<Coord>();
                while (queue.TryDequeue(out var position))
                {
                    foreach (var direction in _AllDirections)
                    {
                        var nextPosition = _ApplyVelocity(position, direction);
                        if (this.IsRock(this.NormalizeToData(nextPosition))) continue;
                        if (visited.Contains(nextPosition)) continue;

                        visited.Add(nextPosition);
                        if (isEven) plots.Add(nextPosition);

                        nextQueue.Enqueue(nextPosition);
                    }
                }

                queue = nextQueue;

                if (iteration == 130)
                {
                    var reachable = visited.Where(_ => _.x >= 0 && _.x < _width && _.y >= 0 && _.y < _height).Count();
                    a = 8L * reachable;
                }
                else if (iteration == 64)
                {
                    c = plots.Count;
                }
            }

            b = plots.Count - c;

            // 26_501_365 steps = 262 * 101_150 + 65
            var x = 101_150L;
            var finalCount = c + b * x + a * x * (x-1) / 2;

            yield return $"{finalCount}";

            await Task.CompletedTask;
        }

        private bool IsRock(Coord position) => _data[position.y][position.x] == '#';
        private Coord NormalizeToData(Coord position) => (((position.x % _width) + _width) % _width, ((position.y % _height) + _height) % _height);

        private static Coord _ApplyVelocity(Coord position, Coord velocity) => (position.x + velocity.x, position.y + velocity.y);
    }
}
