namespace Moyba.AdventOfCode.Year2025
{
    public class Day3(string[] _data) : IPuzzle
    {
        [PartOne("17430")]
        [PartTwo("171975854269367")]
        public async IAsyncEnumerable<string?> ComputeAsync()
        {
            var puzzle1 = _data.Sum(_ => _GetMaximumJoltage(_, 2));

            yield return $"{puzzle1}";

            var puzzle2 = _data.Sum(_ => _GetMaximumJoltage(_, 12));

            yield return $"{puzzle2}";

            await Task.CompletedTask;
        }

        private static long _GetMaximumJoltage(string bank, int batteryCount, int startIndex = 0, long currentVoltage = 0L)
        {
            if (batteryCount == 0) return currentVoltage;

            var battery = '0';
            for (var index = startIndex; index <= bank.Length - batteryCount; index++)
            {
                if (bank[index] <= battery) continue;

                battery = bank[index];
                startIndex = index;
            }

            return _GetMaximumJoltage(bank, batteryCount - 1, startIndex + 1, 10 * currentVoltage + (battery - '0'));
        }
    }
}
