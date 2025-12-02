namespace Moyba.AdventOfCode.Year2025
{
    using Rotation = (int sign, int value);

    public class Day1(string[] _data) : IPuzzle
    {
        private readonly Rotation[] _rotations = _data
            .Select(_ParseRotation)
            .ToArray();

        [PartOne("1154")]
        [PartTwo()]
        public async IAsyncEnumerable<string?> ComputeAsync()
        {
            var dial = 50;
            var puzzle1 = 0;
            var puzzle2 = 0;
            foreach (var rotation in _rotations)
            {
                if (rotation.sign == -1 && dial > 0) dial = 100 - dial;

                var next = dial + rotation.value;
                dial = next % 100;

                puzzle2 += (next - dial) / 100;

                if (rotation.sign == -1 && dial > 0) dial = 100 - dial;

                if (dial == 0) puzzle1++;
            }

            yield return $"{puzzle1}";

            yield return $"{puzzle2}";

            await Task.CompletedTask;
        }

        private static Rotation _ParseRotation(string data)
        => data[0] switch
        {
            'L' => (-1, Int32.Parse(data[1..])),
            'R' => ( 1, Int32.Parse(data[1..])),
            _   => throw new Exception($"Unexpected input format: {data}")
        };
    }
}
