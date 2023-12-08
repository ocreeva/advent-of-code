using System.Text.RegularExpressions;
using AdventOfCode.Utility;

namespace Moyba.AdventOfCode.Year2023
{
    using Node = (string id, string left, string right);

    public class Day8(string[] _data) : IPuzzle
    {
        private static readonly Regex _NodeParser = new Regex(@"^(.+) = \((.+), (.+)\)$", RegexOptions.Compiled);

        private readonly char[] _instructions = _data[0].ToCharArray();
        private readonly IDictionary<string, Node> _nodes = _data.Skip(2).Transform<Node>(_NodeParser).ToDictionary(_ => _.id);

        [PartOne("21797")]
        [PartTwo("23977527174353")]
        public async IAsyncEnumerable<string> ComputeAsync()
        {
            var step = 0L;
            for (var current = "AAA"; !current.Equals("ZZZ"); step++)
            {
                var instruction = _instructions[step % _instructions.Length];
                var node = _nodes[current];
                current = instruction switch
                {
                    'L' => node.left,
                    'R' => node.right,
                    _ => throw new Exception($"Unexpected instruction: {instruction}")
                };
            }

            Console.WriteLine($"Test: {(step) % _instructions.Length}");

            yield return $"{step}";

            // N.B. - this is not valid cycle detection; I'm relying on specific behavior in the input data to simplify the problem down to a LCM calculation

            var lcm = step;
            var aNodes = _nodes.Keys.Where(_ => _.EndsWith('A')).Where(_ => !_.Equals("AAA")).ToArray();
            var zNodes = _nodes.Keys.Where(_ => _.EndsWith('Z')).ToHashSet();
            foreach (var aNode in aNodes)
            {
                var current = aNode;
                for (step = 0; !zNodes.Contains(current); step++)
                {
                    var instruction = _instructions[step % _instructions.Length];
                    var node = _nodes[current];
                    current = instruction switch
                    {
                        'L' => node.left,
                        'R' => node.right,
                        _ => throw new Exception($"Unexpected instruction: {instruction}")
                    };
                }

                lcm = LCM.Calculate(lcm, step);
            }

            yield return $"{lcm}";

            await Task.CompletedTask;
        }
    }
}
