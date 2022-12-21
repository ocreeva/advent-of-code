namespace Moyba.AdventOfCode.Year2018
{
    public class Day2 : SolutionBase
    {
        private char[][] _data = Array.Empty<char[]>();

        [Expect("7936")]
        protected override string SolvePart1()
        {
            var doubleLetter = 0;
            var tripleLetter = 0;

            foreach (var id in _data)
            {
                var letterCounts = id.GroupBy(c => c).Select(g => g.Count()).ToArray();
                if (letterCounts.Any(c => c == 2)) doubleLetter++;
                if (letterCounts.Any(c => c == 3)) tripleLetter++;
            }

            return $"{doubleLetter * tripleLetter}";
        }

        [Expect("lnfqdscwjyteorambzuchrgpx")]
        protected override string SolvePart2()
        {
            string? result = null;
            var desiredLength = _data[0].Length - 1;
            for (var firstIndex = 0; firstIndex < _data.Length && result == null; firstIndex++)
            {
                for (var secondIndex = firstIndex + 1; secondIndex < _data.Length && result == null; secondIndex++)
                {
                    var commonLetters = _data[firstIndex].Zip(_data[secondIndex]).Where(letters => letters.First == letters.Second).Select(letters => letters.First).ToArray();
                    if (commonLetters.Length == desiredLength) result = String.Join("", commonLetters);
                }
            }

            return result ?? String.Empty;
        }

        protected override void TransformData(IEnumerable<string> data) => _data = data.Select(d => d.ToCharArray()).ToArray();
    }
}