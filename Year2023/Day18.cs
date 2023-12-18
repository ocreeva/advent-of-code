namespace Moyba.AdventOfCode.Year2023
{
    using Coord = (int x, int y);
    using Instruction = (char direction, int distance, string color);

    public class Day18(string[] _data) : IPuzzle
    {
        private static readonly Coord _Up = (0, -1);
        private static readonly Coord _Down = (0, 1);
        private static readonly Coord _Left = (-1, 0);
        private static readonly Coord _Right = (1, 0);
        private static readonly Coord[] _AllDirections = [ _Up, _Down, _Left, _Right ];
        private static readonly IDictionary<char, Coord> _directions = new Dictionary<char, Coord>
        {
            { 'U', _Up },
            { 'D', _Down },
            { 'L', _Left },
            { 'R', _Right },
        };

        private readonly Instruction[] _plan = _data
            .Select(_ => _.Split(' '))
            .Select<string[], Instruction>(_ => (_[0][0], Int32.Parse(_[1]), _[2]))
            .ToArray();

        [PartOne("92758")]
        [PartTwo()]
        public async IAsyncEnumerable<string?> ComputeAsync()
        {
            Coord position = (0, 0);
            var trench = new HashSet<Coord> { position };
            foreach ((var direction, var distance, var color) in _plan)
            {
                var velocity = _directions[direction];
                for (var iteration = 0; iteration < distance; iteration++)
                {
                    position = _ApplyVelocity(position, velocity);
                    trench.Add(position);
                }
            }

            var minX = trench.Min(_ => _.x) - 1;
            var maxX = trench.Max(_ => _.x) + 1;
            var minY = trench.Min(_ => _.y) - 1;
            var maxY = trench.Max(_ => _.y) + 1;
            var outside = new HashSet<Coord>();
            var queue = new Queue<Coord>();
            queue.Enqueue((minX, minY));
            while (queue.TryDequeue(out var coord))
            {
                if (outside.Contains(coord)) continue;
                if (trench.Contains(coord)) continue;

                outside.Add(coord);
                foreach (var next in _AllDirections.Select(_ => _ApplyVelocity(coord, _)))
                {
                    if (next.x < minX || next.x > maxX || next.y < minY || next.y > maxY) continue;
                    queue.Enqueue(next);
                }
            }

            var area = (maxX - minX + 1) * (maxY - minY + 1) - outside.Count;

            yield return $"{area}";

            yield return $"";

            await Task.CompletedTask;
        }

        private static Coord _ApplyVelocity(Coord position, Coord velocity) => (position.x + velocity.x, position.y + velocity.y);
    }
}
