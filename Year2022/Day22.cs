using System.Text.RegularExpressions;

namespace Moyba.AdventOfCode.Year2022
{
    public class Day22 : SolutionBase
    {
        private const int _CubeDimension = 50;

        private HashSet<(int x, int y)> _all = new HashSet<(int x, int y)>();
        private HashSet<(int x, int y)> _spaces = new HashSet<(int x, int y)>();
        private HashSet<(int x, int y)> _walls = new HashSet<(int x, int y)>();
        private string[] _instructions = Array.Empty<string>();

        [Expect("93226")]
        protected override string SolvePart1()
        {
            (int x, int y) position = _spaces.Where(s => s.y == 1).MinBy(s => s.x);
            (int x, int y) facing = (1, 0);

            foreach (var instruction in _instructions)
            {
                switch (instruction)
                {
                    case "R":
                        facing = (-facing.y, facing.x);
                        break;

                    case "L":
                        facing = (facing.y, -facing.x);
                        break;

                    default:
                        var steps = Int32.Parse(instruction);
                        for (var iteration = 0; iteration < steps; iteration++)
                        {
                            (int x, int y) next = (position.x + facing.x, position.y + facing.y);
                            if (!_all.Contains(next)) this.UpdatePositionAndFacingForWrapAround(position, ref next, ref facing);
                            if (_walls.Contains(next)) break;
                            if (_spaces.Contains(next))
                            {
                                position = next;
                                continue;
                            }

                            throw new Exception("Failure!");
                        }
                        break;
                }
            }

            var password = Day22.CalculatePassword(position, facing);
            return $"{password}";
        }

        [Expect("37415")]
        protected override string SolvePart2()
        {
            (int x, int y) position = _spaces.Where(s => s.y == 1).MinBy(s => s.x);
            (int x, int y) facing = (1, 0);

            //Console.WriteLine();
            //Console.WriteLine($"Start: ({position.x}, {position.y}) [{facing.x}, {facing.y}]");
            foreach (var instruction in _instructions)
            {
                switch (instruction)
                {
                    case "R":
                        facing = (-facing.y, facing.x);
                        break;

                    case "L":
                        facing = (facing.y, -facing.x);
                        break;

                    default:
                        var steps = Int32.Parse(instruction);
                        for (var iteration = 0; iteration < steps; iteration++)
                        {
                            (int x, int y) next = (position.x + facing.x, position.y + facing.y);
                            if (!_all.Contains(next)) this.UpdatePositionAndFacingForCube(position, ref next, ref facing);

                            if (_walls.Contains(next)) break;
                            if (_spaces.Contains(next))
                            {
                                position = next;
                                continue;
                            }

                            throw new Exception("Failure!");
                        }
                        break;
                }
                //Console.WriteLine($"{instruction}: ({position.x}, {position.y}) [{facing.x}, {facing.y}]");
            }

            var password = Day22.CalculatePassword(position, facing);
            return $"{password}";
        }

        protected override void TransformData(IEnumerable<string> data)
        {
            var y = 0;
            var shouldParseInstructions = false;
            foreach (var line in data)
            {
                if (String.IsNullOrWhiteSpace(line))
                {
                    shouldParseInstructions = true;
                    continue;
                }

                if (shouldParseInstructions)
                {
                    _instructions = Regex.Matches(line, @"(\d+|R|L)").Select(match => match.Groups[1].Value).ToArray();
                    continue;
                }

                y++;
                for (var x = 0; x < line.Length; x++)
                {
                    switch (line[x])
                    {
                        case '.':
                            _all.Add((x + 1, y));
                            _spaces.Add((x + 1, y));
                            break;

                        case '#':
                            _all.Add((x + 1, y));
                            _walls.Add((x + 1, y));
                            break;
                    }
                }
            }
        }

        private static int CalculatePassword((int x, int y) position, (int x, int y) facing)
        {
            return 1000 * position.y + 4 * position.x - (facing.x == 0 ? 0 : facing.x - 1) - (facing.y == 0 ? 0 : facing.y - 2);
        }

        private void UpdatePositionAndFacingForWrapAround((int x, int y) position, ref (int x, int y) next, ref (int x, int y) facing)
        {
            switch (facing)
            {
                case (0, 1):
                    next = _all.Where(s => s.x == position.x).MinBy(s => s.y);
                    break;

                case (0, -1):
                    next = _all.Where(s => s.x == position.x).MaxBy(s => s.y);
                    break;

                case (1, 0):
                    next = _all.Where(s => s.y == position.y).MinBy(s => s.x);
                    break;

                case (-1, 0):
                    next = _all.Where(s => s.y == position.y).MaxBy(s => s.x);
                    break;
            }
        }

