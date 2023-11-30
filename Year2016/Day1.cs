using System.Text.RegularExpressions;

using Coord = (int x, int y);
using Instruction = (char turn, int distance);

namespace Moyba.AdventOfCode.Year2016
{
    public class Day1(IEnumerable<string> data) : IPuzzle
    {
        private static readonly Regex _Parser = new Regex(@"(L|R)(\d+)");

        private readonly Instruction[] _instructions = _Parser.TransformData<Instruction>(data.Single().Split(", ")).ToArray();

        private Coord _position = (0, 0);
        private Coord? _duplicate;

        public Task ComputeAsync()
        {
            var visited = new HashSet<Coord>();
            Coord direction = (0, 1);

            foreach ((var turn, var distance) in _instructions)
            {
                switch (turn)
                {
                    case 'L':
                        direction = (-direction.y, direction.x);
                        break;

                    case 'R':
                        direction = (direction.y, -direction.x);
                        break;
                }

                for (var step = 0; step < distance; step++)
                {
                    _position = (_position.x + direction.x, _position.y + direction.y);

                    if (_duplicate == null && visited.Contains(_position)) _duplicate = _position;
                    visited.Add(_position);
                }
            }

            return Task.CompletedTask;
        }

        [Solution("307")]
        public string SolvePartOne()
        {
            return $"{Math.Abs(_position.x) + Math.Abs(_position.y)}";
        }

        [Solution("165")]
        public string SolvePartTwo()
        {
            if (_duplicate == null) throw new Exception("Duplicate coordinate not found.");

            var location = _duplicate.Value;
            return $"{Math.Abs(location.x) + Math.Abs(location.y)}";
        }
    }
}
