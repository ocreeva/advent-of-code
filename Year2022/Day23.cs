using System.Text.RegularExpressions;

namespace Moyba.AdventOfCode.Year2022
{
    public class Day23 : SolutionBase
    {
        private HashSet<(int x, int y)> _elves = new HashSet<(int x, int y)>();
        private Direction[] _directions = { Direction.North, Direction.South, Direction.West, Direction.East };
        private int _directionOffset = 0;

        [Expect("4138")]
        protected override string SolvePart1()
        {
            bool movedAnElf;
            do
            {
                movedAnElf = false;

                var desiredMoves = new Dictionary<(int x, int y), (int x, int y)>();
                foreach (var elf in _elves)
                {
                    var next = this.FindDesiredNextLocation(elf);
                    if (next == elf) continue;

                    desiredMoves[elf] = next;
                }

                foreach (var realizedMove in desiredMoves.GroupBy(x => x.Value).Where(x => x.Count() == 1).SelectMany(x => x))
                {
                    movedAnElf = true;
                    _elves.Remove(realizedMove.Key);
                    _elves.Add(realizedMove.Value);
                }

                _directionOffset++;
            }
            while (movedAnElf && _directionOffset < 10);

            var xMin = _elves.Min(e => e.x);
            var xMax = _elves.Max(e => e.x);
            var yMin = _elves.Min(e => e.y);
            var yMax = _elves.Max(e => e.y);
            var openSpaces = (xMax - xMin + 1) * (yMax - yMin + 1) - _elves.Count;

            return $"{openSpaces}";
        }

        [Expect("1010")]
        protected override string SolvePart2()
        {
            bool movedAnElf;
            do
            {
                movedAnElf = false;

                var desiredMoves = new Dictionary<(int x, int y), (int x, int y)>();
                foreach (var elf in _elves)
                {
                    var next = this.FindDesiredNextLocation(elf);
                    if (next == elf) continue;

                    desiredMoves[elf] = next;
                }

                foreach (var realizedMove in desiredMoves.GroupBy(x => x.Value).Where(x => x.Count() == 1).SelectMany(x => x))
                {
                    movedAnElf = true;
                    _elves.Remove(realizedMove.Key);
                    _elves.Add(realizedMove.Value);
                }

                _directionOffset++;
            }
            while (movedAnElf);

            return $"{_directionOffset}";
        }

        protected override void TransformData(IEnumerable<string> data)
        {
            _elves = data.SelectMany((string line, int y) => line.Select((char value, int x) => (x, y, value)).Where(t => t.value == '#').Select(t => (t.x, t.y))).ToHashSet();
        }

        private (int x, int y) FindDesiredNextLocation((int x, int y) location)
        {
            var n = _elves.Contains((location.x, location.y - 1));
            var s = _elves.Contains((location.x, location.y + 1));
            var w = _elves.Contains((location.x - 1, location.y));
            var e = _elves.Contains((location.x + 1, location.y));
            var nw = _elves.Contains((location.x - 1, location.y - 1));
            var ne = _elves.Contains((location.x + 1, location.y - 1));
            var sw = _elves.Contains((location.x - 1, location.y + 1));
            var se = _elves.Contains((location.x + 1, location.y + 1));

            if (!n && !s && !w && !e && !nw && !ne && !sw && !se) return location;

            for (var index = 0; index < 4; index++)
            {
                switch (_directions[(index + _directionOffset) % 4])
                {
                    case Direction.North:
                        if (!n && !nw && !ne) return (location.x, location.y - 1);
                        break;

                    case Direction.South:
                        if (!s && !sw && !se) return (location.x, location.y + 1);
                        break;

                    case Direction.West:
                        if (!w && !nw && !sw) return (location.x - 1, location.y);
                        break;

                    case Direction.East:
                        if (!e && !ne && !se) return (location.x + 1, location.y);
                        break;
                }
            }

            return location;
        }

        private enum Direction { North, East, South, West }
    }
}