        private int GetFace((int x, int y) position)
        {
            if (position.x > 100) return 2;
            if (position.y > 150) return 4;
            if (position.x <= 50) return 5;
            if (position.y <= 50) return 1;
            if (position.y > 100) return 6;
            return 3;
        }

        private void AssertFace((int x, int y) position, (int x, int y) next, int expectedPositionFace, int expectedNextFace)
        {
            var actualPositionFace = this.GetFace(position);
            if (actualPositionFace != expectedPositionFace)
            {
                throw new Exception("Failure!");
            }

            var actualNextFace = this.GetFace(next);
            if (actualNextFace != expectedNextFace)
            {
                throw new Exception("Failure");
            }
        }

        private void UpdatePositionAndFacingForCube((int x, int y) position, ref (int x, int y) next, ref (int x, int y) facing)
        {
            //  12  - Ugh! Hardcoding for my puzzle input, rather than trying to computationally
            //  3   - determine where cube faces are.
            // 56
            // 4

            switch (facing)
            {
                case (1, 0):
                    if (next.y <= 50)
                    {
                        // right from 2 to left on 6
                        next = (100, 151 - next.y);
                        if (_spaces.Contains(next)) facing = (-1, 0);
                        AssertFace(position, next, 2, 6);
                    }
                    else if (next.y <= 100)
                    {
                        // right from 3 to up on 2
                        next = (next.y + 50, 50);
                        if (_spaces.Contains(next)) facing = (0, -1);
                        AssertFace(position, next, 3, 2);
                    }
                    else if (next.y <= 150)
                    {
                        // right from 6 to left on 2
                        next = (150, 151 - next.y);
                        if (_spaces.Contains(next)) facing = (-1, 0);
                        AssertFace(position, next, 6, 2);
                    }
                    else
                    {
                        // right from 4 to up on 6
                        next = (next.y - 100, 150);
                        if (_spaces.Contains(next)) facing = (0, -1);
                        AssertFace(position, next, 4, 6);
                    }
                    break;

                case (-1, 0):
                     if (next.y <= 50)
                    {
                        // left from 1 to right on 5
                        next = (1, 151 - next.y);
                        if (_spaces.Contains(next)) facing = (1, 0);
                        AssertFace(position, next, 1, 5);
                    }
                    else if (next.y <= 100)
                    {
                        // left from 3 to down on 5
                        next = (next.y - 50, 101);
                        if (_spaces.Contains(next)) facing = (0, 1);
                        AssertFace(position, next, 3, 5);
                    }
                    else if (next.y <= 150)
                    {
                        // left from 5 to right on 1
                        next = (51, 151 - next.y);
                        if (_spaces.Contains(next)) facing = (1, 0);
                        AssertFace(position, next, 5, 1);
                    }
                    else
                    {
                        // left from 4 to down on 1
                        next = (next.y - 100, 1);
                        if (_spaces.Contains(next)) facing = (0, 1);
                        AssertFace(position, next, 4, 1);
                    }
                   break;

                case (0, 1):
                    if (next.x <= 50)
                    {
                        // down from 4 to down on 2
                        next = (next.x + 100, 1);
                        AssertFace(position, next, 4, 2);
                    }
                    else if (next.x <= 100)
                    {
                        // down from 6 to left on 4
                        next = (50, next.x + 100);
                        if (_spaces.Contains(next)) facing = (-1, 0);
                        AssertFace(position, next, 6, 4);
                    }
                    else
                    {
                        // down from 2 to left on 3
                        next = (100, next.x - 50);
                        if (_spaces.Contains(next)) facing = (-1, 0);
                        AssertFace(position, next, 2, 3);
                    }
                    break;

                case (0, -1):
                    if (next.x <= 50)
                    {
                        // up from 5 to right on 3
                        next = (51, next.x + 50);
                        if (_spaces.Contains(next)) facing = (1, 0);
                        AssertFace(position, next, 5, 3);
                    }
                    else if (next.x <= 100)
                    {
                        // up from 1 to right on 4
                        next = (1, next.x + 100);
                        if (_spaces.Contains(next)) facing = (1, 0);
                        AssertFace(position, next, 1, 4);
                    }
                    else
                    {
                        // up from 2 to up on 4
                        next = (next.x - 100, 200);
                        AssertFace(position, next, 2, 4);
                    }
                    break;
            }

            if (!_all.Contains(next))
            {
                throw new Exception("Failure!");
            }
        }
    }
}