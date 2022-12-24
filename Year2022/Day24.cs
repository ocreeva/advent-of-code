namespace Moyba.AdventOfCode.Year2022
{
    public class Day24 : SolutionBase
    {
        private (int x, int y) _start = (-1, -1), _end = (-1, -1);
        private HashSet<(int x, int y)>[] _openSpaces = Array.Empty<HashSet<(int x, int y)>>();
        private int _foundEndOnTurn = -1;

        [Expect("255")]
        protected override string SolvePart1()
        {
            _foundEndOnTurn = this.SearchForLocation(_start, _end, 0);

            return $"{_foundEndOnTurn}";
        }

        [Expect("809")]
        protected override string SolvePart2()
        {
            var foundStartOnTurn = this.SearchForLocation(_end, _start, _foundEndOnTurn);
            var foundEndAgain = this.SearchForLocation(_start, _end, foundStartOnTurn);

            return $"{foundEndAgain}";
        }

        protected override void TransformData(IEnumerable<string> data)
        {
            var grid = data.ToArray();
            var width = grid[0].Length;
            var height = grid.Length;

            var northWind = new HashSet<(int x, int y)>[height - 2];
            var southWind = new HashSet<(int x, int y)>[height - 2];
            var westWind = new HashSet<(int x, int y)>[width - 2];
            var eastWind = new HashSet<(int x, int y)>[width - 2];

            northWind[0] = new HashSet<(int x, int y)>();
            southWind[0] = new HashSet<(int x, int y)>();
            westWind[0] = new HashSet<(int x, int y)>();
            eastWind[0] = new HashSet<(int x, int y)>();

            for (var y = 0; y < grid.Length; y++)
            {
                var line = grid[y];
                for (var x = 0; x < line.Length; x++)
                {
                    switch (line[x])
                    {
                        case '^':
                            northWind[0].Add((x, y));
                            break;

                        case 'v':
                            southWind[0].Add((x, y));
                            break;

                        case '<':
                            westWind[0].Add((x, y));
                            break;

                        case '>':
                            eastWind[0].Add((x, y));
                            break;

                        case '.':
                            if (y == 0) _start = (x, y);
                            else if (y == grid.Length - 1) _end = (x, y);
                            break;

                        case '#':
                            break;

                        default:
                            throw new Exception("Unhandled input!");
                    }
                }
            }

            for (var iteration = 1; iteration < northWind.Length; iteration++)
            {
                northWind[iteration] = new HashSet<(int x, int y)>();
                foreach (var wind in northWind[iteration - 1]) northWind[iteration].Add((wind.x, wind.y == 1 ? height - 2 : wind.y - 1));
            }

            for (var iteration = 1; iteration < southWind.Length; iteration++)
            {
                southWind[iteration] = new HashSet<(int x, int y)>();
                foreach (var wind in southWind[iteration - 1]) southWind[iteration].Add((wind.x, wind.y == height - 2 ? 1 : wind.y + 1));
            }

            for (var iteration = 1; iteration < westWind.Length; iteration++)
            {
                westWind[iteration] = new HashSet<(int x, int y)>();
                foreach (var wind in westWind[iteration - 1]) westWind[iteration].Add((wind.x == 1 ? width - 2 : wind.x - 1, wind.y));
            }

            for (var iteration = 1; iteration < eastWind.Length; iteration++)
            {
                eastWind[iteration] = new HashSet<(int x, int y)>();
                foreach (var wind in eastWind[iteration - 1]) eastWind[iteration].Add((wind.x == width - 2 ? 1 : wind.x + 1, wind.y));
            }

            var repetition = _LCM(width - 2, height - 2);
            _openSpaces = new HashSet<(int x, int y)>[repetition];
            for (var iteration = 0; iteration < repetition; iteration++)
            {
                _openSpaces[iteration] = new HashSet<(int x, int y)> { _start, _end };
                for (var y = 1; y < height - 1; y++)
                {
                    for (var x = 1; x < width - 1; x++)
                    {
                        if (northWind[iteration % northWind.Length].Contains((x, y))) continue;
                        if (southWind[iteration % southWind.Length].Contains((x, y))) continue;
                        if (westWind[iteration % westWind.Length].Contains((x, y))) continue;
                        if (eastWind[iteration % eastWind.Length].Contains((x, y))) continue;

                        _openSpaces[iteration].Add((x, y));
                    }
                }
            }
        }

        private int SearchForLocation((int x, int y) initial, (int x, int y) target, int initialTurn)
        {
            var evaluate = new Queue<((int x, int y) space, int turn)>();
            var state = new HashSet<((int x, int y) space, int turn)> { (initial, initialTurn % _openSpaces.Length) };

            evaluate.Enqueue((initial, initialTurn));
            while (evaluate.Count > 0)
            {
                (var position, var turn) = evaluate.Dequeue();
                var nextTurn = turn + 1;
                var nextIteration = nextTurn % _openSpaces.Length;

                var openSpaces = _openSpaces[nextIteration];

                var nextPosition = position;
                if (openSpaces.Contains(nextPosition))
                {
                    if (!state.Contains((nextPosition, nextIteration)))
                    {
                        state.Add((nextPosition, nextIteration));
                        evaluate.Enqueue((nextPosition, nextTurn));
                    }
                }

                nextPosition = (position.x - 1, position.y);
                if (openSpaces.Contains(nextPosition))
                {
                    if (!state.Contains((nextPosition, nextIteration)))
                    {
                        state.Add((nextPosition, nextIteration));
                        evaluate.Enqueue((nextPosition, nextTurn));
                    }
                }

                nextPosition = (position.x + 1, position.y);
                if (openSpaces.Contains(nextPosition))
                {
                    if (!state.Contains((nextPosition, nextIteration)))
                    {
                        state.Add((nextPosition, nextIteration));
                        evaluate.Enqueue((nextPosition, nextTurn));
                    }
                }

                nextPosition = (position.x, position.y - 1);
                if (openSpaces.Contains(nextPosition))
                {
                    if (!state.Contains((nextPosition, nextIteration)))
                    {
                        if (nextPosition == target) return nextTurn;

                        state.Add((nextPosition, nextIteration));
                        evaluate.Enqueue((nextPosition, nextTurn));
                    }
                }

                nextPosition = (position.x, position.y + 1);
                if (openSpaces.Contains(nextPosition))
                {
                    if (!state.Contains((nextPosition, nextIteration)))
                    {
                        if (nextPosition == target) return nextTurn;

                        state.Add((nextPosition, nextIteration));
                        evaluate.Enqueue((nextPosition, nextTurn));
                    }
                }
            }

            throw new Exception("Failed to find a path!");
        }
    }
}
