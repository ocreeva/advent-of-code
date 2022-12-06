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
            await this.SolveAsync(() => Puzzles2022.Day5);
            await this.SolveAsync(() => Puzzles2022.Day6, LineDelimited, AsString);
        }

        [Answer("1356", "2564")]
        private static (string, string) Day6(string input)
        {
            long puzzle1 = 0L, puzzle2 = 0L;

            for (var index = 0; index < input.Length - 3; index++)
            {
                if (puzzle1 == 0)
                {
                    var distinctCount = input[index..(index+4)].Distinct().Count();
                    if (distinctCount == 4) puzzle1 = index + 4;
                }

                var distinctCount2 = input[index..(index+14)].Distinct().Count();
                if (distinctCount2 == 14) puzzle2 = index + 14;

                if (puzzle2 != 0) break;
            }

            return ($"{puzzle1}", $"{puzzle2}");
        }

        [Answer("HBTMTBSDC", "PQTJRSHWS")]
        private static (string, string) Day5(IEnumerable<string> input)
        {
            var instructionRegex = new Regex(@"^move (?<Count>\d+) from (?<Source>\d+) to (?<Destination>\d+)$");

            Stack<char>[] stacks1 = new Stack<char>[0], stacks2 = new Stack<char>[0], stacks = new Stack<char>[9];
            for (var index = 0; index < stacks.Length; index++) stacks[index] = new Stack<char>();

            var isParsingStacks = true;
            foreach (var line in input)
            {
                if (String.IsNullOrWhiteSpace(line))
                {
                    stacks1 = stacks.Select(stack => new Stack<char>(stack)).ToArray();
                    stacks2 = stacks.Select(stack => new Stack<char>(stack)).ToArray();
                    isParsingStacks = false;
                    continue;
                }

                if (isParsingStacks)
                {
                    if (!line.Contains('[')) continue;

                    for (var index = 1; index < line.Length; index += 4)
                    {
                        var value = line[index];
                        if (!Char.IsLetter(value)) continue;
                        stacks[(index - 1) / 4].Push(value);
                    }

                    continue;
                }

                var match = instructionRegex.Match(line);
                if (!match.Success) throw new Exception($"Unexpected failed to parse input: {line}");

                var count = Int32.Parse(match.Groups["Count"].Value);
                var source = Int32.Parse(match.Groups["Source"].Value) - 1;
                var destination = Int32.Parse(match.Groups["Destination"].Value) - 1;

                for (var x = 0; x < count; x++)
                {
                    stacks1[destination].Push(stacks1[source].Pop());
                }

                var tempStack = new Stack<char>();
                for (var x = 0; x < count; x++) tempStack.Push(stacks2[source].Pop());
                for (var x = 0; x < count; x++) stacks2[destination].Push(tempStack.Pop());
            }

            var puzzle1 = String.Join("", stacks1.Select(stack => stack.Peek()));
            var puzzle2 = String.Join("", stacks2.Select(stack => stack.Peek()));

            return (puzzle1, puzzle2);
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
