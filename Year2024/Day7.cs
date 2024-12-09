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
            var validEquations = _equations.Select(_ => (_, _IsValidEquation(_.value, _.numbers, _.numbers.Length - 1)));

            var part1 = validEquations.Where(_ => _.Item2).Sum(_ => _.Item1.value);

            yield return $"{part1}";

            var part2 = part1 + validEquations
                .Where(_ => !_.Item2)
                .Select(_ => _.Item1)
                .Where(_ => _IsValidEquation(_.value, _.numbers, _.numbers.Length - 1, tryConcatentation: true))
                .Sum(_ => _.value);

            yield return $"{part2}";

            await Task.CompletedTask;
        }

        private static bool _IsValidEquation(long value, long[] numbers, int index, bool tryConcatentation = false)
        {
            var number = numbers[index];

            if (index == 0) return value == number;

            // try concatenation
            if (tryConcatentation)
            {
                var powerOf10 = (long)Math.Pow(10, Math.Ceiling(Math.Log10(number + 1)));
                if (value % powerOf10 == number)
                {
                    var concatenationIsValid = _IsValidEquation(value / powerOf10, numbers, index - 1, tryConcatentation);
                    if (concatenationIsValid) return true;
                }
            }

            // try multiplication
            if (value % number == 0)
            {
                var multiplicationIsValid = _IsValidEquation(value / number, numbers, index - 1, tryConcatentation);
                if (multiplicationIsValid) return true;
            }

            // try addition
            if (value > number)
            {
                var sumIsValid = _IsValidEquation(value - number, numbers, index - 1, tryConcatentation);
                if (sumIsValid) return true;
            }

            return false;
        }
    }
}
