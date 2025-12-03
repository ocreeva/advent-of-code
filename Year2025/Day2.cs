namespace Moyba.AdventOfCode.Year2025
{
    using Range = (long start, long end, int digits);

    public class Day2(string[] _data) : IPuzzle
    {
        private readonly Range[] _ranges = _data
            .Single()
            .Split(',')
            .SelectMany(_ParseRange)
            .OrderBy(_ => _.start)
            .ToArray();

        [PartOne("34826702005")]
        [PartTwo("43287141963")]
        public async IAsyncEnumerable<string?> ComputeAsync()
        {
            var puzzle1 = 0L;
            var puzzle2 = 0L;

            foreach (var range in _ranges)
            {
                var invalidIDs = new HashSet<long>();
                var sequenceLength = range.digits >> 1;
                if (range.digits % 2 == 0)
                {
                    invalidIDs = _FindInvalidIDs(range, sequenceLength--).ToHashSet();
                    puzzle1 += invalidIDs.Sum();
                }

                for ( ; sequenceLength > 0; sequenceLength--)
                {
                    invalidIDs.UnionWith(_FindInvalidIDs(range, sequenceLength));
                }

                puzzle2 += invalidIDs.Sum();
            }

            yield return $"{puzzle1}";

            yield return $"{puzzle2}";

            await Task.CompletedTask;
        }

        private static IEnumerable<long> _FindInvalidIDs(Range range, int sequenceLength)
        {
            if (sequenceLength == 0) yield break;
            if (range.digits % sequenceLength != 0) yield break;

            var sequenceOffset = Math.Pow(10, sequenceLength);
            var step = Enumerable.Range(0, range.digits / sequenceLength).Sum(_ => (long)Math.Pow(sequenceOffset, _));
            var offset = range.start % step;
            if (offset != 0) offset = step - offset;
            for (var invalid = range.start + offset; invalid <= range.end; invalid += step) yield return invalid;
        }

        private static IEnumerable<Range> _ParseRange(string data)
        => _ParseRange(data.Split('-'));

        private static IEnumerable<Range> _ParseRange(string[] data)
        => _ParseRange(Int64.Parse(data[0]), Int64.Parse(data[1]));

        private static IEnumerable<Range> _ParseRange(long start, long end)
        {
            var startDigits = (int)Math.Floor(Math.Log10(start) + 1);
            var endDigits = (int)Math.Floor(Math.Log10(end) + 1);

            if (startDigits != endDigits)
            {
                var split = (long)Math.Pow(10, startDigits);
                yield return (start, split - 1, startDigits);

                start = split;
            }

            yield return (start, end, endDigits);
        }
    }
}
