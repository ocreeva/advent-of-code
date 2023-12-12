namespace Moyba.AdventOfCode.Year2023
{
    using ConditionRecord = (int length, HashSet<int> operational, HashSet<int> damaged, int[] damagedGroups);

    public class Day12(string[] _data) : IPuzzle
    {
        private readonly ConditionRecord[] _records = _data
            .Select(_ => _.Split(' '))
            .Select<string[], ConditionRecord>(_ => (
                _[0].Length,
                Enumerable.Range(0, _[0].Length)
                    .Where(index => _[0][index] == '.')
                    .ToHashSet(),
                Enumerable.Range(0, _[0].Length)
                    .Where(index => _[0][index] == '#')
                    .ToHashSet(),
                _[1].Split(',').Select(Int32.Parse).ToArray()
            ))
            .ToArray();

        [PartOne("7191")]
        [PartTwo("6512849198636")]
        public async IAsyncEnumerable<string?> ComputeAsync()
        {
            var count = 0L;
            foreach (var record in _records)
            {
                var availableSpace = record.length - record.damagedGroups.Sum() - record.damagedGroups.Length + 1;
                var minStart = record.length - availableSpace + 1;
                IDictionary<int, long> permutations = new Dictionary<int, long> { { record.length + 1, 1L } };
                for (var groupIndex = record.damagedGroups.Length - 1; groupIndex >= 0; groupIndex--)
                {
                    minStart -= record.damagedGroups[groupIndex] + 1;
                    permutations = _CountPartialPermutations(record, minStart, availableSpace, groupIndex, permutations);
                }

                var minDamaged = record.damaged.Any() ? record.damaged.Min() : Int32.MaxValue;
                count += permutations.Where(_ => _.Key <= minDamaged).Sum(_ => _.Value);
            }
            yield return $"{count}";

            count = 0L;
            foreach (var record in _records)
            {
                var offset = record.length + 1;
                var length = record.length + 4 * offset;
                var operational = new HashSet<int>();
                var damaged = new HashSet<int>();
                var numGroups = record.damagedGroups.Length;
                var damagedGroups = new int[numGroups * 5];
                for (var fold = 0; fold < 5; fold++)
                {
                    foreach (var value in record.operational) operational.Add(value + offset * fold);
                    foreach (var value in record.damaged) damaged.Add(value + offset * fold);
                    for (var index = 0; index < numGroups; index++) damagedGroups[index + fold * numGroups] = record.damagedGroups[index];
                }

                ConditionRecord newRecord = (length, operational, damaged, damagedGroups);
                var availableSpace = length - damagedGroups.Sum() - damagedGroups.Length + 1;
                var minStart = length - availableSpace + 1;
                IDictionary<int, long> permutations = new Dictionary<int, long>
                {
                    { length + 1, 1L }
                };
                for (var groupIndex = damagedGroups.Length - 1; groupIndex >= 0; groupIndex--)
                {
                    minStart -= damagedGroups[groupIndex] + 1;
                    permutations = _CountPartialPermutations(newRecord, minStart, availableSpace, groupIndex, permutations);
                }

                var minDamaged = record.damaged.Any() ? record.damaged.Min() : Int32.MaxValue;
                count += permutations.Where(_ => _.Key <= minDamaged).Sum(_ => _.Value);
            }

            yield return $"{count}";

            await Task.CompletedTask;
        }

        private static IDictionary<int, long> _CountPartialPermutations(ConditionRecord record, int minStart, int availableSpace, int groupIndex, IDictionary<int, long> nextPermutations)
        {
            var groupLength = record.damagedGroups[groupIndex];
            var permutations = new Dictionary<int, long>();
            for (var space = 0; space <= availableSpace; space++)
            {
                var start = minStart + space;
                var end = start + groupLength;

                if (record.damaged.Contains(start - 1)) continue;
                if (record.damaged.Contains(end)) continue;
                if (Enumerable.Range(start, groupLength).Any(record.operational.Contains)) continue;

                var count = 0L;
                foreach (var nextStart in Enumerable.Range(end + 1, availableSpace - space + 1))
                {
                    if (nextPermutations.TryGetValue(nextStart, out var value)) count += value;
                    if (record.damaged.Contains(nextStart)) break;
                }

                if (count > 0) permutations.Add(start, count);
            }

            return permutations;
        }
    }
}
