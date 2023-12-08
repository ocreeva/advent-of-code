namespace Moyba.AdventOfCode.Year2022
{
    public class Day3(string[] _data) : IPuzzle
    {
        private readonly int[][] _rucksacks = _data
            .Select(_ => _.Select(_TransformPriority).ToArray())
            .ToArray();

        [PartOne("7863")]
        [PartTwo("2488")]
        public async IAsyncEnumerable<string> ComputeAsync()
        {
            var priority = 0;
            foreach (var rucksack in _rucksacks)
            {
                var compartmentSize = rucksack.Length >> 1;
                priority += rucksack[..compartmentSize].Intersect(rucksack[^compartmentSize..]).Single();
            }

            yield return $"{priority}";

            priority = 0;
            for (var index = 0; index < _rucksacks.Length; )
            {
                priority += _rucksacks[index++].Intersect(_rucksacks[index++]).Intersect(_rucksacks[index++]).Single();
            }

            yield return $"{priority}";

            await Task.CompletedTask;
        }

        private static int _TransformPriority(char c) => Char.IsLower(c) ? (1 + c - 'a') : (27 + c - 'A');
    }
}
