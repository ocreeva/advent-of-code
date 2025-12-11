using Microsoft.Z3;

namespace Moyba.AdventOfCode.Year2025
{
    using Machine = (long lights, long[] buttons, int[] joltage);
    using LightState = (long lights, int depth, int index);

    public class Day10(string[] _data) : IPuzzle
    {
        private readonly Machine[] _machines = _data.Select(_ParseMachine).ToArray();

        [PartOne("530")]
        [PartTwo("20172")]
        public async IAsyncEnumerable<string?> ComputeAsync()
        {
            var puzzle1 = _machines.Sum(_FindMinimumButtonCombinationForLights);

            yield return $"{puzzle1}";

            var puzzle2 = _machines.Sum(_FindMinimumButtonCombinationForJoltage);

            yield return $"{puzzle2}";

            await Task.CompletedTask;
        }

        private static int _FindMinimumButtonCombinationForLights(Machine machine)
        {
            var queue = new Queue<LightState>();
            queue.Enqueue((0, 0, 0));

            while (true)
            {
                var state = queue.Dequeue();
                for (var index = state.index; index < machine.buttons.Length; index++)
                {
                    var depth = state.depth + 1;

                    var lights = state.lights ^ machine.buttons[index];
                    if (lights == machine.lights) return depth;

                    queue.Enqueue((lights, depth, index + 1));
                }
            }
        }

        private static int _FindMinimumButtonCombinationForJoltage(Machine machine)
        {
            using (var context = new Context())
            {
                var buttonVariables = machine.buttons.Select((_, i) => context.MkIntConst($"x{i}")).ToArray();

                var optimize = context.MkOptimize();
                foreach (var buttonVariable in buttonVariables)
                {
                    optimize.Assert(context.MkGe(buttonVariable, context.MkInt(0)));
                }

                var joltageButtonVariables = Enumerable.Range(0, machine.joltage.Length)
                    .Select(_ => 1 << _)
                    .Select(_ => buttonVariables.Where((b, i) => (machine.buttons[i] & _) != 0))
                    .ToArray();
                for (var index = 0; index < machine.joltage.Length; index++)
                {
                    var joltage = 1 << index;
                    var relevantButtonVariables = buttonVariables.Where((_, i) => (machine.buttons[i] & joltage) != 0);
                    optimize.Assert(context.MkEq(context.MkAdd(relevantButtonVariables), context.MkInt(machine.joltage[index])));
                }

                optimize.MkMinimize(context.MkAdd(buttonVariables));

                if (optimize.Check() != Status.SATISFIABLE) throw new Exception($"Unable to find solution.");

                var model = optimize.Model;
                var result = buttonVariables.Sum(_ => ((IntNum)model.Evaluate(_)).Int);
                return result;
            }
        }

        private static Machine _ParseMachine(string data)
        => _ParseMachine(data.Split(' '));

        private static Machine _ParseMachine(string[] parts)
        => (
            _ParseLights(parts[0]),
            parts[1..^1].Select(_ParseButton).ToArray(),
            _ParseJoltage(parts[^1])
        );

        private static long _ParseLights(string data)
        => data[1..^1].Select((_, i) => (_ == '#' ? 1L : 0L) << i).Sum();

        private static long _ParseButton(string data)
        => data[1..^1].Split(',').Select(Int32.Parse).Sum(_ => 1 << _);

        private static int[] _ParseJoltage(string data)
        => data[1..^1].Split(',').Select(Int32.Parse).ToArray();
    }
}
