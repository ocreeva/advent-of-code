using System.Text.RegularExpressions;

namespace Moyba.AdventOfCode
{
    internal class Puzzles2022 : PuzzlesBase
    {
        protected override int Year => 2022;

        public override async Task SolveAsync()
        {
            await this.SolveAsync(() => Puzzles2022.Day1, LineDelimited, AsBatchesOfLongs);
            await this.SolveAsync(() => Puzzles2022.Day2);
            await this.SolveAsync(() => Puzzles2022.Day3);
            await this.SolveAsync(() => Puzzles2022.Day4, LineDelimited, new Regex(@"(?<Min1>\d+)\-(?<Max1>\d+),(?<Min2>\d+)-(?<Max2>\d+)"));
        }

        [Answer("507", "897")]
        private static (string, string) Day4(IEnumerable<Match> input)
        {
            long puzzle1 = 0L, puzzle2 = 0L;

            foreach (var match in input)
            {
                var min1 = Int64.Parse(match.Groups["Min1"].Value);
                var max1 = Int64.Parse(match.Groups["Max1"].Value);
                var min2 = Int64.Parse(match.Groups["Min2"].Value);
                var max2 = Int64.Parse(match.Groups["Max2"].Value);

                if (min1 <= min2 && max1 >= max2) puzzle1++;
                else if (min1 >= min2 && max1 <= max2) puzzle1++;

                if (min1 <= max2 && min2 <= max1) puzzle2++;
            }

            return ($"{puzzle1}", $"{puzzle2}");
        }

        [Answer("7863", "2488")]
        private static (string, string) Day3(IEnumerable<string> input)
        {
            long priority1 = 0L, priority2 = 0L;

            var index = 0;
            char[][] rucksacks = new char[3][];
            foreach (var line in input)
            {
                var packSize = line.Length;
                var compartmentSize = packSize >> 1;
                var rucksack = line.ToCharArray();

                var overlap = rucksack[0..compartmentSize].Intersect(rucksack[compartmentSize..packSize]).Single();
                priority1 += (Char.IsLower(overlap) ? (1 + overlap - 'a') : (27 + overlap - 'A'));

                rucksacks[index++] = rucksack;
                if (index == 3)
                {
                    index = 0;
                    var badge = rucksacks[0].Intersect(rucksacks[1]).Intersect(rucksacks[2]).Single();
                    priority2 += (Char.IsLower(badge) ? (1 + badge - 'a') : (27 + badge - 'A'));
                }
            }

            return ($"{priority1}", $"{priority2}");
        }

        [Answer("11150", "8295")]
        private static (string, string) Day2(IEnumerable<string> input)
        {
            // A - rock, B - paper, C - scissors
            // X - rock, Y - paper, C - scissors

            var score1 = 0L;
            var score2 = 0L;
            foreach (var line in input)
            {
                var opponentPlay = line[0] - 'A';
                var myPlay = line[2] - 'X';

                score1 += myPlay + 1;
                if (myPlay == opponentPlay)
                {
                    score1 += 3;
                }
                else if (myPlay == opponentPlay + 1 || myPlay == opponentPlay - 2)
                {
                    score1 += 6;
                }

                // X - lose, Y - draw, Z - win
                score2 += myPlay * 3;
                score2 += ((opponentPlay + myPlay + 2) % 3) + 1;
            }

            return ($"{score1}", $"{score2}");
        }

        [Answer("71924", "210406")]
        private static (string, string) Day1(IEnumerable<IEnumerable<long>> input)
        {
            var top3 = input
                .Select(batch => batch.Sum())
                .OrderByDescending(x => x)
                .Take(3);

            return ($"{top3.First()}", $"{top3.Sum()}");
        }
    }
}
