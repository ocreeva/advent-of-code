using System.Text.RegularExpressions;

using Data = (string name, int capacity, int durability, int flavor, int texture, int calories);

namespace Moyba.AdventOfCode.Year2015
{
    public class Day15 : SolutionBase
    {
        private static readonly Regex Parser = new Regex(@"^(?<name>.*): capacity (?<capacity>-?\d+), durability (?<durability>-?\d+), flavor (?<flavor>-?\d+), texture (?<texture>-?\d+), calories (?<calories>-?\d+)$", RegexOptions.Compiled);

        private Data[] _data = Array.Empty<Data>();
        private int _part1 = 0;
        private int _part2 = 0;

        [Expect("13882464")]
        protected override string SolvePart1()
        {
            MaximizeCookie();
            return $"{_part1}";
        }

        [Expect("11171160")]
        protected override string SolvePart2()
        {
            return $"{_part2}";
        }

        private void MaximizeCookie(int dataIndex = 0, int remainingTeaspoons = 100, int capacity = 0, int durability = 0, int flavor = 0, int texture = 0, int calories = 0)
        {
            var data = _data[dataIndex];
            if (dataIndex != _data.Length - 1)
            {
                for (var teaspoons = 0; teaspoons <= remainingTeaspoons; teaspoons++)
                {
                    MaximizeCookie(
                        dataIndex + 1,
                        remainingTeaspoons - teaspoons,
                        capacity + data.capacity * teaspoons,
                        durability + data.durability * teaspoons,
                        flavor + data.flavor * teaspoons,
                        texture + data.texture * teaspoons,
                        calories + data.calories * teaspoons
                    );
                }

                return;
            }

            capacity += data.capacity * remainingTeaspoons;
            if (capacity <= 0) return;

            durability += data.durability * remainingTeaspoons;
            if (durability <= 0) return;

            flavor += data.flavor * remainingTeaspoons;
            if (flavor <= 0) return;

            texture += data.texture * remainingTeaspoons;
            if (texture <= 0) return;

            var score = capacity * durability * flavor * texture;
            _part1 = Math.Max(_part1, score);

            calories += data.calories * remainingTeaspoons;
            if (calories == 500) _part2 = Math.Max(_part2, score);
        }

        protected override void TransformData(IEnumerable<string> data) => _data = data
            .Select(_ => Parser.Match(_))
            .Select(_ => (
                _.Groups["name"].Value,
                Int32.Parse(_.Groups["capacity"].Value),
                Int32.Parse(_.Groups["durability"].Value),
                Int32.Parse(_.Groups["flavor"].Value),
                Int32.Parse(_.Groups["texture"].Value),
                Int32.Parse(_.Groups["calories"].Value)
            ))
            .ToArray();
    }
}
