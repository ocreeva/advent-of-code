namespace Moyba.AdventOfCode.Year2022
{
    public class Day6(string[] _data) : IPuzzle
    {
        private readonly string _stream = _data.Single();

        [PartOne("1356")]
        [PartTwo("2564")]
        public async IAsyncEnumerable<string?> ComputeAsync()
        {
            var marker = 0;
            for ( ; _stream[marker..(marker+4)].Distinct().Count() < 4; marker++) { }

            yield return $"{marker + 4}";

            for ( ; _stream[marker..(marker+14)].Distinct().Count() < 14; marker++) { }

            yield return $"{marker + 14}";

            await Task.CompletedTask;
        }
    }
}
