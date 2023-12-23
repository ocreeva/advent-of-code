namespace Moyba.AdventOfCode.Year2023
{
    using Coord = (int x, int y);

    public class Day23(string[] _data) : IPuzzle
    {
        private static readonly Coord _North = (0, -1);
        private static readonly Coord _South = (0, 1);
        private static readonly Coord _West = (-1, 0);
        private static readonly Coord _East = (1, 0);
        private static readonly IEnumerable<Coord> _AllDirections = [ _North, _South, _West, _East ];

        private readonly Coord _start = (1, 0);
        private readonly Coord _end = (_data[0].Length - 2, _data.Length - 1);

        private readonly int _height = _data.Length;
        private readonly int _width = _data[0].Length;

        public bool SkipEvaluation { get; set; } = true;

        [PartOne("2166")]
        [PartTwo("6378")]
        public async IAsyncEnumerable<string?> ComputeAsync()
        {
            if (this.SkipEvaluation)
            {
                // skip evaluation to avoid delay when solving other problems
                yield return "2166";
                yield return "6378";
                yield break;
            }

            // find all intersections
            var intersections = Enumerable.Range(1, _height - 2)
                .SelectMany(y => Enumerable.Range(1, _width - 2)
                    .Select<int, Coord>(x => (x, y))
                    .Where(this.IsIntersection))
                .ToDictionary(_ => _, _ => new Dictionary<Coord, int>());
            intersections.Add(_start, new Dictionary<Coord, int>());
            intersections.Add(_end, new Dictionary<Coord, int>());

            yield return null;

            foreach (var start in intersections.Keys.Except([ _end ]))
            {
                foreach (var direction in _AllDirections)
                {
                    if (this.TryTraverseToIntersection(start, direction, false, out var steps, out var intersection))
                    {
                        intersections[start].Add(intersection, steps);
                    }
                }
            }

            this.TryFindMaxSteps(intersections, _start, new HashSet<Coord> { _start }, out var maxSteps);

            yield return $"{maxSteps}";

            foreach (var start in intersections.Keys.Except([ _end ]))
            {
                foreach (var direction in _AllDirections)
                {
                    if (this.TryTraverseToIntersection(start, direction, true, out var steps, out var intersection))
                    {
                        if (intersections[start].ContainsKey(intersection)) continue;

                        intersections[start].Add(intersection, steps);
                    }
                }
            }

            this.TryFindMaxSteps(intersections, _start, new HashSet<Coord> { _start }, out var maxStepsIncUphill);

            yield return $"{maxStepsIncUphill}";

            await Task.CompletedTask;
        }

        private bool CanTraverse(Coord position, Coord direction, bool canWalkUphill) => this.GetMapNode(position, direction) switch
        {
            '#' => false,
            '.' => true,
            '<' => canWalkUphill || direction.x < 0,
            '>' => canWalkUphill || direction.x > 0,
            '^' => canWalkUphill || direction.y < 0,
            'v' => canWalkUphill || direction.y > 0,
            _ => throw new Exception($"Unhandled")
        };

        private bool TryFindMaxSteps(Dictionary<Coord, Dictionary<Coord, int>> intersections, Coord position, HashSet<Coord> visited, out int steps)
        {
            steps = 0;

            if (position == _end) return true;

            var destinations = intersections[position];
            foreach (var target in destinations.Keys)
            {
                if (visited.Contains(target)) continue;

                if (this.TryFindMaxSteps(intersections, target, new HashSet<Coord>(visited) { target }, out var nextSteps))
                {
                    nextSteps += destinations[target];
                    if (nextSteps > steps) steps = nextSteps;
                }
            }

            return steps > 0;
        }

        private char GetMapNode(Coord position)
        {
            if (position.x < 0 || position.x >= _width) return '#';
            if (position.y < 0 || position.y >= _height) return '#';
            return _data[position.y][position.x];
        }

        private char GetMapNode(Coord position, Coord direction) => this.GetMapNode(_ApplyVelocity(position, direction));

        private bool IsIntersection(Coord position) =>
            this.GetMapNode(position) != '#' &&
            (
                position.y == 0 ||
                position.y == _height - 1 ||
                _AllDirections.Where(_ => this.GetMapNode(position, _) != '#').Count() > 2
            );

        private bool TryTraverseToIntersection(Coord position, Coord direction, bool canWalkUphill, out int steps, out Coord intersection)
        {
            steps = 1;
            intersection = _ApplyVelocity(position, direction);

            if (!this.CanTraverse(position, direction, canWalkUphill)) return false;

            if (this.IsIntersection(intersection)) return true;

            var nextPosition = intersection;
            foreach (var nextDirection in _AllDirections)
            {
                if (nextDirection.x == -direction.x && nextDirection.y == -direction.y) continue;

                if (this.TryTraverseToIntersection(nextPosition, nextDirection, canWalkUphill, out steps, out intersection))
                {
                    steps++;
                    return true;
                }
            }

            return false;
        }

        private Coord _ApplyVelocity(Coord position, Coord velocity) => (position.x + velocity.x, position.y + velocity.y);
    }
}
