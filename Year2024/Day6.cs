namespace Moyba.AdventOfCode.Year2024
{
    using Coordinate = (int x, int y);

    public class Day6 : IPuzzle
    {
        private readonly HashSet<Coordinate> _obstructions = new HashSet<Coordinate>();

        private Coordinate _position;
        private Coordinate _heading = (0, -1);
        private int _height, _width;

        private enum GuardStatus
        {
            Exited,
            Looped,
        }

        public Day6(string[] data)
        {
            _height = data.Length;
            _width = data[0].Length;

            for (var y = 0; y < _height; y++)
            {
                for (var x = 0; x < _width; x++)
                {
                    switch (data[y][x])
                    {
                        case '#':
                            _obstructions.Add((x, y));
                            break;

                        case '^':
                            _position = (x, y);
                            break;
                    }
                }
            }
        }

        [PartOne("5242")]
        [PartTwo("1424")]
        public async IAsyncEnumerable<string?> ComputeAsync()
        {
            (var status, var visited) = this.CheckGuardPath();

            yield return $"{visited.Count}";

            var loopingObstacles = visited
                .Except([ _position ])
                .AsParallel()
                .Select(extraObstruction => this.CheckGuardPath(extraObstruction))
                .Count(result => result.status == GuardStatus.Looped);

            yield return $"{loopingObstacles}";

            await Task.CompletedTask;
        }

        private (GuardStatus status, HashSet<Coordinate> visited) CheckGuardPath(Coordinate? extraObstruction = null)
        {
            var position = _position;
            var heading = _heading;

            var visited = new HashSet<Coordinate>();
            var history = new HashSet<(Coordinate, Coordinate)>();

            do
            {
                visited.Add(position);
                history.Add((position, heading));

                Coordinate nextPosition = (position.x + heading.x, position.y + heading.y);
                if (_obstructions.Contains(nextPosition) || nextPosition == extraObstruction)
                {
                    // turn right
                    heading = (-heading.y, heading.x);
                    continue;
                }

                if (nextPosition.x < 0 || nextPosition.x >= _width ||
                    nextPosition.y < 0 || nextPosition.y >= _height)
                {
                    return (GuardStatus.Exited, visited);
                }

                position = nextPosition;
            } while (!history.Contains((position, heading)));

            return (GuardStatus.Looped, visited);
        }
    }
}
