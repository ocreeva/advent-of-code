namespace Moyba.AdventOfCode.Year2024
{
    public class Day2(string[] _data) : IPuzzle
    {
        private readonly long[][] _reports = _data
            .Select(r => r.Split(' ').Select(Int64.Parse).ToArray())
            .ToArray();

        [PartOne("213")]
        [PartTwo("285")]
        public async IAsyncEnumerable<string?> ComputeAsync()
        {
            var safeCount = 0;
            var unsafeIndicesByReport = new Dictionary<int, int[]>();
            for (var r = 0; r < _reports.Length; r++)
            {
                var unsafeIndices = this.FindUnsafeIndices(r);
                if (unsafeIndices.Length == 0)
                {
                    safeCount++;
                }
                else
                {
                    unsafeIndicesByReport[r] = unsafeIndices;
                }
            }

            yield return $"{safeCount}";

            var dampenedCount = safeCount;
            foreach (var r in unsafeIndicesByReport.Keys)
            {
                var unsafeIndices = unsafeIndicesByReport[r];
                foreach (var unsafeIndex in unsafeIndices)
                {
                    if (unsafeIndex < 0) continue;
                    if (unsafeIndex >= _reports[r].Length) continue;

                    var dampenedIndices = this.FindUnsafeIndices(r, unsafeIndex);
                    if (dampenedIndices.Length == 0)
                    {
                        dampenedCount++;
                        break;
                    }
                }
            }

            yield return $"{dampenedCount}";

            await Task.CompletedTask;
        }

        private int[] FindUnsafeIndices(int reportIndex, int skipIndex = -1)
        {
            var report = _reports[reportIndex];

            if (skipIndex >= 0) report = report
                .Where((_, i) => i != skipIndex)
                .ToArray();

            var differences = report[..^1]
                .Zip(report[1..], (x, y) => x - y)
                .ToArray();

            for (var i = 0; i < differences.Length; i++)
            {
                var difference = differences[i];

                if (difference == 0) return [ i ];

                if (Math.Abs(difference) > 3) return [ i, i + 1 ];

                if (i == 0) continue;

                if (Math.Sign(difference) != Math.Sign(differences[i - 1])) return [ i - 1, i, i + 1 ];
            }

            return Array.Empty<int>();
        }
    }
}
