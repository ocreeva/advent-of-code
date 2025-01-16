using Moyba.AdventOfCode.Utility;

namespace Moyba.AdventOfCode.Year2024
{
    public class Day15 : IPuzzle
    {
        private readonly HashSet<Coordinate>
            _boxesPart1 = new HashSet<Coordinate>(),
            _boxesPart2 = new HashSet<Coordinate>(),
            _wallsPart1 = new HashSet<Coordinate>(),
            _wallsPart2 = new HashSet<Coordinate>();
        private readonly Coordinate[] _movements;

        private Coordinate _robot, _robotPart1, _robotPart2;
        private HashSet<Coordinate> _boxes, _walls;

        public Day15(string[] data)
        {
            int y;
            for (y = 0; !String.IsNullOrEmpty(data[y]); y++)
            {
                var line = data[y];
                for (var x = 0; x < line.Length; x++)
                {
                    switch (line[x])
                    {
                        case '#':
                            _wallsPart1.Add(new Coordinate(x, y));
                            _wallsPart2.Add(new Coordinate(x * 2, y));
                            _wallsPart2.Add(new Coordinate(x * 2 + 1, y));
                            break;

                        case 'O':
                            _boxesPart1.Add(new Coordinate(x, y));
                            _boxesPart2.Add(new Coordinate(x * 2, y));
                            break;

                        case '@':
                            _robotPart1 = new Coordinate(x, y);
                            _robotPart2 = new Coordinate(x * 2, y);
                            break;

                        case '.':
                            break;

                        default:
                            throw new NotSupportedException($"Unhandled map value '{line[x]}'.");
                    }
                }
            }

            _movements = data.Skip(y + 1).SelectMany(_ => _.Select(_ParseMovement)).ToArray();

            _boxes = _boxesPart1;
            _robot = _robotPart1;
            _walls = _wallsPart1;
        }

        [PartOne("1490942")]
        [PartTwo("1519202")]
        public async IAsyncEnumerable<string?> ComputeAsync()
        {
            foreach (var movement in _movements) this.MoveRobot(movement);
            var part1 = _boxes.Sum(_ => 100 * _.y + _.x);

            yield return $"{part1}";

            _boxes = _boxesPart2;
            _robot = _robotPart2;
            _walls = _wallsPart2;
            foreach (var movement in _movements) this.MoveRobot(movement, boxesAreWide: true);
            var part2 = _boxes.Sum(_ => 100 * _.y + _.x);

            yield return $"{part2}";

            await Task.CompletedTask;
        }

        private void MoveRobot(Coordinate movement, bool boxesAreWide = false)
        {
            if (this.DoesMovementHitWall(_robot, movement, false)) return;

            var pushedBoxes = this.FindPushedBoxes(movement, boxesAreWide).ToArray();
            foreach (var pushedBox in pushedBoxes) if (this.DoesMovementHitWall(pushedBox, movement, boxesAreWide)) return;

            _robot += movement;
            foreach (var pushedBox in pushedBoxes) _boxes.Remove(pushedBox);
            foreach (var pushedBox in pushedBoxes) _boxes.Add(pushedBox + movement);
        }

        private IEnumerable<Coordinate> FindPushedBoxes(Coordinate movement, bool boxesAreWide)
        {
            var queued = new HashSet<Coordinate>();
            var pending = new Queue<Coordinate>();

            var target = _robot + movement;
            _EnqueueIfNotYet(target, queued, pending);

            if (boxesAreWide) _EnqueueIfNotYet(target + Coordinate.West, queued, pending);

            while (pending.Count > 0)
            {
                var box = pending.Dequeue();
                if (!_boxes.Contains(box)) continue;

                yield return box;

                target = box + movement;
                _EnqueueIfNotYet(target, queued, pending);

                if (boxesAreWide)
                {
                    _EnqueueIfNotYet(target + Coordinate.West, queued, pending);
                    _EnqueueIfNotYet(target + Coordinate.East, queued, pending);
                }
            }
        }

        private static void _EnqueueIfNotYet(Coordinate target, HashSet<Coordinate> queued, Queue<Coordinate> pending)
        {
            if (queued.Contains(target)) return;

            queued.Add(target);
            pending.Enqueue(target);
        }

        private bool DoesMovementHitWall(Coordinate source, Coordinate movement, bool isWideBox)
        {
            var target = source + movement;
            if (_walls.Contains(target)) return true;

            if (!isWideBox) return false;

            target += Coordinate.East;
            if (_walls.Contains(target)) return true;

            return false;
        }

        private static Coordinate _ParseMovement(char _)
            => _ switch
            {
                '^' => Coordinate.North,
                '>' => Coordinate.East,
                'v' => Coordinate.South,
                '<' => Coordinate.West,
                _ => throw new NotSupportedException($"Unhandled movement '{_}'.")
            };
    }
}
