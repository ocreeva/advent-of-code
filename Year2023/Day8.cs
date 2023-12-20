using Moyba.AdventOfCode.Utility;

namespace Moyba.AdventOfCode.Year2023
{
    public class Day8(string[] _data) : IPuzzle
    {
        private readonly int[] _instructions = _data[0].Select(_ => _ - 'L').Select(Math.Sign).ToArray();
        private readonly IDictionary<string, string[]> _nodes = _data
            .Skip(2)
            .ToDictionary<string, string, string[]>(_ => _[0..3], _ => [ _[7..10], _[12..15] ]);

        [PartOne("21797")]
        [PartTwo("23977527174353")]
        public async IAsyncEnumerable<string> ComputeAsync()
        {
            // N.B. - this is not generally valid; I'm relying on specific behavior in the input data, where cycles
            // occur along multiples of the instruction set length, and recur with the same number of iterations

            var iteration = 0L;
            var iterationLength = _instructions.Length;
            for (var current = "AAA"; !current.Equals("ZZZ"); iteration++)
            {
                foreach (var instruction in _instructions) current = _nodes[current][instruction];
            }

            yield return $"{iteration * iterationLength}";

            var lcm = iteration;
            var aNodes = _nodes.Keys.Where(_ => _.EndsWith('A')).Where(_ => !_.Equals("AAA")).ToArray();
            var zNodes = _nodes.Keys.Where(_ => _.EndsWith('Z')).ToHashSet();
            foreach (var aNode in aNodes)
            {
                var current = aNode;
                for (iteration = 0; !zNodes.Contains(current); iteration++)
                {
                    foreach (var instruction in _instructions) current = _nodes[current][instruction];
                }

                lcm = LCM.Calculate(lcm, iteration);
            }

            yield return $"{lcm * iterationLength}";

            await Task.CompletedTask;
        }
    }
}
