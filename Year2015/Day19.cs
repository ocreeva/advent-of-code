using System.Text.RegularExpressions;

namespace Moyba.AdventOfCode.Year2015
{
    public class Day19 : SolutionBase
    {
        private static readonly Regex Parser = new Regex(@"^(?<input>.+) => (?<output>.+)$", RegexOptions.Compiled);

        private readonly IDictionary<string, IList<string>> _replacements = new Dictionary<string, IList<string>>();
        private readonly IDictionary<string, string> _reverse = new Dictionary<string, string>();

        private string _molecule = String.Empty;
        private IEnumerable<string> _orderedOutput = Enumerable.Empty<string>();

        [Expect("518")]
        protected override string SolvePart1()
        {
            var results = new HashSet<string>();
            this.FindAllResults(_molecule, results);

            return $"{results.Count}";
        }

        [Expect("200")]
        protected override string SolvePart2()
        {
            this.TryFindSourceLength(_molecule, out int iteration);

            return $"{iteration}";
        }

        private void FindAllResults(string molecule, HashSet<string> results)
        {
            foreach (var input in _replacements.Keys)
            {
                var outputs = _replacements[input];
                for (var index = molecule.IndexOf(input); index >= 0; index = molecule.IndexOf(input, index + 1))
                {
                    var start = molecule.Substring(0, index);
                    var end = molecule.Substring(index + input.Length);
                    foreach (var output in outputs)
                    {
                        results.Add($"{start}{output}{end}");
                    }
                }
            }
        }

        private bool TryFindSourceLength(string molecule, out int iteration)
        {
            if (molecule.Equals("e"))
            {
                iteration = 0;
                return true;
            }

            iteration = -1;
            foreach (var output in _orderedOutput)
            {
                var input = _reverse[output];
                for (var index = molecule.IndexOf(output); index >= 0; index = molecule.IndexOf(output, index + 1))
                {
                    var start = molecule.Substring(0, index);
                    var end = molecule.Substring(index + output.Length);
                    if (this.TryFindSourceLength($"{start}{input}{end}", out iteration))
                    {
                        iteration++;
                        return true;
                    }
                }
            }

            return false;
        }

        protected override void TransformData(IEnumerable<string> data)
        {
            var isMolecule = false;
            foreach (var line in data)
            {
                if (isMolecule)
                {
                    _molecule = line;
                    break;
                }

                if (String.IsNullOrWhiteSpace(line))
                {
                    isMolecule = true;
                    continue;
                }

                var match = Parser.Match(line);
                if (!match.Success) throw new Exception($"Unhandled input: {line}");

                var input = match.Groups["input"].Value;
                var output = match.Groups["output"].Value;

                if (!_replacements.ContainsKey(input)) _replacements[input] = new List<string>();
                _replacements[input].Add(output);
                _reverse.Add(output, input);
            }

            _orderedOutput = _reverse.Keys.OrderByDescending(_ => _.Length).ToArray();
        }
    }
}
