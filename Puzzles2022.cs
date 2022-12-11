using System;
using System.Linq.Expressions;
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
            await this.SolveAsync(() => Puzzles2022.Day7);
            await this.SolveAsync(() => Puzzles2022.Day8);
            await this.SolveAsync(() => Puzzles2022.Day9);
            await this.SolveAsync(() => Puzzles2022.Day10);
            await this.SolveAsync(() => Puzzles2022.Day11, LineDelimited, AsBatchesOfStrings);
        }

        private class Monkey
        {
            private readonly Queue<long> _items1 = new Queue<long>();
            private readonly Queue<long> _items2 = new Queue<long>();

            public Queue<long> Items1 => _items1;
            public Queue<long> Items2 => _items2;
            public Func<long, long> Operation1 { get; set; }
            public Func<long, long> Operation2 { get; set; }
            public long TestDivisor { get; set; }
            public int TrueMonkey { get; set; }
            public int FalseMonkey { get; set; }
            public long Inspections1 { get; set; }
            public long Inspections2 { get; set; }
        }

        [Answer("99840", "20683044837")]
        private static (string, string) Day11(IEnumerable<IEnumerable<string>> input)
        {
            const string ItemsPrefix = "  Starting items: ";
            const string OperationPrefix = "  Operation: new = old ";
            const string TestPrefix = "  Test: divisible by ";
            const string TruePrefix = "    If true: throw to monkey ";
            const string FalsePrefix = "    If false: throw to monkey ";

            var monkeys = new List<Monkey>();

            var commonDivisor = 1L;
            foreach (var lines in input.Select(x => x.ToArray()))
            {
                var monkey = new Monkey();

                if (!lines[1].StartsWith(ItemsPrefix)) throw new Exception("Starting items failure.");
                var items = lines[1].Substring(ItemsPrefix.Length).Split(", ");
                foreach (var item in items)
                {
                    var parsedItem = Int64.Parse(item);
                    monkey.Items1.Enqueue(parsedItem);
                    monkey.Items2.Enqueue(parsedItem);
                }

                if (!lines[2].StartsWith(OperationPrefix)) throw new Exception("Operation failure.");
                var parts = lines[2].Substring(OperationPrefix.Length).Split(' ');
                var parameter = Expression.Parameter(typeof(long));
                Expression second = parts[1] == "old" ? parameter : Expression.Constant(Int64.Parse(parts[1]));
                switch (parts[0])
                {
                    case "+":
                        monkey.Operation1 = Expression.Lambda<Func<long, long>>(Expression.Divide(Expression.Add(parameter, second), Expression.Constant(3L)), parameter).Compile();
                        monkey.Operation2 = Expression.Lambda<Func<long, long>>(Expression.Add(parameter, second), parameter).Compile();
                        break;

                    case "*":
                        monkey.Operation1 = Expression.Lambda<Func<long, long>>(Expression.Divide(Expression.Multiply(parameter, second), Expression.Constant(3L)), parameter).Compile();
                        monkey.Operation2 = Expression.Lambda<Func<long, long>>(Expression.Multiply(parameter, second), parameter).Compile();
                        break;

                    default:
                        throw new Exception("Unrecognized operator.");
                }

                if (!lines[3].StartsWith(TestPrefix)) throw new Exception("Test failure.");
                monkey.TestDivisor = Int64.Parse(lines[3].Substring(TestPrefix.Length));
                commonDivisor = LCM(commonDivisor, monkey.TestDivisor);

                if (!lines[4].StartsWith(TruePrefix)) throw new Exception("True monkey failure.");
                monkey.TrueMonkey = Int32.Parse(lines[4].Substring(TruePrefix.Length));

                if (!lines[5].StartsWith(FalsePrefix)) throw new Exception("False monkey failure.");
                monkey.FalseMonkey = Int32.Parse(lines[5].Substring(FalsePrefix.Length));

                monkeys.Add(monkey);
            }

            for (var iteration = 0; iteration < 20; iteration++)
            {
                for (var index = 0; index < monkeys.Count; index++)
                {
                    var monkey = monkeys[index];
                    while (monkey.Items1.Any())
                    {
                        var item = monkey.Items1.Dequeue();
                        item = monkey.Operation1(item);
                        if (item % monkey.TestDivisor == 0)
                        {
                            monkeys[monkey.TrueMonkey].Items1.Enqueue(item);
                        }
                        else
                        {
                            monkeys[monkey.FalseMonkey].Items1.Enqueue(item);
                        }

                        monkey.Inspections1++;
                    }
                }
            }

            for (var iteration = 0; iteration < 10_000; iteration++)
            {
                for (var index = 0; index < monkeys.Count; index++)
                {
                    var monkey = monkeys[index];
                    while (monkey.Items2.Any())
                    {
                        var item = monkey.Items2.Dequeue();
                        item = monkey.Operation2(item);
                        if (item % monkey.TestDivisor == 0)
                        {
                            monkeys[monkey.TrueMonkey].Items2.Enqueue(item % commonDivisor);
                        }
                        else
                        {
                            monkeys[monkey.FalseMonkey].Items2.Enqueue(item % commonDivisor);
                        }

                        monkey.Inspections2++;
                    }
                }
            }

            var puzzle1 = monkeys.OrderByDescending(monkey => monkey.Inspections1).Take(2).Aggregate(1L, (x, monkey) => x * monkey.Inspections1);
            var puzzle2 = monkeys.OrderByDescending(monkey => monkey.Inspections2).Take(2).Aggregate(1L, (x, monkey) => x * monkey.Inspections2);

            return ($"{puzzle1}", $"{puzzle2}");
        }

        [Answer("13480")]
        private static (string, string) Day10(IEnumerable<string> input)
        {
            var cycle = 0;
            var register = 1;
            var puzzle1 = 0;
            var puzzle2 = new char[6][];
            for (var row = 0; row < 6; row++) puzzle2[row] = new char[40];

            foreach (var line in input)
            {
                switch (line[0])
                {
                    case 'n':
                        puzzle2[cycle/40][cycle%40] = Math.Abs(register - (cycle % 40)) <= 1 ? '#' : '.';
                        if (++cycle % 40 == 20) puzzle1 += register * cycle;
                        break;

                    case 'a':
                        puzzle2[cycle/40][cycle%40] = Math.Abs(register - (cycle % 40)) <= 1 ? '#' : '.';
                        if (++cycle % 40 == 20) puzzle1 += register * cycle;
                        puzzle2[cycle/40][cycle%40] = Math.Abs(register - (cycle % 40)) <= 1 ? '#' : '.';
                        if (++cycle % 40 == 20) puzzle1 += register * cycle;
                        register += Int32.Parse(line.Substring(5));
                        break;

                    default:
                        throw new Exception($"Unexpected input: {line}");
                }

                if (cycle >= 240) break;
            }

            var puzzle2Result = String.Join('\n', puzzle2.Select(x => String.Join("", x)));

            return ($"{puzzle1}", $"\n{puzzle2Result}");
        }

        [Answer("6337", "2455")]
        private static (string, string) Day9(IEnumerable<string> input)
        {
            var knots = new (int x, int y)[10];
            for (var index = 0; index < 10; index++) knots[index] = (0, 0);

            var locations1 = new HashSet<(int, int)> { knots[1] };
            var locations2 = new HashSet<(int, int)> { knots[9] };

            foreach (var line in input)
            {
                Func<(int x, int y), (int x, int y)> direction;
                switch (line[0])
                {
                    case 'R':
                        direction = ((int x, int y) head) => (head.x + 1, head.y);
                        break;

                    case 'L':
                        direction = ((int x, int y) head) => (head.x - 1, head.y);
                        break;

                    case 'U':
                        direction = ((int x, int y) head) => (head.x, head.y + 1);
                        break;

                    case 'D':
                        direction = ((int x, int y) head) => (head.x, head.y - 1);
                        break;

                    default:
                        throw new Exception($"Unhandled direction ({line[0]}) in input.");
                }

                var count = Int32.Parse(line.Substring(2));
                for (var iteration = 0; iteration < count; iteration++)
                {
                    knots[0] = direction(knots[0]);
                    for (var index = 1; index < 10; index++)
                    {
                        if (Math.Abs(knots[index - 1].x - knots[index].x) <= 1 && Math.Abs(knots[index - 1].y - knots[index].y) <= 1) break;

                        knots[index] = (knots[index].x + Math.Sign(knots[index - 1].x - knots[index].x), knots[index].y + Math.Sign(knots[index - 1].y - knots[index].y));
                        if (index == 1) locations1.Add(knots[index]);
                        else if (index == 9) locations2.Add(knots[index]);
                    }
                }
            }

            return ($"{locations1.Count}", $"{locations2.Count}");
        }

        [Answer("1818", "368368")]
        private static (string, string) Day8(IEnumerable<string> input)
        {
            var treeArray = input.Select(row => row.ToCharArray().Select(c => c - '0').ToArray()).ToArray();
            var height = treeArray.Length;
            var width = treeArray[0].Length;

            var visibleArray = new bool[height][];

            var scenicArray = new int[height][];
            for (var rowIndex = 0; rowIndex < height; rowIndex++)
            {
                scenicArray[rowIndex] = new int[width];
                for (var columnIndex = 0; columnIndex < width; columnIndex++) scenicArray[rowIndex][columnIndex] = 1;
            }

            for (var rowIndex = 0; rowIndex < height; rowIndex++)
            {
                visibleArray[rowIndex] = new bool[width];
                scenicArray[rowIndex] = new int[width];

                for (var columnIndex = 0; columnIndex < width; columnIndex++)
                {
                    if (rowIndex == 0 || rowIndex == height - 1 || columnIndex == 0 || columnIndex == width - 1)
                    {
                        visibleArray[rowIndex][columnIndex] = true;
                        scenicArray[rowIndex][columnIndex] = 0;
                        continue;
                    }

                    scenicArray[rowIndex][columnIndex] = 1;

                    for (var rowSeek = rowIndex - 1; rowSeek >= 0; rowSeek--)
                    {
                        if (treeArray[rowIndex][columnIndex] <= treeArray[rowSeek][columnIndex])
                        {
                            scenicArray[rowIndex][columnIndex] *= rowIndex - rowSeek;
                            break;
                        }

                        if (rowSeek == 0)
                        {
                            visibleArray[rowIndex][columnIndex] = true;
                            scenicArray[rowIndex][columnIndex] *= rowIndex - rowSeek;
                        }
                    }

                    for (var rowSeek = rowIndex + 1; rowSeek < height; rowSeek++)
                    {
                        if (treeArray[rowIndex][columnIndex] <= treeArray[rowSeek][columnIndex])
                        {
                            scenicArray[rowIndex][columnIndex] *= rowSeek - rowIndex;
                            break;
                        }

                        if (rowSeek == height - 1)
                        {
                            visibleArray[rowIndex][columnIndex] = true;
                            scenicArray[rowIndex][columnIndex] *= rowSeek - rowIndex;
                        }
                    }

                    for (var columnSeek = columnIndex - 1; columnSeek >= 0; columnSeek--)
                    {
                        if (treeArray[rowIndex][columnIndex] <= treeArray[rowIndex][columnSeek])
                        {
                            scenicArray[rowIndex][columnIndex] *= columnIndex - columnSeek;
                            break;
                        }

                        if (columnSeek == 0)
                        {
                            visibleArray[rowIndex][columnIndex] = true;
                            scenicArray[rowIndex][columnIndex] *= columnIndex - columnSeek;
                        }
                    }

                    for (var columnSeek = columnIndex + 1; columnSeek < width; columnSeek++)
                    {
                        if (treeArray[rowIndex][columnIndex] <= treeArray[rowIndex][columnSeek])
                        {
                            scenicArray[rowIndex][columnIndex] *= columnSeek - columnIndex;
                            break;
                        }

                        if (columnSeek == width - 1)
                        {
                            visibleArray[rowIndex][columnIndex] = true;
                            scenicArray[rowIndex][columnIndex] *= columnSeek - columnIndex;
                        }
                    }
                }
            }

            var puzzle1 = visibleArray.Sum(x => x.Where(y => y).Count());
            var puzzle2 = scenicArray.Max(x => x.Max());

            return ($"{puzzle1}", $"{puzzle2}");
        }

        private class File
        {
            public string Name { get; set; }
            public long Size { get; set; }
        }

        private class Directory
        {
            public Directory() => (Files, SubDirectories) = (new List<File>(), new Dictionary<string, Directory>());

            public string Name { get; set; }
            public Directory Parent { get; set; }
            public IList<File> Files { get; }
            public IDictionary<string, Directory> SubDirectories { get; }

            public long Size => this.Files.Sum(f => f.Size) + this.SubDirectories.Sum(d => d.Value.Size);
        }

        [Answer("1844187", "4978279")]
        private static (string, string) Day7(IEnumerable<string> input)
        {
            var root = new Directory { Name = "/" };
            var current = root;

            var allDirectories = new List<Directory> { root };

            foreach (var line in input)
            {
                if (String.IsNullOrWhiteSpace(line)) continue;

                var parts = line.Split(' ');
                switch (parts[0])
                {
                    case "$":
                        switch (parts[1])
                        {
                            case "cd":
                                switch (parts[2])
                                {
                                    case "/":
                                        current = root;
                                        break;

                                    case "..":
                                        current = current.Parent;
                                        break;

                                    default:
                                        current = current.SubDirectories[parts[2]];
                                        break;
                                }
                                break;

                            case "ls":
                                break;
                        }
                        break;

                    case "dir":
                        var newDirectory = new Directory { Name = parts[1], Parent = current };
                        current.SubDirectories.Add(parts[1], newDirectory);
                        allDirectories.Add(newDirectory);
                        break;

                    default:
                        current.Files.Add(new File { Name = parts[1], Size = Int64.Parse(parts[0]) });
                        break;
                }
            }

            var puzzle1 = allDirectories.Where(d => d.Size <= 100000).Sum(d => d.Size);

            var availableSpace = 70_000_000 - root.Size;
            var neededSpace = 30_000_000 - availableSpace;
            var puzzle2 = allDirectories.Where(d => d.Size > neededSpace).Min(d => d.Size);

            return ($"{puzzle1}", $"{puzzle2}");
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
