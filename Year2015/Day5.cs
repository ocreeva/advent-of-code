using System.Text.RegularExpressions;

namespace Moyba.AdventOfCode.Year2015
{
    public class Day5 : SolutionBase
    {
        private static readonly Regex Part1_ThreeVowels = new Regex(@"[aeiou].*[aeiou].*[aeiou]", RegexOptions.Compiled);
        private static readonly Regex Part1_DoubleLetters = new Regex(@"(.)\1", RegexOptions.Compiled);
        private static readonly Regex Part1_BannedStrings = new Regex(@"ab|cd|pq|xy", RegexOptions.Compiled);

        private static readonly Regex Part2_RepeatedPair = new Regex(@"(..).*\1", RegexOptions.Compiled);
        private static readonly Regex Part2_DoubleLettersWithGap = new Regex(@"(.).\1", RegexOptions.Compiled);

        private string[] _data = Array.Empty<string>();

        [Expect("258")]
        protected override string SolvePart1()
        {
            var count = _data
                .Where(d => Part1_ThreeVowels.IsMatch(d))
                .Where(d => Part1_DoubleLetters.IsMatch(d))
                .Where(d => !Part1_BannedStrings.IsMatch(d))
                .Count();
            return $"{count}";
        }

        [Expect("53")]
        protected override string SolvePart2()
        {
            var count = _data
                .Where(d => Part2_RepeatedPair.IsMatch(d))
                .Where(d => Part2_DoubleLettersWithGap.IsMatch(d))
                .Count();
            return $"{count}";
        }

        protected override void TransformData(IEnumerable<string> data) => _data = data.ToArray();
    }
}
