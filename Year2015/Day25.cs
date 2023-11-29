using System.Text.RegularExpressions;

namespace Moyba.AdventOfCode.Year2015
{
    public class Day25 : SolutionBase<string>
    {
        private static readonly Regex Parser = new Regex(@"^To continue, please consult the code grid in the manual\.  Enter the code at row (?<row>\d+), column (?<column>\d+)\.$", RegexOptions.Compiled);

        private int _row;
        private int _column;

        [Expect("9132360")]
        protected override string SolvePart1()
        {
            var diagonal = _row + _column;
            var iterations = diagonal * (diagonal - 3) / 2 + 1 + _column;

            long value = 20_151_125;
            for (var index = 1; index < iterations; index++)
            {
                value = value * 252_533 % 33_554_393;
            }

            return $"{value}";
        }

        [Expect("Merry Christmas!")]
        protected override string SolvePart2()
        {
            return "Merry Christmas!";
        }

        protected override string ReadInput(IEnumerable<string> input) => input.First();
        protected override void TransformData(string data)
        {
            var match = Parser.Match(data);
            if (!match.Success) throw new Exception($"Unhandled input: {data}");

            _row = Int32.Parse(match.Groups["row"].Value);
            _column = Int32.Parse(match.Groups["column"].Value);
        }
    }
}
