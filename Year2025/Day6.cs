namespace Moyba.AdventOfCode.Year2025
{
    using Problem = (char operation, char[][] data);

    public class Day6 : IPuzzle
    {
        private readonly Problem[] _problems;

        public Day6(string[] data)
        {
            var problemStartIndices = data[^1].Select((operation, index) => (operation, index)).Where(_ => _.operation != ' ').ToArray();
            var problemEndIndices = problemStartIndices[1..].Select(_ => _.index - 1).Append(data[^1].Length);
            var problemIndices = problemStartIndices.Zip(problemEndIndices, (si, ei) => (si.operation, start: si.index, end: ei)).ToArray();

            _problems = problemIndices.Select(index => (
                index.operation,
                data[..^1].Select(_ => _.ToCharArray()[index.start..index.end]).ToArray()
            )).ToArray();
        }

        [PartOne("4951502530386")]
        [PartTwo("8486156119946")]
        public async IAsyncEnumerable<string?> ComputeAsync()
        {
            var puzzle1 = _problems.Sum(_ => _.operation switch
            {
                '+' => _ParseHorizontalNumbers(_.data).Sum(),
                '*' => _ParseHorizontalNumbers(_.data).Aggregate(1L, (product, number) => product * number),
                _ => throw new Exception($"Unexpected operator: {_.operation}")
            });

            yield return $"{puzzle1}";

            var puzzle2 = _problems.Sum(_ => _.operation switch
            {
                '+' => _ParseVerticalNumbers(_.data).Sum(),
                '*' => _ParseVerticalNumbers(_.data).Aggregate(1L, (product, number) => product * number),
                _ => throw new Exception($"Unexpected operator: {_.operation}")
            });

            yield return $"{puzzle2}";

            await Task.CompletedTask;
        }

        private static IEnumerable<long> _ParseHorizontalNumbers(char[][] data)
        {
            var xMax = data[0].Length;
            var yMax = data.Length;

            for (var y = 0; y < yMax; y++)
            {
                var number = 0L;
                for (var x = 0; x < xMax; x++)
                {
                    var value = data[y][x];
                    if (value == ' ') continue;

                    number = number * 10 + (value - '0');
                }

                yield return number;
            }
        }

        private static IEnumerable<long> _ParseVerticalNumbers(char[][] data)
        {
            var xMax = data[0].Length;
            var yMax = data.Length;

            for (var x = 0; x < xMax; x++)
            {
                var number = 0L;
                for (var y = 0; y < yMax; y++)
                {
                    var value = data[y][x];
                    if (value == ' ') continue;

                    number = number * 10 + (value - '0');
                }

                yield return number;
            }
        }
    }
}
