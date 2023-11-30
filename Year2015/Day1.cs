namespace Moyba.AdventOfCode.Year2015
{
    public class Day1(IEnumerable<string> data) : IPuzzle
    {
        private readonly int[] _data = data.Single().Select(c => 2 * ('(' - c) + 1).ToArray();

        private int _finalFloor;
        private int? _firstBasementPosition;

        public Task ComputeAsync()
        {
            for (var index = 0; index < _data.Length; index++)
            {
                _finalFloor += _data[index];
                if (!_firstBasementPosition.HasValue && (_finalFloor < 0)) _firstBasementPosition = index + 1;
            }

            return Task.CompletedTask;
        }

        [Solution("138")]
        public string SolvePartOne()
        {
            return $"{_finalFloor}";
        }

        [Solution("1771")]
        public string SolvePartTwo()
        {
            return $"{_firstBasementPosition}";
        }
    }
}
