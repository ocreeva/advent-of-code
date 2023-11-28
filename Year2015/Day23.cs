namespace Moyba.AdventOfCode.Year2015
{
    public class Day23 : SolutionBase
    {
        private delegate void Instruction(ref int index);

        private Instruction[] _program = Array.Empty<Instruction>();
        private IDictionary<string, int> _registers = new Dictionary<string, int>();

        [Expect("255")]
        protected override string SolvePart1()
        {
            _registers.Clear();
            _registers["a"] = 0;
            _registers["b"] = 0;

            for (var index = 0; index < _program.Length; )
            {
                _program[index](ref index);
            }

            var b = _registers["b"];
            return $"{b}";
        }

        [Expect("334")]
        protected override string SolvePart2()
        {
            _registers.Clear();
            _registers["a"] = 1;
            _registers["b"] = 0;

            for (var index = 0; index < _program.Length; )
            {
                _program[index](ref index);
            }

            var b = _registers["b"];
            return $"{b}";
        }

        protected override void TransformData(IEnumerable<string> data)
        {
            _program = data
                .Select<string, Instruction>(line => {
                    int offset;
                    var parts = line.Split(' ', 2);
                    switch (parts[0])
                    {
                        case "hlf":
                            return (ref int index) => {
                                index++;
                                _registers[parts[1]] >>= 1;
                            };
                        
                        case "tpl":
                            return (ref int index) => {
                                index++;
                                _registers[parts[1]] *= 3;
                            };

                        case "inc":
                            return (ref int index) => {
                                index++;
                                _registers[parts[1]]++;
                            };

                        case "jmp":
                            offset = Int32.Parse(parts[1]);
                            return (ref int index) => {
                                index += offset;
                            };

                        case "jie":
                            parts = parts[1].Split(", ");
                            offset = Int32.Parse(parts[1]);
                            return (ref int index) => {
                                if ((_registers[parts[0]] % 2) == 0) index += offset;
                                else index++;
                            };

                        case "jio":
                            parts = parts[1].Split(", ");
                            offset = Int32.Parse(parts[1]);
                            return (ref int index) => {
                                if (_registers[parts[0]] == 1) index += offset;
                                else index++;
                            };

                        default:
                            throw new Exception($"Unhandled operation: {parts[0]}");
                    }
                })
                .ToArray();
        }
    }
}
