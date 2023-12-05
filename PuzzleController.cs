using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Moyba.AdventOfCode
{
    using PuzzleInfo = (int year, int day, ConstructorInfo constructor, Action<string> validatePartOne, Action<string> validatePartTwo);

    public class PuzzleController
    {
        private static readonly Type[] _PuzzleConstructorTypes = [ typeof(IEnumerable<string>) ];
        private static readonly SolutionAttribute _DefaultSolution = new SolutionAttribute();
        private static readonly Regex _EndsWithNumberRegex = new Regex(@"[^\d](?<Number>\d+)$", RegexOptions.Compiled);

        private readonly IEnumerable<PuzzleInfo> _puzzles;

        public PuzzleController(IEnumerable<Type> types)
        {
            _puzzles = types
                .Where(t => !t.IsAbstract && !t.IsInterface)
                .Where(t => t.IsAssignableTo(typeof(IPuzzle)))
                .Select<Type, PuzzleInfo>(t => {
                    var name = t.Name;
                    var ns = t.Namespace;

                    if (ns == null) throw new Exception($"Puzzle '{name}' does not follow the naming convention for puzzles.");

                    var yearMatch = _EndsWithNumberRegex.Match(ns);
                    if (!yearMatch.Success) throw new Exception($"Puzzle '{name}' does not follow the naming convention for puzzles.");
                    var year = Int32.Parse(yearMatch.Groups["Number"].Value);

                    var dayMatch = _EndsWithNumberRegex.Match(name);
                    if (!dayMatch.Success) throw new Exception($"Puzzle '{name}' does not follow the naming convention for puzzles.");
                    var day = Int32.Parse(dayMatch.Groups["Number"].Value);

                    var constructor = t.GetConstructor(_PuzzleConstructorTypes);
                    if (constructor == null) throw new Exception($"Puzzle '{name}' does not follow the convention for constructor signatures.");

                    var partOne = (t.GetMethod(nameof(IPuzzle.PartOne))?.GetCustomAttribute<SolutionAttribute>() ?? _DefaultSolution).GetValidator("One");
                    var partTwo = (t.GetMethod(nameof(IPuzzle.PartTwo))?.GetCustomAttribute<SolutionAttribute>() ?? _DefaultSolution).GetValidator("Two");

                    return (year, day, constructor, partOne, partTwo);
                })
                .OrderBy(_ => _.day)
                .OrderBy(_ => _.year);
        }

        public async Task SolvePuzzlesAsync()
        {
            foreach ((var year, var day, var constructor, var validatePartOne, var validatePartTwo) in _puzzles)
            {
                var data = await this.ReadInputFileAsync(year, day);
                Console.WriteLine();

                var times = new List<(char code, long time)>(4);
                var overallStopwatch = Stopwatch.StartNew();

                var stopwatch = Stopwatch.StartNew();
                var puzzle = (IPuzzle)constructor.Invoke([ data.ToArray() ]);
                times.Add(('T', stopwatch.ElapsedMilliseconds));

                stopwatch = Stopwatch.StartNew();
                await puzzle.ComputeAsync();
                times.Add(('C', stopwatch.ElapsedMilliseconds));

                var serializedTimes = String.Join(", ", times.Where(t => !Char.IsDigit(t.code) || t.time > 1).Select(_ => $"{_.code}: {_.time}"));
                Console.WriteLine($"Year {year}, Day {day} [{serializedTimes}] ({overallStopwatch.Elapsed})");
                validatePartOne(puzzle.PartOne);
                validatePartTwo(puzzle.PartTwo);
            }
        }

        private async Task<IEnumerable<string>> ReadInputFileAsync(int year, int day)
        {
            // check whether the file already exists
            var inputFile = $"{year}/{day}.txt";
            if (!File.Exists(inputFile))
            {
                // read the session cookie value
                const string sessionPath = ".session";
                if (!File.Exists(sessionPath)) throw new Exception("Set the Advent of Code session cookie value in a .session file.");
                var sessionCookieValue = await File.ReadAllTextAsync(sessionPath);

                // set up a client for requesting the input data
                var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("cookie", $"session={sessionCookieValue}");

                // request the input data
                var inputUri = $"https://adventofcode.com/{year}/day/{day}/input";
                var inputData = await httpClient.GetStringAsync(inputUri);

                // ensure the directory exists
                var directoryPath = Path.GetDirectoryName(inputFile);
                if (directoryPath != null && !Directory.Exists(directoryPath)) Directory.CreateDirectory(directoryPath);

                // write the input to file
                await File.WriteAllTextAsync(inputFile, inputData);
            }

            return await File.ReadAllLinesAsync(inputFile);
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class SolutionAttribute(string? value = null) : Attribute
    {
        private readonly string? _value = value;

        public Action<string> GetValidator(string part)
        {
            return (string answer) => {
                if (_value?.Equals(answer) ?? true)
                {
                    Console.WriteLine($"  Part {part}: {answer}");
                }
                else
                {
                    Console.WriteLine($"  Part {part} incorrect.");
                    Console.WriteLine($"    Answer: {answer}");
                    Console.WriteLine($"    Expected: {_value}");
                }
            };
        }
    }

    public interface IPuzzle
    {
        Task ComputeAsync();
        string PartOne { get; }
        string PartTwo { get; }
    }
}
