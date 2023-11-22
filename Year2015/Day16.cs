using System.Text.RegularExpressions;

namespace Moyba.AdventOfCode.Year2015
{
    public class Day16 : SolutionBase
    {
        private static readonly Regex Parser = new Regex(@"^Sue (?<number>\d+): (?<item1>[a-z]+): (?<count1>\d+), (?<item2>[a-z]+): (?<count2>\d+), (?<item3>[a-z]+): (?<count3>\d+)$", RegexOptions.Compiled);
        private static readonly IDictionary<string, int> Clues = new Dictionary<string, int> {
            { "children", 3 },
            { "cats", 7 },
            { "samoyeds", 2 },
            { "pomeranians", 3 },
            { "akitas", 0 },
            { "vizslas", 0 },
            { "goldfish", 5 },
            { "trees", 3 },
            { "cars", 2 },
            { "perfumes", 1 },
        };

        private (int number, Dictionary<string, int> items)[] _data = Array.Empty<(int, Dictionary<string, int>)>();

        [Expect("213")]
        protected override string SolvePart1()
        {
            var aunt = _data
                .Where(_ => _.items.All(clue => Clues[clue.Key] == clue.Value))
                .Select(_ => _.number)
                .Single();
            return $"{aunt}";
        }

        [Expect("323")]
        protected override string SolvePart2()
        {
            var aunt = _data
                .Where(_ => _.items.All(clue => {
                    switch (clue.Key)
                    {
                        case "cats":
                        case "trees":
                            return clue.Value > Clues[clue.Key];

                        case "pomeranians":
                        case "goldfish":
                            return clue.Value < Clues[clue.Key];

                        default:
                            return clue.Value == Clues[clue.Key];
                    }
                }))
                .Select(_ => _.number)
                .Single();
            return $"{aunt}";
        }

        protected override void TransformData(IEnumerable<string> data) => _data = data
            .Select(_ => {
                var match = Parser.Match(_);
                if (!match.Success) throw new Exception($"Unexpected input: {_}");
                return match;
            })
            .Select(_ => (
                Int32.Parse(_.Groups["number"].Value),
                new Dictionary<string, int> {
                    { _.Groups["item1"].Value, Int32.Parse(_.Groups["count1"].Value) },
                    { _.Groups["item2"].Value, Int32.Parse(_.Groups["count2"].Value) },
                    { _.Groups["item3"].Value, Int32.Parse(_.Groups["count3"].Value) },
                }
            ))
            .ToArray();
    }
}
