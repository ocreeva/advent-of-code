namespace Moyba.AdventOfCode.Year2015
{
    public class Day3 : SolutionBase<string>
    {
        private static readonly IDictionary<char, (int, int)> TransformLookup = new Dictionary<char, (int, int)> {
            { '<', (-1,  0) },
            { '>', ( 1,  0) },
            { '^', ( 0, -1) },
            { 'v', ( 0,  1) },
        };

        private (int x, int y)[] _data = Array.Empty<(int, int)>();

        protected override string ReadInput(IEnumerable<string> input) => input.First();

        [Expect("2081")]
        protected override string SolvePart1()
        {
            (int x, int y) current = (0, 0);
            var visited = new HashSet<(int, int)> { current };

            foreach (var direction in _data)
            {
                current = (current.x + direction.x, current.y + direction.y);
                visited.Add(current);
            }

            return $"{visited.Count}";
        }

        [Expect("2341")]
        protected override string SolvePart2()
        {
            (int x, int y) current = (0, 0), other = (0, 0);
            var visited = new HashSet<(int, int)> { current };

            foreach (var direction in _data)
            {
                var temp = current;
                current = (other.x + direction.x, other.y + direction.y);
                other = temp;

                visited.Add(current);
            }

            return $"{visited.Count}";
        }

        protected override void TransformData(string data) => _data = data.Select(c => Day3.TransformLookup[c]).ToArray();
    }
}
