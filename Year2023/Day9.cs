namespace Moyba.AdventOfCode.Year2023
{
    public class Day9(string[] _data) : IPuzzle
    {
        private readonly long[][] _sequences = _data
            .Select(_ => _.Split(' ').Select(Int64.Parse).ToArray())
            .ToArray();

        [PartOne("1916822650")]
        [PartTwo("966")]
        public async IAsyncEnumerable<string?> ComputeAsync()
        {
            var extensions = _sequences.Select(_FindNextAndPreviousInSequence).ToArray();

            yield return null;

            var sumOfNext = extensions.Select(_ => _.next).Sum();

            yield return $"{sumOfNext}";

            var sumOfPrev = extensions.Select(_ => _.prev).Sum();

            yield return $"{sumOfPrev}";

            await Task.CompletedTask;
        }

        private static (long prev, long next) _FindNextAndPreviousInSequence(long[] sequence)
        {
            var length = sequence.Length - 1;
            var differences = new long[length];
            for (var index = 0; index < length; index++) differences[index] = sequence[index + 1] - sequence[index];

            if (differences.All(s => s == 0)) return (sequence[0], sequence[length]);

            (var prev, var next) = _FindNextAndPreviousInSequence(differences);
            return (sequence[0] - prev, sequence[length] + next);
        }
    }
}
