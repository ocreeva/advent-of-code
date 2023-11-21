using System.Text.RegularExpressions;

namespace Moyba.AdventOfCode.Year2015
{
    public class Day13 : SolutionBase
    {
        private static readonly Regex Parser = new Regex(@"^(.+) would (lose|gain) (\d+) happiness units? by sitting next to (.+)\.$", RegexOptions.Compiled);

        private string[] _names = Array.Empty<string>();
        private IDictionary<(string, string), int> _data = new Dictionary<(string, string), int>();

        [Expect("664")]
        protected override string SolvePart1()
        {
            // var happiness = Enumerable.Range(0, _names.Length).Max(start => this.MaximizeHappiness(start, start, 1 << start));
            var happiness = this.MaximizeHappiness(0, 0, 1);
            return $"{happiness}";
        }

        [Expect("640")]
        protected override string SolvePart2()
        {
            foreach (var name in _names)
            {
                _data[("_", name)] = 0;
                _data[(name, "_")] = 0;
            }

            _names = [ .._names, "_" ];

            var happiness = this.MaximizeHappiness(0, 0, 1);
            return $"{happiness}";
        }

        private int MaximizeHappiness(int startingIndex, int previousIndex, int seatedIndexes)
        {
            var happiness = Int32.MinValue;
            for (var index = 0; index < _names.Length; index++)
            {
                var bitwise = 1 << index;
                if ((bitwise & seatedIndexes) != 0) continue;

                var currentHappiness = _data[(_names[previousIndex], _names[index])];
                var futureHappiness = this.MaximizeHappiness(startingIndex, index, seatedIndexes | bitwise);
                happiness = Math.Max(happiness, currentHappiness + futureHappiness);
            }

            if (happiness == Int32.MinValue) return _data[(_names[previousIndex], _names[startingIndex])];
            return happiness;
        }

        protected override void TransformData(IEnumerable<string> data)
        {
            var names = new HashSet<string>();
            foreach (var line in data)
            {
                var match = Parser.Match(line);
                if (!match.Success) throw new Exception($"Unexpected input: {line}");

                var name1 = match.Groups[1].Value;
                var name2 = match.Groups[4].Value;
                var positive = match.Groups[2].Value.Equals("gain");
                var happiness = Int32.Parse(match.Groups[3].Value) * (positive ? 1 : -1);

                names.Add(name1);
                names.Add(name2);

                if (!_data.ContainsKey((name1, name2))) _data[(name1, name2)] = 0;
                if (!_data.ContainsKey((name2, name1))) _data[(name2, name1)] = 0;

                _data[(name1, name2)] += happiness;
                _data[(name2, name1)] += happiness;
            }

            _names = names.ToArray();
        }
    }
}
