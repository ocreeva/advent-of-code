namespace Moyba.AdventOfCode.Year2022
{
    public class Day1(string[] data) : IPuzzle
    {
        private readonly long[][] _elves = data.Cluster().Select(_ => _.Select(Int64.Parse).ToArray()).ToArray();

        [PartOne("71924")]
        [PartTwo("210406")]
        public async IAsyncEnumerable<string> ComputeAsync()
        {
            var calories = _elves.Select(_ => _.Sum()).OrderDescending().Take(3).ToArray();

            yield return $"{calories[0]}";

            yield return $"{calories.Sum()}";

            await Task.CompletedTask;
        }
    }
}
