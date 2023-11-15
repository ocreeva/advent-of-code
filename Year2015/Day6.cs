using System.Text.RegularExpressions;

using Coord = (int x, int y);

namespace Moyba.AdventOfCode.Year2015
{
    public class Day6 : SolutionBase
    {
        private static readonly Regex Parser = new Regex(@"^(turn on|turn off|toggle) (\d+),(\d+) through (\d+),(\d+)$", RegexOptions.Compiled);
        private static readonly IDictionary<string, Operation> OperationLookup = new Dictionary<string, Operation>
        {
            { "turn on", Operation.TurnOn },
            { "turn off", Operation.TurnOff },
            { "toggle", Operation.Toggle },
        };

        private (Operation op, Coord from, Coord to)[] _data = Array.Empty<(Operation, Coord, Coord)>();

        [Expect("569999")]
        protected override string SolvePart1()
        {
            var result = this.OperateLights(
                (int value) => 1,
                (int value) => 0,
                (int value) => 1 - value
            );

            return $"{result}";
        }

        [Expect("17836115")]
        protected override string SolvePart2()
        {
            var result = this.OperateLights(
                (int value) => value + 1,
                (int value) => Math.Max(0, value - 1),
                (int value) => value + 2
            );

            return $"{result}";
        }

        private int OperateLights(Func<int, int> turnOn, Func<int, int> turnOff, Func<int, int> toggle)
        {
            var lights = new int[1_000_000];

            foreach (var instruction in _data)
            {
                Func<int, int> action;
                switch (instruction.op)
                {
                    case Operation.TurnOn:
                        action = turnOn;
                        break;

                    case Operation.TurnOff:
                        action = turnOff;
                        break;

                    case Operation.Toggle:
                        action = toggle;
                        break;

                    default:
                        throw new Exception($"Unsupported operation: {instruction.op}");
                }

                for (var y = instruction.from.y; y <= instruction.to.y; y++)
                {
                    for (int x = instruction.from.x, index = y * 1_000 + x; x <= instruction.to.x; x++, index++)
                    {
                        lights[index] = action(lights[index]);
                    }
                }
            }

            return lights.Sum();
        }

        protected override void TransformData(IEnumerable<string> data) => _data = data
            .Select(d => {
                var match = Day6.Parser.Match(d);
                return (
                    OperationLookup[match.Groups[1].Value],
                    (Int32.Parse(match.Groups[2].Value), Int32.Parse(match.Groups[3].Value)),
                    (Int32.Parse(match.Groups[4].Value), Int32.Parse(match.Groups[5].Value))
                );
            })
            .ToArray();

        private enum Operation
        {
            TurnOn,
            TurnOff,
            Toggle,
        }
    }
}
