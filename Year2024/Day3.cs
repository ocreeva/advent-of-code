using System.Text.RegularExpressions;

namespace Moyba.AdventOfCode.Year2024
{
    public class Day3(string[] _data) : IPuzzle
    {
        private static readonly Regex _Regex = new Regex(@"(?:(?<instruction>mul)\((\d+),(\d+)\)|(?<instruction>do|don't)\(\))", RegexOptions.Compiled);

        private readonly MatchCollection _matches = _Regex.Matches(String.Join('\n', _data));

        [PartOne("175015740")]
        [PartTwo("112272912")]
        public async IAsyncEnumerable<string?> ComputeAsync()
        {
            var part1 = 0L;
            var part2 = 0L;

            var doPart2 = true;

            foreach (var match in _matches.AsEnumerable())
            {
                var instruction = match.Groups["instruction"].Value;
                switch (instruction)
                {
                    case "mul":
                        var product = Int64.Parse(match.Groups[1].Value) * Int64.Parse(match.Groups[2].Value);
                        part1 += product;
                        if (doPart2) part2 += product;
                        break;

                    case "do":
                        doPart2 = true;
                        break;

                    case "don't":
                        doPart2 = false;
                        break;
                }
            }

            yield return $"{part1}";

            yield return $"{part2}";

            await Task.CompletedTask;
        }
    }
}
