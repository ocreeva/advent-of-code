using System.Text.RegularExpressions;

namespace Moyba.AdventOfCode.Year2023
{
    using Map = (string targetCategory, SortedList<long, (long end, long offset)> conversions);

    public class Day5 : IPuzzle
    {
        private static readonly Regex _CategoryParser = new Regex(@"^(.*)-to-(.*) map:$", RegexOptions.Compiled);

        private readonly long[] _seeds;
        private readonly IDictionary<string, Map> _almanac = new Dictionary<string, Map>();

        private long _singleSeedLocation;
        private long _multiSeedLocation;

        public Day5(IEnumerable<string> data)
        {
            var lines = data.ToArray();

            _seeds = lines[0].Split(' ').Skip(1).Select(Int64.Parse).ToArray();

            for (var index = 2; index < lines.Length; index++)
            {
                var match = _CategoryParser.Match(lines[index]);
                if (!match.Success) throw new Exception($"Unexpected failure to parse category: {lines[index]}");

                var sourceCategory = match.Groups[1].Value;
                var targetCategory = match.Groups[2].Value;
                var conversions = new SortedList<long, (long, long)>();
                for (index++; index < lines.Length; index++)
                {
                    var line = lines[index];
                    if (String.IsNullOrEmpty(line)) break;

                    // target start, source start, range
                    var parts = line.Split(' ').Select(Int64.Parse).ToArray();
                    conversions.Add(parts[1], (parts[1] + parts[2], parts[0] - parts[1]));
                }

                _almanac.Add(sourceCategory, (targetCategory, conversions));
            }
        }

        public Task ComputeAsync()
        {
            var sourceRanges = new SortedList<long, long>();
            foreach (var seed in _seeds)
            {
                _AddRange(sourceRanges, seed, seed + 1);
            }

            var category = "seed";
            while (_almanac.ContainsKey(category))
            {
                var entry = _almanac[category];
                var targetRanges = new SortedList<long, long>();

                _UtilizeAlmanac(entry.conversions, sourceRanges, targetRanges);

                category = entry.targetCategory;
                sourceRanges = targetRanges;
            }

            _singleSeedLocation = sourceRanges.GetKeyAtIndex(0);

            sourceRanges.Clear();
            for (var seedIndex = 0; seedIndex < _seeds.Length; )
            {
                var start = _seeds[seedIndex++];
                var end = start + _seeds[seedIndex++];
                _AddRange(sourceRanges, start, end);
            }

            category = "seed";
            while (_almanac.ContainsKey(category))
            {
                var entry = _almanac[category];
                var targetRanges = new SortedList<long, long>();

                _UtilizeAlmanac(entry.conversions, sourceRanges, targetRanges);

                category = entry.targetCategory;
                sourceRanges = targetRanges;
            }

            _multiSeedLocation = sourceRanges.GetKeyAtIndex(0);

            return Task.CompletedTask;
        }

        [Solution("662197086")]
        public string SolvePartOne() => $"{_singleSeedLocation}";

        [Solution("52510809")]
        public string SolvePartTwo() => $"{_multiSeedLocation}";

        private static void _UtilizeAlmanac(SortedList<long, (long end, long offset)> conversions, SortedList<long, long> sourceRanges, SortedList<long, long> targetRanges)
        {
            var conversionIndex = 0;
            var conversionStart = conversions.GetKeyAtIndex(conversionIndex);
            (var conversionEnd, var conversionOffset) = conversions[conversionStart];

            for (var sourceIndex = 0; sourceIndex < sourceRanges.Count; sourceIndex++)
            {
                var rangeStart = sourceRanges.GetKeyAtIndex(sourceIndex);
                var rangeEnd = sourceRanges[rangeStart];

                while (rangeStart < rangeEnd)
                {
                    if (rangeStart < conversionStart)
                    {
                        _AddRange(targetRanges, rangeStart, Math.Min(rangeEnd, conversionStart));
                        rangeStart = conversionStart;
                        continue;
                    }

                    if (rangeStart < conversionEnd)
                    {
                        _AddRange(targetRanges, rangeStart + conversionOffset, Math.Min(rangeEnd, conversionEnd) + conversionOffset);
                        rangeStart = conversionEnd;
                        continue;
                    }

                    conversionIndex++;
                    if (conversionIndex == conversions.Count)
                    {
                        conversionStart = Int64.MaxValue;
                        conversionEnd = Int64.MaxValue;
                    }
                    else
                    {
                        conversionStart = conversions.GetKeyAtIndex(conversionIndex);
                        (conversionEnd, conversionOffset) = conversions[conversionStart];
                    }
                }
            }
        }

        private static void _AddRange(SortedList<long, long> ranges, long start, long end)
        {
            int index;
            if (ranges.ContainsKey(start))
            {
                ranges[start] = Math.Max(ranges[start], end);
            }
            else
            {
                ranges.Add(start, end);

                // can we collapse with the previous entry?
                index = ranges.IndexOfKey(start);
                while (index > 0)
                {
                    var prevStart = ranges.GetKeyAtIndex(--index);
                    var prevEnd = ranges[prevStart];
                    if (prevEnd < start) break;

                    if (prevEnd >= end)
                    {
                        ranges.Remove(start);
                        return;
                    }

                    ranges[prevStart] = end;
                    ranges.Remove(start);
                    start = prevStart;
                }
            }

            // can we collapse with the subsequent entries?
            index = ranges.IndexOfKey(start);
            while (index < ranges.Count - 1)
            {
                var nextStart = ranges.GetKeyAtIndex(index + 1);
                if (nextStart > end) break;

                ranges[start] = Math.Max(ranges[start], ranges[nextStart]);
                ranges.Remove(nextStart);
            }
        }
    }
}
