using System.Text.RegularExpressions;

namespace Moyba.AdventOfCode.Year2023
{
    public class Day1(string[] _data) : IPuzzle
    {
        private static readonly Regex _LeadNumberParser = new Regex(@"\d|one|two|three|four|five|six|seven|eight|nine", RegexOptions.Compiled);
        private static readonly Regex _TailNumberParser = new Regex(@"\d|one|two|three|four|five|six|seven|eight|nine", RegexOptions.Compiled | RegexOptions.RightToLeft);

        [PartOne("54968")]
        [PartTwo("54094")]
        public async IAsyncEnumerable<string> ComputeAsync()
        {
            var numericCalibration = 0;
            var digitCalibration = 0;

            foreach (var line in _data)
            {
                var leadNumber = _LeadNumberParser.Match(line).Value;
                (var leadDigit, var leadDigitIsNumeric) = _DigitLookup(leadNumber);
                var leadNumericDigit = leadDigitIsNumeric
                    ? leadDigit
                    : _DigitLookup(line.Where(Char.IsDigit).First());

                var tailNumber = _TailNumberParser.Match(line).Value;
                (var tailDigit, var tailDigitIsNumeric) = _DigitLookup(tailNumber);
                var tailNumericDigit = tailDigitIsNumeric
                    ? tailDigit
                    : _DigitLookup(line.Reverse().Where(Char.IsDigit).First());

                numericCalibration += 10 * leadNumericDigit + tailNumericDigit;
                digitCalibration += 10 * leadDigit + tailDigit;
            }

            yield return $"{numericCalibration}";

            yield return $"{digitCalibration}";

            await Task.CompletedTask;
        }

        private static (int digit, bool isNumeric) _DigitLookup(string s) => s switch
        {
            "one" => (1, false),
            "two" => (2, false),
            "three" => (3, false),
            "four" => (4, false),
            "five" => (5, false),
            "six" => (6, false),
            "seven" => (7, false),
            "eight" => (8, false),
            "nine" => (9, false),
            _ => (_DigitLookup(s[0]), true)
        };

        private static int _DigitLookup(char c) => c - '0';
    }
}
