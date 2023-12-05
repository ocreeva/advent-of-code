namespace Moyba.AdventOfCode.Year2018
{
    public class Day1(string[] data) : IPuzzle
    {
        private readonly long[] _data = data.Select(Int64.Parse).ToArray();

        private long _frequency;
        private long? _duplicate;

        public Task ComputeAsync()
        {
            var reached = new HashSet<long> { _frequency };

            for (var index = 0; index < _data.Length; index++)
            {
                _frequency += _data[index];

                if (reached.Contains(_frequency) && !_duplicate.HasValue)
                {
                    _duplicate = _frequency;
                }

                reached.Add(_frequency);
            }

            var frequency = _frequency;
            while (!_duplicate.HasValue)
            {
                for (var index = 0; index < _data.Length; index++)
                {
                    frequency += _data[index];

                    if (reached.Contains(frequency))
                    {
                        _duplicate = frequency;
                        break;
                    }

                    reached.Add(frequency);
                }
            }

            return Task.CompletedTask;
        }

        [Solution("547")]
        public string PartOne => $"{_frequency}";

        [Solution("76414")]
        public string PartTwo => $"{_duplicate}";
    }
}
