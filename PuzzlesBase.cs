using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;

namespace Moyba.AdventOfCode
{
    internal abstract class PuzzlesBase
    {
        protected abstract int Year { get; }

        public abstract Task SolveAsync();

        protected static IEnumerable<string> LineDelimited(IEnumerable<string> input) => input;

        protected static IEnumerable<string> AsStrings(IEnumerable<string> input) => input;
        protected static IEnumerable<IEnumerable<long>> AsBatchesOfLongs(IEnumerable<string> input) =>
            PuzzlesBase.AsBatchesOf<long>(input, Int64.Parse);

        private static IEnumerable<IEnumerable<T>> AsBatchesOf<T>(IEnumerable<string> input, Func<string, T> converter)
        {
            using (var enumerator = input.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    yield return PuzzlesBase.EnumerateGroup<T>(enumerator, converter);
                }
            }
        }

        private static IEnumerable<T> EnumerateGroup<T>(IEnumerator<string> enumerator, Func<string, T> converter)
        {
            do
            {
                if (String.IsNullOrEmpty(enumerator.Current)) yield break;

                yield return converter(enumerator.Current);

            } while (enumerator.MoveNext());
        }

        protected Task SolveAsync(Expression<Func<Func<IEnumerable<string>, (string, string)>>> dayExpression) =>
            this.SolveAsync(dayExpression, LineDelimited, AsStrings);

        protected async Task SolveAsync<T>(Expression<Func<Func<T, (string, string)>>> dayExpression, Func<IEnumerable<string>, IEnumerable<string>> delimiter, Func<IEnumerable<string>, T> converter)
        {
            var bodyExpression = (UnaryExpression)dayExpression.Body;
            var operandExpression = (MethodCallExpression)bodyExpression.Operand;
            var objectExpression = (ConstantExpression)operandExpression.Object;
            var methodInfo = (MethodInfo)objectExpression.Value;
            var day = Int32.Parse(methodInfo.Name.Substring(3));

            var inputTask = this.GetInputAsync(day);

            var answerAttribute = methodInfo.GetCustomAttribute<AnswerAttribute>();
            var method = dayExpression.Compile()();
            var input = converter(delimiter(await inputTask));

            if (day > 1) Console.WriteLine();
            Console.Write($"Day {day}");

            var stopwatch = Stopwatch.StartNew();
            var (answer1, answer2) = method(input);
            Console.WriteLine($" ({stopwatch.Elapsed})");
            if (answerAttribute != null)
            {
                answerAttribute.ValidateAndDisplay(answer1, answer2);
            }
            else
            {
                Console.WriteLine($"Puzzle 1: {answer1}");
                Console.WriteLine($"Puzzle 2: {answer2}");
            }
        }

        private async Task<IEnumerable<string>> GetInputAsync(int day)
        {
            await this.EnsureInputFileAsync(day);
            return await File.ReadAllLinesAsync($"{this.Year}/{day}.txt");
        }

        private async Task EnsureInputFileAsync(int day)
        {
            // check whether the file already exists
            var inputFile = $"{this.Year}/{day}.txt";
            if (File.Exists(inputFile)) return;

            // make sure the session cookie has a value
            if (String.IsNullOrEmpty(Session.CookieValue)) throw new Exception("Set the Advent of Code session cookie value in Session.cs.");

            // set up a client for requesting the input data
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("cookie", $"session={Session.CookieValue}");

            // request the input data
            var inputUri = $"https://adventofcode.com/{this.Year}/day/{day}/input";
            var inputData = await httpClient.GetStringAsync(inputUri);

            // ensure the directory exists
            var directoryPath = Path.GetDirectoryName(inputFile);
            if (directoryPath != null && !Directory.Exists(directoryPath)) Directory.CreateDirectory(directoryPath);

            // write the input to file
            await File.WriteAllTextAsync(inputFile, inputData);
        }

        protected class AnswerAttribute : Attribute
        {
            public AnswerAttribute(string? expected1 = null, string? expected2 = null)
                => (Expected1, Expected2) = (expected1, expected2);

            public string? Expected1 { get; }

            public string? Expected2 { get; }

            public void ValidateAndDisplay(string? answer1, string? answer2)
            {
                this.ValidateAndDisplay(1, this.Expected1, answer1);
                this.ValidateAndDisplay(2, this.Expected2, answer2);
            }

            private void ValidateAndDisplay(int number, string? expected, string? answer)
            {
                if (expected?.Equals(answer) ?? true)
                {
                    Console.WriteLine($"Puzzle {number}: {answer}");
                }
                else
                {
                    Console.WriteLine($"Puzzle {number} incorrect.");
                    Console.WriteLine($"  Answer: {answer}");
                    Console.WriteLine($"  Expected: {expected}");
                }
            }
        }
    }
}