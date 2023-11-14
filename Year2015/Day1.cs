namespace Moyba.AdventOfCode.Year2015
{
    public class Day1 : SolutionBase<string>
    {
        private int[] _data = Array.Empty<int>();

        protected override string ReadInput(IEnumerable<string> input) => input.First();

        [Expect("138")]
        protected override string SolvePart1()
        {
            return $"{_data.Sum()}";
        }

        [Expect("1771")]
        protected override string SolvePart2()
        {
            var floor = 0;
            for (var index = 0; index < _data.Length; index++)
            {
                floor += _data[index];
                if (floor < 0) return $"{index + 1}";
            }

            throw new Exception("Unexpected end of data with no solution.");
        }

        protected override void TransformData(string data) => _data = data.Select(c => 2 * ('(' - c) + 1).ToArray();
    }
}
