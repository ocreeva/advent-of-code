using Moyba.AdventOfCode.Utility;

namespace Moyba.AdventOfCode.Year2024
{
    using Route = (Coordinate destination, Coordinate endFacing, long score, HashSet<Coordinate> visited);
    using Path = (Coordinate position, Coordinate facing, long score, HashSet<Coordinate> visited);

    public class Day16 : IPuzzle
    {
        private static readonly Route _EmptyRoute = (new Coordinate(0, 0), Coordinate.North, 0, new HashSet<Coordinate>());

        private readonly HashSet<Coordinate>
            _intersections,
            _tiles = new HashSet<Coordinate>();
        private readonly Coordinate _start, _end;

        public Day16(string[] data)
        {
            for (var y = 0; y < data.Length; y++)
            {
                var line = data[y];
                for (var x = 0; x < line.Length; x++)
                {
                    switch (line[x])
                    {
                        case '.':
                            _tiles.Add(new Coordinate(x, y));
                            break;

                        case 'S':
                            _start = new Coordinate(x, y);
                            _tiles.Add(_start);
                            break;

                        case 'E':
                            _end = new Coordinate(x, y);
                            _tiles.Add(_end);
                            break;

                        case '#':
                            break;

                        default:
                            throw new NotSupportedException($"Unhandled maze character ({line[x]}).");
                    }
                }
            }

            foreach (var tile in _tiles.ToArray())
            {
                var position = tile;
                do
                {
                    if (!_tiles.Contains(position)) break;

                    var adjacent = Coordinate.Orthogonals.Select(_ => position + _).Where(_ => _tiles.Contains(_)).ToArray();
                    if (adjacent.Length > 1) break;

                    _tiles.Remove(position);
                    position = adjacent[0];
                }
                while (true);
            }

            _intersections = _tiles
                .Where(tile => tile == _start || tile == _end || Coordinate.Orthogonals.Count(orthogonal => _tiles.Contains(tile + orthogonal)) > 2)
                .ToHashSet();
        }

        [PartOne("123540")]
        [PartTwo("665")]
        public async IAsyncEnumerable<string?> ComputeAsync()
        {
            var routes = new Dictionary<Coordinate, IDictionary<Coordinate, Route>>();
            foreach (var intersection in _intersections)
            {
                // no need to map routes out of the end
                if (intersection == _end) continue;

                routes[intersection] = new Dictionary<Coordinate, Route>();
                foreach (var orthogonal in Coordinate.Orthogonals)
                {
                    if (this.TryWalkPath(intersection, orthogonal, out Route route))
                    {
                        routes[intersection].Add(orthogonal, route);
                    }
                }
            }

            var minScores = _intersections.ToDictionary(_ => _, _ => Int64.MaxValue);
            var minVisited = new HashSet<Coordinate>();

            var queue = new PriorityQueue<Path, long>();
            queue.Enqueue((_start, Coordinate.East, 0, new HashSet<Coordinate> { _start }), this.GetPriority(0, _start));
            while (queue.Count > 0)
            {
                (var position, var facing, var score, var visited) = queue.Dequeue();
                if (score - 1000 > minScores[position]) continue;

                var reverse = -facing;
                foreach (var nextFacing in routes[position].Keys)
                {
                    var route = routes[position][nextFacing];
                    if (visited.Contains(route.destination)) continue;

                    var nextScore = score + route.score + (nextFacing == facing ? 0 : 1000);
                    if (nextFacing == reverse)
                    {
                        if (position != _start) continue;
                        nextScore += 1000;
                    }

                    if (route.destination == _end)
                    {
                        if (nextScore < minScores[_end])
                        {
                            minScores[_end] = nextScore;
                            minVisited = new HashSet<Coordinate>();
                        }

                        if (nextScore == minScores[_end])
                        {
                            foreach (var location in visited) minVisited.Add(location);
                            foreach (var location in route.visited) minVisited.Add(location);
                        }

                        continue;
                    }

                    if (nextScore < minScores[route.destination]) minScores[route.destination] = nextScore;
                    else if (nextScore - 1000 > minScores[route.destination]) continue;

                    var nextPriority = this.GetPriority(nextScore, route.destination);

                    var nextVisited = new HashSet<Coordinate>(visited);
                    foreach (var location in route.visited) nextVisited.Add(location);

                    queue.Enqueue((route.destination, route.endFacing, nextScore, nextVisited), nextPriority);
                }
            }

            yield return $"{minScores[_end]}";

            yield return $"{minVisited.Count}";

            await Task.CompletedTask;
        }

        private long GetPriority(long score, Coordinate position)
        {
            var xCost = Math.Abs(position.x - _end.x);
            var yCost = Math.Abs(position.y - _end.y);
            var turnCost = (xCost > 0 && yCost > 0) ? 1000 : 0;
            return score + xCost + yCost + turnCost;
        }

        private bool TryWalkPath(Coordinate source, Coordinate facing, out Route route)
        {
            route = _EmptyRoute;

            var target = source + facing;
            if (!_tiles.Contains(target)) return false;

            // don't walk back to start
            if (_start == target) return false;

            if (_intersections.Contains(target) || _end == target)
            {
                route = (target, facing, 1, new HashSet<Coordinate> { target });
                return true;
            }

            if (this.TryWalkPath(target, facing, out route))
            {
                route.score++;
                route.visited.Add(target);
                return true;
            }

            if (this.TryWalkPath(target, new Coordinate(-facing.y, facing.x), out route))
            {
                route.score += 1001;
                route.visited.Add(target);
                return true;
            }

            if (this.TryWalkPath(target, new Coordinate(facing.y, -facing.x), out route))
            {
                route.score += 1001;
                route.visited.Add(target);
                return true;
            }

            return false;
        }
    }
}
