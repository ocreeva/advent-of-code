namespace Moyba.AdventOfCode.Year2023
{
    using ConditionRecord = (string status, int[] groups);

    public class Day12(string[] _data) : IPuzzle
    {
        private readonly ConditionRecord[] _records = _data
            .Select(_ => _.Split(' '))
            .Select<string[], ConditionRecord>(_ => (_[0], _[1].Split(',').Select(Int32.Parse).ToArray()))
            .ToArray();

        [PartOne("7191")]
        [PartTwo("6512849198636")]
        public async IAsyncEnumerable<string?> ComputeAsync()
        {
            var count = _records.Sum(_CountPermutations);

            yield return $"{count}";

            count = _records
                .Select<ConditionRecord, ConditionRecord>(_ => (
                    $"{_.status}?{_.status}?{_.status}?{_.status}?{_.status}",
                    [ .._.groups, .._.groups, .._.groups, .._.groups, .._.groups ]
                ))
                .Sum(_CountPermutations);

            yield return $"{count}";

            await Task.CompletedTask;
        }

        private static long _CountPermutations(ConditionRecord record)
        {
            var length = record.status.Length;
            var minStart = record.groups.Sum() + record.groups.Length;
            var availableSpace = length - minStart + 1;
            var permutations = new long[length + 2];
            permutations[length + 1] = 1L;

            for (var groupIndex = record.groups.Length - 1; groupIndex >= 0; groupIndex--)
            {
                minStart -= record.groups[groupIndex] + 1;
                permutations = _CountPartialPermutations(record, minStart, availableSpace, groupIndex, permutations);
            }

            var minDamaged = record.status.IndexOf('#');
            if (minDamaged == -1) minDamaged = availableSpace;

            return permutations[..(minDamaged+1)].Sum();
        }

        private static long[] _CountPartialPermutations(ConditionRecord record, int minStart, int availableSpace, int groupIndex, long[] nextPermutations)
        {
            var damageLength = record.groups[groupIndex];
            var permutations = new long[nextPermutations.Length];
            for (var space = 0; space <= availableSpace; space++)
            {
                var start = minStart + space;
                var end = start + damageLength;

                if (start > 0 && record.status[start - 1] == '#') continue;
                if (end < record.status.Length && record.status[end] == '#') continue;
                if (record.status[start..end].Contains('.')) continue;

                var availableRange = end + availableSpace - space + 2;
                for (var nextStart = end + 1; nextStart < availableRange; nextStart++)
                {
                    permutations[start] += nextPermutations[nextStart];
                    if (nextStart < record.status.Length && record.status[nextStart] == '#') break;
                }
            }

            return permutations;
        }
    }
}
