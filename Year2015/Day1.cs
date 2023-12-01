namespace Moyba.AdventOfCode.Year2015
{
    public class Day1(IEnumerable<string> data) : IPuzzle
    {
        private static readonly IDictionary<char, int> _DataLookup = new Dictionary<char, int>
        {
            { '(', 1 },
            { ')', -1 },
        };

        private readonly int[] _data = data
            .Single()
            .Select(_ => _DataLookup[_])
            .ToArray();

        private int _finalFloor;
        private int? _firstBasementPosition;

        public Task ComputeAsync()
        {
            var index = 0;
            while (index < _data.Length)
            {
                _finalFloor += _data[index++];
                if (_finalFloor < 0)
                {
                    _firstBasementPosition = index;
                    break;
                }
            }

            while (index < _data.Length) _finalFloor += _data[index++];

            return Task.CompletedTask;
        }

        [Solution("138")]
        public string SolvePartOne() => $"{_finalFloor}";

        [Solution("1771")]
        public string SolvePartTwo() => $"{_firstBasementPosition}";
    }
}
