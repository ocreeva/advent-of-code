namespace Moyba.AdventOfCode.Year2025
{
    using System.Text.RegularExpressions;
    using Range = (long start, long end);

    public class Day5 : IPuzzle
    {
        private static readonly Regex _RangeParser = new Regex(@"^(\d+)-(\d+)$", RegexOptions.Compiled);

        private readonly List<Range> _freshIDs;
        private readonly long[] _productIDs;

        public Day5(string[] _data)
        {
            var clusteredData = _data.Cluster().ToArray();

            _freshIDs = clusteredData[0].Transform<Range>(_RangeParser).ToList();
            for (var currentIndex = 0; currentIndex < _freshIDs.Count; currentIndex++)
            {
                var currentRange = _freshIDs[currentIndex];
                for (var testIndex = currentIndex + 1; testIndex < _freshIDs.Count; testIndex++)
                {
                    var testRange = _freshIDs[testIndex];
                    if (currentRange.start > testRange.end) continue;
                    if (currentRange.end < testRange.start) continue;

                    _freshIDs.RemoveAt(testIndex);
                    
                    currentRange = (Math.Min(currentRange.start, testRange.start), Math.Max(currentRange.end, testRange.end));
                    _freshIDs[currentIndex] = currentRange;

                    testIndex = currentIndex + 1;
                }
            }

            _productIDs = clusteredData[1].Select(Int64.Parse).ToArray();
        }

        [PartOne("885")]
        [PartTwo("348115621205535")]
        public async IAsyncEnumerable<string?> ComputeAsync()
        {
            var puzzle1 = 0;
            foreach (var id in _productIDs)
            {
                if (_freshIDs.Any(_ => _.start <= id && _.end >= id)) puzzle1++;
            }

            yield return $"{puzzle1}";

            var puzzle2 = _freshIDs.Sum(_ => _.end - _.start + 1);

            yield return $"{puzzle2}";

            await Task.CompletedTask;
        }
    }
}
