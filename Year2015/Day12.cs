using System.Text.Json;

namespace Moyba.AdventOfCode.Year2015
{
    public class Day12 : SolutionBase<string>
    {
        private JsonDocument _document = JsonDocument.Parse("{}");

        protected override string ReadInput(IEnumerable<string> input) => input.First();

        [Expect("191164")]
        protected override string SolvePart1()
        {
            var sum = this.SumNumbers(_document.RootElement);
            return $"{sum}";
        }

        [Expect("87842")]
        protected override string SolvePart2()
        {
            var sum = this.SumNumbers(_document.RootElement, ignoreRed: true);
            return $"{sum}";
        }

        private int SumNumbers(JsonElement element, bool ignoreRed = false)
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.Number:
                    return element.GetInt32();

                case JsonValueKind.String:
                    return 0;

                case JsonValueKind.Array:
                    using (var enumerator = element.EnumerateArray())
                    {
                        return enumerator.Sum(_ => this.SumNumbers(_, ignoreRed));
                    }

                case JsonValueKind.Object:
                    using (var enumerator = element.EnumerateObject())
                    {
                        var properties = enumerator.Select(_ => _.Value).ToArray();
                        if (ignoreRed && properties.Any(_ => _.ValueKind == JsonValueKind.String && _.GetString() == "red")) return 0;
                        return properties.Sum(_ => this.SumNumbers(_, ignoreRed));
                    }

                default:
                    throw new Exception($"Unsupported JSON value kind: {element.ValueKind}");
            }
        }

        protected override void TransformData(string data) => _document = JsonDocument.Parse(data);
    }
}
