namespace Moyba.AdventOfCode.Year2016
{
    public class Day2(string[] _data) : IPuzzle
    {
        // U, L, R, D
        private static readonly IDictionary<char, char[]> _PartOneLookup = new Dictionary<char, char[]>
        {
            { '1', [ '1', '1', '2', '4' ] },
            { '2', [ '2', '1', '3', '5' ] },
            { '3', [ '3', '2', '3', '6' ] },
            { '4', [ '1', '4', '5', '7' ] },
            { '5', [ '2', '4', '6', '8' ] },
            { '6', [ '3', '5', '6', '9' ] },
            { '7', [ '4', '7', '8', '7' ] },
            { '8', [ '5', '7', '9', '8' ] },
            { '9', [ '6', '8', '9', '9' ] },
        };

        // U, L, R, D
        private static readonly IDictionary<char, char[]> _PartTwoLookup = new Dictionary<char, char[]>
        {
            { '1', [ '1', '1', '1', '3' ] },
            { '2', [ '2', '2', '3', '6' ] },
            { '3', [ '1', '2', '4', '7' ] },
            { '4', [ '4', '3', '4', '8' ] },
            { '5', [ '5', '5', '6', '5' ] },
            { '6', [ '2', '5', '7', 'A' ] },
            { '7', [ '3', '6', '8', 'B' ] },
            { '8', [ '4', '7', '9', 'C' ] },
            { '9', [ '9', '8', '9', '9' ] },
            { 'A', [ '6', 'A', 'B', 'A' ] },
            { 'B', [ '7', 'A', 'C', 'D' ] },
            { 'C', [ '8', 'B', 'C', 'C' ] },
            { 'D', [ 'B', 'D', 'D', 'D' ] },
        };

        private readonly int[][] _instructions = _data
            .Select(line => line
                .Select(_ => (_ - 'A') % 5)
                .ToArray())
            .ToArray();

        [PartOne("48584")]
        [PartTwo("563B6")]
        public async IAsyncEnumerable<string> ComputeAsync()
        {
            var partOneCode = new char[_data.Count()];
            var partTwoCode = new char[_data.Count()];

            char partOne = '5', partTwo = '5';

            for (var index = 0; index < _instructions.Length; index++)
            {
                var instructionSet = _instructions[index];
                foreach (var instruction in instructionSet)
                {
                    partOne = _PartOneLookup[partOne][instruction];
                    partTwo = _PartTwoLookup[partTwo][instruction];
                }

                partOneCode[index] = partOne;
                partTwoCode[index] = partTwo;
            }

            yield return String.Join("", partOneCode);

            yield return String.Join("", partTwoCode);

            await Task.CompletedTask;
        }
    }
}
