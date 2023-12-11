using System.Text.RegularExpressions;

namespace Moyba.AdventOfCode.Year2022
{
    using Assignment = (int min1, int max1, int min2, int max2);

    public class Day4(string[] _data) : IPuzzle
    {
        private static readonly Regex _Parser = new Regex(@"(\d+)\-(\d+),(\d+)-(\d+)");

        private readonly Assignment[] _assignments = _data.Transform<Assignment>(_Parser).ToArray();

        [PartOne("507")]
        [PartTwo("897")]
        public async IAsyncEnumerable<string?> ComputeAsync()
        {
            // since 'contains' is a subset of 'overlap', we can calculate part 2 first and use it to calculate part 1 on a smaller set
            var overlap = _assignments.Where(_ => _.min1 <= _.max2 && _.min2 <= _.max1).ToArray();

            yield return null;

            var contains = overlap.Where(_ => (_.min1 <= _.min2 && _.max1 >= _.max2) || (_.min2 <= _.min1 && _.max2 >= _.max1)).Count();

            yield return $"{contains}";

            yield return $"{overlap.Length}";

            await Task.CompletedTask;
        }
    }
}


        // [Answer("507", "897")]
        // private static (string, string) Day4(IEnumerable<Match> input)
        // {
        //     long puzzle1 = 0L, puzzle2 = 0L;

        //     foreach (var match in input)
        //     {
        //         var min1 = Int64.Parse(match.Groups["Min1"].Value);
        //         var max1 = Int64.Parse(match.Groups["Max1"].Value);
        //         var min2 = Int64.Parse(match.Groups["Min2"].Value);
        //         var max2 = Int64.Parse(match.Groups["Max2"].Value);

        //         if (min1 <= min2 && max1 >= max2) puzzle1++;
        //         else if (min1 >= min2 && max1 <= max2) puzzle1++;

        //         if (min1 <= max2 && min2 <= max1) puzzle2++;
        //     }

        //     return ($"{puzzle1}", $"{puzzle2}");
        // }
