namespace Moyba.AdventOfCode.Year2015
{
    public class Day20 : SolutionBase<string>
    {
        private int _presents;

        protected override string ReadInput(IEnumerable<string> input) => input.First();

        [Expect("776160")]
        protected override string SolvePart1()
        {
            // automatically apply elf #1
            var target = _presents / 10 - 1;

            var limit = target >> 2;
            var houses = new long[limit];
            for (var elf = 2; elf < limit; elf++)
            {
                for (var index = elf; index < limit; index += elf)
                {
                    houses[index] += elf;
                }

                if (houses[elf] >= target) return $"{elf}";
            }

            throw new Exception($"No solution found within limit ({limit}) for presents target of {_presents}.");
        }

        [Expect("786240")]
        protected override string SolvePart2()
        {
            // automatically apply elf #1
            var target = _presents / 11 - 1;

            var limit = target >> 1;
            var houses = new long[limit];
            for (var elf = 2; elf < limit; elf++)
            {
                for (int index = elf, house = 0; index < limit && house < 50; index += elf, house++)
                {
                    houses[index] += elf;
                }

                if (houses[elf] >= target) return $"{elf}";
            }

            throw new Exception($"No solution found within limit ({limit}) for presents target of {_presents}.");
        }

        protected override void TransformData(string data) => _presents = Int32.Parse(data);
    }
}
