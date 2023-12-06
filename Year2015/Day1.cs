namespace Moyba.AdventOfCode.Year2015
{
    public class Day1(string[] data) : IPuzzle
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


        [PartOne("138")]
        [PartTwo("1771")]
        public async IAsyncEnumerable<string> ComputeAsync()
        {
            var finalFloor = 0;
            var firstBasementPosition = 0;

            var index = 0;
            while (index < _data.Length)
            {
                finalFloor += _data[index++];
                if (finalFloor < 0)
                {
                    firstBasementPosition = index;
                    break;
                }
            }

            while (index < _data.Length) finalFloor += _data[index++];

            yield return $"{finalFloor}";

            yield return $"{firstBasementPosition}";

            await Task.CompletedTask;
        }
    }
}
