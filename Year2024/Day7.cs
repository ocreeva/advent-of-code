namespace Moyba.AdventOfCode.Year2024
{
    using Equation = (long value, long[] numbers);

    public class Day7(string[] _data) : IPuzzle
    {
        private readonly Equation[] _equations = _data
            .Select(_ => _.Split(' ', StringSplitOptions.RemoveEmptyEntries))
            .Select(_ => (Int64.Parse(_[0].TrimEnd(':')), _[1..].Select(Int64.Parse).ToArray()))
            .ToArray();

        [PartOne("1153997401072")]
        [PartTwo("97902809384118")]
        public async IAsyncEnumerable<string?> ComputeAsync()
        {
            var part1 = 0L;

            foreach (var equation in _equations)
            {
                var partialValues = new HashSet<long> { equation.numbers[0] };
                for (var i = 1; i < equation.numbers.Length; i++)
                {
                    var nextNumber = equation.numbers[i];
                    var nextPartialValues = new HashSet<long>();
                    foreach (var partialValue in partialValues)
                    {
                        var sum = partialValue + nextNumber;
                        if (sum <= equation.value) nextPartialValues.Add(sum);

                        var product = partialValue * nextNumber;
                        if (product <= equation.value) nextPartialValues.Add(product);
                    }

                    partialValues = nextPartialValues;
                }

                if (partialValues.Contains(equation.value)) part1 += equation.value;
            }

            yield return $"{part1}";

            var part2 = 0L;
            foreach (var equation in _equations)
            {
                var partialValues = new HashSet<long> { equation.numbers[0] };
                for (var i = 1; i < equation.numbers.Length; i++)
                {
                    var nextNumber = equation.numbers[i];
                    var nextPowerOf10 = (long)Math.Pow(10, Math.Ceiling(Math.Log10(nextNumber + 1)));
                    var nextPartialValues = new HashSet<long>();
                    foreach (var partialValue in partialValues)
                    {
                        var sum = partialValue + nextNumber;
                        if (sum <= equation.value) nextPartialValues.Add(sum);

                        var product = partialValue * nextNumber;
                        if (product <= equation.value) nextPartialValues.Add(product);

                        var concatenation = partialValue * nextPowerOf10 + nextNumber;
                        if (concatenation <= equation.value) nextPartialValues.Add(concatenation);
                    }

                    partialValues = nextPartialValues;
                }

                if (partialValues.Contains(equation.value)) part2 += equation.value;
            }

            yield return $"{part2}";

            await Task.CompletedTask;
        }
    }
}
