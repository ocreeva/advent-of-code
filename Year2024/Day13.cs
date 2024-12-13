using System.Text.RegularExpressions;
using Moyba.AdventOfCode.Utility;

namespace Moyba.AdventOfCode.Year2024
{
    using Machine = (Coordinate buttonA, Coordinate buttonB, Coordinate prize);
    using Presses = (bool isValid, long pressesA, long pressesB);

    public class Day13(string[] _data) : IPuzzle
    {
        private static readonly Regex _ButtonParser = new Regex(@"^Button (?:A|B): X\+(?<X>\d+), Y\+(?<Y>\d+)$", RegexOptions.Compiled);
        private static readonly Regex _PrizeParser = new Regex(@"^Prize: X=(?<X>\d+), Y=(?<Y>\d+)$", RegexOptions.Compiled);

        private static readonly Coordinate _PrizeConversion = new Coordinate(10_000_000_000_000, 10_000_000_000_000);

        private readonly Machine[] _machines = Enumerable
            .Range(0, (_data.Length + 1) >> 2)
            .Select(_ => _ << 2)
            .Select(_ => (_Parse(_data[_], _ButtonParser), _Parse(_data[_ + 1], _ButtonParser), _Parse(_data[_ + 2], _PrizeParser)))
            .ToArray();

        [PartOne("31065")]
        [PartTwo("93866170395343")]
        public async IAsyncEnumerable<string?> ComputeAsync()
        {
            var part1 = _machines
                .Select(_CalculatePresses)
                .Where(_ => _.isValid)
                .Sum(_ => 3 * _.pressesA + _.pressesB);

            yield return $"{part1}";

            var part2 = _machines
                .Select(_ => (_.buttonA, _.buttonB, _.prize + _PrizeConversion))
                .Select(_CalculatePresses)
                .Where(_ => _.isValid)
                .Sum(_ => _.pressesA * 3 + _.pressesB);

            yield return $"{part2}";

            await Task.CompletedTask;
        }

        private static Presses _CalculatePresses(Machine machine)
        {
            (var buttonA, var buttonB, var prize) = machine;

            var divisor = buttonA.y * buttonB.x - buttonA.x * buttonB.y;
            var numeratorA = prize.y * buttonB.x - prize.x * buttonB.y;
            var numeratorB = prize.x * buttonA.y - prize.y * buttonA.x;

            if (numeratorA % divisor != 0) return (false, 0, 0);
            if (numeratorB % divisor != 0) return (false, 0, 0);

            var pressesA = numeratorA / divisor;
            if (pressesA < 0) return (false, 0, 0);

            var pressesB = numeratorB / divisor;
            if (pressesB < 0) return (false, 0, 0);

            return (true, pressesA, pressesB);
        }

        private static Coordinate _Parse(string text, Regex parser)
        {
            var match = parser.Match(text);
            return new Coordinate(Int64.Parse(match.Groups["X"].Value), Int64.Parse(match.Groups["Y"].Value));
        }
    }
}
