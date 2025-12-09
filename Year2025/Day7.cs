namespace Moyba.AdventOfCode.Year2025
{
    public class Day7(string[] _data) : IPuzzle
    {
        private readonly int _startColumn = _data[0].IndexOf('S');
        private readonly HashSet<int>[] _splitters = _data
            .Select(_ => _.Select((c, i) => c == '^' ? i : -1).Where(_ => _ != -1).ToHashSet())
            .ToArray();

        [PartOne("1579")]
        [PartTwo("13418215871354")]
        public async IAsyncEnumerable<string?> ComputeAsync()
        {
            var puzzle1 = 0;
            var beamTimelines = new Dictionary<int, long> { { _startColumn, 1L } };
            for (var row = 1; row < _splitters.Length; row++)
            {
                var intersections = _splitters[row].Intersect(beamTimelines.Keys).ToArray();
                puzzle1 += intersections.Length;

                foreach (var intersection in intersections)
                {
                    if (!beamTimelines.ContainsKey(intersection - 1)) beamTimelines[intersection - 1] = 0L;
                    if (!beamTimelines.ContainsKey(intersection + 1)) beamTimelines[intersection + 1] = 0L;

                    beamTimelines[intersection - 1] += beamTimelines[intersection];
                    beamTimelines[intersection + 1] += beamTimelines[intersection];
                    beamTimelines.Remove(intersection);
                }
            }

            yield return $"{puzzle1}";

            var puzzle2 = beamTimelines.Values.Sum();

            yield return $"{puzzle2}";

            await Task.CompletedTask;
        }
    }
}
