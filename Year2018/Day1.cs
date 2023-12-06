namespace Moyba.AdventOfCode.Year2018
{
    public class Day1(string[] data) : IPuzzle
    {
        private readonly long[] _data = data.Select(Int64.Parse).ToArray();

        [PartOne("547")]
        [PartTwo("76414")]
        public async IAsyncEnumerable<string> ComputeAsync()
        {
            var frequency = 0L;
            long? duplicate = null;

            var reached = new HashSet<long> { frequency };

            for (var index = 0; index < _data.Length; index++)
            {
                frequency += _data[index];

                if (reached.Contains(frequency) && !duplicate.HasValue)
                {
                    duplicate = frequency;
                }

                reached.Add(frequency);
            }

            yield return $"{frequency}";

            while (!duplicate.HasValue)
            {
                for (var index = 0; index < _data.Length; index++)
                {
                    frequency += _data[index];

                    if (reached.Contains(frequency))
                    {
                        duplicate = frequency;
                        break;
                    }

                    reached.Add(frequency);
                }
            }

            yield return $"{duplicate}";

            await Task.CompletedTask;
        }
    }
}
