using Coord = (int x, int y);
using Instruction = (char turn, int distance);

namespace Moyba.AdventOfCode.Year2016
{
    public class Day1(IEnumerable<string> data) : IPuzzle
    {
        private static readonly IDictionary<char, Func<Coord, Coord>> _TurnLookup = new Dictionary<char, Func<Coord, Coord>>
        {
            { 'L', _ => (-_.y, _.x) },
            { 'R', _ => (_.y, -_.x) },
        };

        private readonly Instruction[] _instructions = data
            .Single()
            .Split(", ")
            .Select<string, Instruction>(_ => (_[0], Int32.Parse(_[1..])))
            .ToArray();

        private Coord _position = (0, 0);
        private Coord? _duplicate;

        public Task ComputeAsync()
        {
            var visited = new HashSet<Coord>();
            Coord direction = (0, 1);

            foreach ((var turn, var distance) in _instructions)
            {
                direction = _TurnLookup[turn](direction);

                var step = 0;
                if (!_duplicate.HasValue)
                {
                    while (step++ < distance)
                    {
                        _position = (_position.x + direction.x, _position.y + direction.y);
                        if (visited.Contains(_position))
                        {
                            _duplicate = _position;
                            break;
                        }

                        visited.Add(_position);
                    }
                }

                while (step++ < distance) _position = (_position.x + direction.x, _position.y + direction.y);
            }

            return Task.CompletedTask;
        }

        [Solution("307")]
        public string PartOne => $"{Math.Abs(_position.x) + Math.Abs(_position.y)}";

        [Solution("165")]
        public string PartTwo
        {
            get
            {
                if (_duplicate == null) throw new Exception("Duplicate coordinate not found.");

                var location = _duplicate.Value;
                return $"{Math.Abs(location.x) + Math.Abs(location.y)}";
            }
        }
    }
}
