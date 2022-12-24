using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Moyba.AdventOfCode
{
    public abstract class SolutionBase<T> : ISolution
    {
        private static readonly ExpectAttribute DefaultExpect = new ExpectAttribute();

        private readonly Type _type;
        private readonly int _day, _year;

        private static Regex _EndsWithNumberRegex = new Regex(@"[^\d](?<Number>\d+)$", RegexOptions.Compiled);

        public SolutionBase()
        {
            _type = this.GetType();
            if (_type.Namespace == null) throw new Exception($"{_type.Name} does not follow the naming convention for solutions.");

            var yearMatch = _EndsWithNumberRegex.Match(_type.Namespace);
            if (!yearMatch.Success) throw new Exception($"{_type.Name} does not follow the naming convention for solutions.");
            _year = Int32.Parse(yearMatch.Groups["Number"].Value);

            var dayMatch = _EndsWithNumberRegex.Match(_type.Name);
            if (!dayMatch.Success) throw new Exception($"{_type.Name} does not follow the naming convention for solutions.");
            _day = Int32.Parse(dayMatch.Groups["Number"].Value);
        }

        public int Year => _year;
        public int Day => _day;

        public async Task SolveAsync()
        {
            var overallStopwatch = Stopwatch.StartNew();
            Console.WriteLine();

            Console.Write($"Year {_year}, Day {_day}");

            var stopwatch = Stopwatch.StartNew();
            var input = await this.ReadInputFileAsync();
            var data = this.ReadInput(input);
            Console.Write($" [R: {stopwatch.ElapsedMilliseconds}");

            stopwatch = Stopwatch.StartNew();
            this.TransformData(data);
            Console.Write($", T: {stopwatch.ElapsedMilliseconds}");

            stopwatch = Stopwatch.StartNew();
            var part1 = this.SolvePart1();
            Console.Write($", 1: {stopwatch.ElapsedMilliseconds}");

            stopwatch = Stopwatch.StartNew();
            var part2 = this.SolvePart2();
            Console.WriteLine($", 2: {stopwatch.ElapsedMilliseconds}] ({overallStopwatch.Elapsed})");

            (_type.GetMethod(nameof(SolvePart1))?.GetCustomAttribute<ExpectAttribute>() ?? DefaultExpect).ValidateAndDisplay(1, part1);
            (_type.GetMethod(nameof(SolvePart2))?.GetCustomAttribute<ExpectAttribute>() ?? DefaultExpect).ValidateAndDisplay(2, part2);
        }

        protected abstract T ReadInput(IEnumerable<string> input);
        protected abstract void TransformData(T data);
        protected abstract string SolvePart1();
        protected abstract string SolvePart2();

        protected static long _GCD(long a, long b)
        {
            if (b == 0) return a;
            return _GCD(b, a % b);
        }

        protected static long _LCM(long a, long b)
        {
            return a * b / _GCD(a, b);
        }

        private async Task<IEnumerable<string>> ReadInputFileAsync()
        {
            // check whether the file already exists
            var inputFile = $"{_year}/{_day}.txt";
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
                var inputUri = $"https://adventofcode.com/{_year}/day/{_day}/input";
                var inputData = await httpClient.GetStringAsync(inputUri);

                // ensure the directory exists
                var directoryPath = Path.GetDirectoryName(inputFile);
                if (directoryPath != null && !Directory.Exists(directoryPath)) Directory.CreateDirectory(directoryPath);

                // write the input to file
                await File.WriteAllTextAsync(inputFile, inputData);
            }

            return await File.ReadAllLinesAsync(inputFile);
        }

        [AttributeUsage(AttributeTargets.Method)]
        protected class ExpectAttribute : Attribute
        {
            private readonly string? _value;

            public ExpectAttribute(string? value = null) => _value = value;

            public void ValidateAndDisplay(int number, string? answer)
            {
                if (_value?.Equals(answer) ?? true)
                {
                    Console.WriteLine($"  Part {number}: {answer}");
                }
                else
                {
                    Console.WriteLine($"  Part {number} incorrect.");
                    Console.WriteLine($"    Answer: {answer}");
                    Console.WriteLine($"    Expected: {_value}");
                }
            }
        }
    }

    public abstract class SolutionBase : SolutionBase<IEnumerable<string>>
    {
        protected override IEnumerable<string> ReadInput(IEnumerable<string> input) => input;
    }

    public interface ISolution
    {
        int Year { get; }
        int Day { get; }
        Task SolveAsync();
    }
}
