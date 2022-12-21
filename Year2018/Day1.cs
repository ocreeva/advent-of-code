namespace Moyba.AdventOfCode.Year2018
{
    public class Day1 : SolutionBase<IEnumerable<long>>
    {
        private long[] _data = Array.Empty<long>();

        protected override IEnumerable<long> ReadInput(IEnumerable<string> input) => input.Select(Int64.Parse);

        [Expect("547")]
        protected override string SolvePart1() => $"{_data.Sum()}";

        [Expect("76414")]
        protected override string SolvePart2()
        {
            var frequency = 0L;
            var frequencies = new HashSet<long>();

            for (var index = 0; !frequencies.Contains(frequency); frequency += _data[index++ % _data.Length]) frequencies.Add(frequency);

            return $"{frequency}";
        }

        protected override void TransformData(IEnumerable<long> data) => _data = data.ToArray();
    }
}
