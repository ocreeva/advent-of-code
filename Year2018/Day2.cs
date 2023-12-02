namespace Moyba.AdventOfCode.Year2018
{
    public class Day2(IEnumerable<string> data) : IPuzzle
    {
        private readonly char[][] _data = data.Select(_ => _.ToCharArray()).ToArray();

        private int _checksum;
        private string? _prototypeID;

        public Task ComputeAsync()
        {
            var doubleLetters = 0;
            var tripleLetters = 0;
            foreach (var id in _data)
            {
                var letterCounts = id.GroupBy(_ => _).Select(_ => _.Count()).ToHashSet();
                if (letterCounts.Contains(2)) doubleLetters++;
                if (letterCounts.Contains(3)) tripleLetters++;
            }

            _checksum = doubleLetters * tripleLetters;

            // PERF - swap to a trie?
            var desiredLength = _data[0].Length - 1;
            for (var first = 0; first < _data.Length && _prototypeID == null; first++)
            {
                for (var second = first + 1; second < _data.Length && _prototypeID == null; second++)
                {
                    var commonLetters = _data[first].Zip(_data[second]).Where(_ => _.First == _.Second).ToArray();
                    if (commonLetters.Length == desiredLength) _prototypeID = String.Join("", commonLetters.Select(_ => _.First));
                }
            }

            return Task.CompletedTask;
        }

        [Solution("7936")]
        public string SolvePartOne() => $"{_checksum}";

        [Solution("lnfqdscwjyteorambzuchrgpx")]
        public string SolvePartTwo() => _prototypeID ?? String.Empty;
    }
}
