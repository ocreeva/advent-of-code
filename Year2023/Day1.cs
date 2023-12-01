using System.Text.RegularExpressions;

namespace Moyba.AdventOfCode.Year2023
{
    public class Day1(IEnumerable<string> data) : IPuzzle
    {
        private static readonly Regex _LeadNumberParser = new Regex(@"\d|one|two|three|four|five|six|seven|eight|nine", RegexOptions.Compiled);
        private static readonly Regex _TailNumberParser = new Regex(@"\d|one|two|three|four|five|six|seven|eight|nine", RegexOptions.Compiled | RegexOptions.RightToLeft);

        private readonly string[] _data = data.ToArray();

        private int _numericCalibration;
        private int _digitCalibration;

        public Task ComputeAsync()
        {
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

                _numericCalibration += 10 * leadNumericDigit + tailNumericDigit;
                _digitCalibration += 10 * leadDigit + tailDigit;
            }

            return Task.CompletedTask;
        }

        [Solution("54968")]
        public string SolvePartOne() => $"{_numericCalibration}";

        [Solution("54094")]
        public string SolvePartTwo() => $"{_digitCalibration}";

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
