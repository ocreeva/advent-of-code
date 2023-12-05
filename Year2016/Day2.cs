namespace Moyba.AdventOfCode.Year2016
{
    public class Day2(string[] data) : IPuzzle
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

        private readonly int[][] _instructions = data
            .Select(line => line
                .Select(_ => (_ - 'A') % 5)
                .ToArray())
            .ToArray();

        private readonly char[] _partOneCode = new char[data.Count()];
        private readonly char[] _partTwoCode = new char[data.Count()];

        public Task ComputeAsync()
        {
            char partOne = '5', partTwo = '5';

            for (var index = 0; index < _instructions.Length; index++)
            {
                var instructionSet = _instructions[index];
                foreach (var instruction in instructionSet)
                {
                    partOne = _PartOneLookup[partOne][instruction];
                    partTwo = _PartTwoLookup[partTwo][instruction];
                }

                _partOneCode[index] = partOne;
                _partTwoCode[index] = partTwo;
            }

            return Task.CompletedTask;
        }

        [Solution("48584")]
        public string PartOne => String.Join("", _partOneCode);

        [Solution("563B6")]
        public string PartTwo => String.Join("", _partTwoCode);
    }
}
