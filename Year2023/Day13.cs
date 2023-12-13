namespace Moyba.AdventOfCode.Year2023
{
    using Pattern = (int height, int width, string[] data);

    public class Day13(string[] _data) : IPuzzle
    {
        private readonly Pattern[] _patterns = _data
            .Cluster()
            .Select(_ => _.ToArray())
            .Select<string[], Pattern>(_ => (_.Length, _[0].Length, _))
            .ToArray();

        [PartOne("37113")]
        [PartTwo("30449")]
        public async IAsyncEnumerable<string?> ComputeAsync()
        {
            var summary = 0;
            foreach (var pattern in _patterns)
            {
                if (_TryFindVerticalMirror(pattern, 0, out var column)) summary += column;
                else if (_TryFindHorizontalMirror(pattern, 0, out var row)) summary += 100 * row;
            }

            yield return $"{summary}";

            summary = 0;
            foreach (var pattern in _patterns)
            {
                if (_TryFindVerticalMirror(pattern, 1, out var column)) summary += column;
                else if (_TryFindHorizontalMirror(pattern, 1, out var row)) summary += 100 * row;
            }

            yield return $"{summary}";

            await Task.CompletedTask;
        }

        private static bool _TryFindHorizontalMirror(Pattern pattern, int smudges, out int row)
        {
            for (row = 1; row < pattern.height; row++)
            {
                var count = 0;
                for (int over = row, under = row - 1; over < pattern.height && under >= 0 && count <= smudges; over++, under--)
                {
                    for (var x = 0; x < pattern.width && count <= smudges; x++)
                    {
                        if (pattern.data[over][x] != pattern.data[under][x]) count++;
                    }
                }

                if (count == smudges) return true;
            }

            return false;
        }

        private static bool _TryFindVerticalMirror(Pattern pattern, int smudges, out int column)
        {
            for (column = 1; column < pattern.width; column++)
            {
                var count = 0;
                for (int over = column, under = column - 1; over < pattern.width && under >= 0 && count <= smudges; over++, under--)
                {
                    for (var y = 0; y < pattern.height && count <= smudges; y++)
                    {
                        if (pattern.data[y][over] != pattern.data[y][under]) count++;
                    }
                }

                if (count == smudges) return true;
            }

            return false;
        }
    }
}
