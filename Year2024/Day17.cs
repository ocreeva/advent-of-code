using System.Text.RegularExpressions;

namespace Moyba.AdventOfCode.Year2024
{
    public class Day17 : IPuzzle
    {
        private static readonly Regex _RegisterParser = new Regex(@"^Register (?<id>.): (?<value>\d+)$", RegexOptions.Compiled);

        private readonly long[] _program;

        private long _registerA, _registerB, _registerC;
        private long _pointer = 0;
        private IList<long> _output;

        public Day17(string[] data)
        {
            var aMatch = _RegisterParser.Match(data[0]);
            _registerA = Int64.Parse(aMatch.Groups["value"].Value);

            var bMatch = _RegisterParser.Match(data[1]);
            _registerB = Int64.Parse(bMatch.Groups["value"].Value);

            var cMatch = _RegisterParser.Match(data[2]);
            _registerC = Int64.Parse(cMatch.Groups["value"].Value);

            _program = data[4]["Program: ".Length..].Split(',').Select(Int64.Parse).ToArray();
            _output = new List<long>(_program.Length);
        }

        [PartOne("7,5,4,3,4,5,3,4,6")]
        [PartTwo("164278899142333")]
        public async IAsyncEnumerable<string?> ComputeAsync()
        {
            while (_pointer < _program.Length) this.OperateProgram();

            yield return String.Join(',', _output);

            this.TryOutputValue(_program.Length - 1, 0, out long part2);
            yield return $"{part2}";

            await Task.CompletedTask;
        }

        private bool TryOutputValue(int index, long initialA, out long finalA)
        {
            finalA = 0;

            initialA <<= 3;
            for (var iteration = 0; iteration < 8; iteration++)
            {
                _output.Clear();
                _pointer = 0;
                _registerA = initialA + iteration;

                while (_output.Count == 0) this.OperateProgram();
                if (_output[0] != _program[index]) continue;

                if (index == 0)
                {
                    finalA = initialA + iteration;
                    return true;
                }

                if (this.TryOutputValue(index - 1, initialA + iteration, out finalA)) return true;
            }

            return false;
        }

        private void OperateProgram()
        {
            var instruction = _program[_pointer++];
            var operand = _program[_pointer++];

            switch (instruction)
            {
                case 0:
                    _registerA = _registerA / (long)Math.Pow(2, this.Combo(operand));
                    break;

                case 1:
                    _registerB = _registerB ^ operand;
                    break;

                case 2:
                    _registerB = this.Combo(operand) % 8;
                    break;

                case 3:
                    if (_registerA != 0) _pointer = operand;
                    break;

                case 4:
                    _registerB = _registerB ^ _registerC;
                    break;

                case 5:
                    _output.Add(this.Combo(operand) % 8);
                    break;

                case 6:
                    _registerB = _registerA / (long)Math.Pow(2, this.Combo(operand));
                    break;

                case 7:
                    _registerC = _registerA / (long)Math.Pow(2, this.Combo(operand));
                    break;

                default:
                    throw new NotSupportedException($"Unsupported instruction value ({instruction}).");
            }
        }

        private long Combo(long operand) => operand switch
        {
            0 => 0,
            1 => 1,
            2 => 2,
            3 => 3,
            4 => _registerA,
            5 => _registerB,
            6 => _registerC,
            _ => throw new NotSupportedException($"Unsupported combo operand value ({operand}).")
        };
    }
}
