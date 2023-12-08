using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Moyba.AdventOfCode
{
    using PuzzleInfo = (int year, int day, ConstructorInfo constructor, IDictionary<int, SolutionAttribute> validators);

    public class PuzzleController
    {
        private static readonly Type[] _PuzzleConstructorTypes = [ typeof(string[]) ];
        private static readonly IDictionary<int, SolutionAttribute> _EmptyValidators = new Dictionary<int, SolutionAttribute>();
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

                    var validators = t.GetMethod(nameof(IPuzzle.ComputeAsync))?.GetCustomAttributes<SolutionAttribute>().ToDictionary(_ => _.Part) ?? _EmptyValidators;

                    return (year, day, constructor, validators);
                })
                .OrderBy(_ => _.day)
                .OrderBy(_ => _.year);
        }

        public async Task SolvePuzzlesAsync()
        {
            foreach ((var year, var day, var constructor, var validators) in _puzzles)
            {
                var data = await this.ReadInputFileAsync(year, day);
                Console.WriteLine();

                var times = new List<(char code, long time)>(4);
                var overallStopwatch = Stopwatch.StartNew();

                var stopwatch = Stopwatch.StartNew();
                var puzzle = (IPuzzle)constructor.Invoke([ data.ToArray() ]);
                times.Add(('T', stopwatch.ElapsedMilliseconds));

                var key = '1';
                var solutions = new List<string>(2);
                stopwatch = Stopwatch.StartNew();
                await foreach (var solution in puzzle.ComputeAsync())
                {
                    if (solution == null)
                    {
                        times.Add(('C', stopwatch.ElapsedMilliseconds));
                    }
                    else
                    {
                        times.Add((key++, stopwatch.ElapsedMilliseconds));
                        solutions.Add(solution);
                    }

                    stopwatch = Stopwatch.StartNew();
                }

                var serializedTimes = String.Join(", ", times.Select(_ => $"{_.code}: {_.time}"));
                Console.WriteLine($"Year {year}, Day {day} [{serializedTimes}] ({overallStopwatch.Elapsed})");
                for (var index = 0; index < solutions.Count; index++)
                {
                    if (validators.ContainsKey(index + 1)) validators[index + 1].Validate(solutions[index]);
                    else Console.WriteLine($"  Part {index + 1}: {solutions[index]}");
                }
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

    [AttributeUsage(AttributeTargets.Method)]
    public abstract class SolutionAttribute(int _part, string? _value) : Attribute
    {
        public int Part => _part;

        public void Validate(string answer)
        {
            if (_value?.Equals(answer) ?? true)
            {
                Console.WriteLine($"  Part {_part}: {answer}");
            }
            else
            {
                Console.WriteLine($"  Part {_part} incorrect.");
                Console.WriteLine($"    Answer: {answer}");
                Console.WriteLine($"    Expected: {_value}");
            }
        }
    }

    public class PartOneAttribute(string? value = null) : SolutionAttribute(1, value) { }
    public class PartTwoAttribute(string? value = null) : SolutionAttribute(2, value) { }

    public interface IPuzzle
    {
        IAsyncEnumerable<string?> ComputeAsync();
    }
}
