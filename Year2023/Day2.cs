namespace Moyba.AdventOfCode.Year2023
{
    using Game = (int number, int[][] rounds);

    public class Day2(string[] data) : IPuzzle
    {
        private const int _RedIndex = 0;
        private const int _GreenIndex = 1;
        private const int _BlueIndex = 2;

        private const int _RedLimit = 12;
        private const int _GreenLimit = 13;
        private const int _BlueLimit = 14;

        private readonly Game[] _games = data
            .Select(_ => _.Split(": "))
            .Select(_ => (
                Int32.Parse(_[0]["Game ".Length..]),
                _[1].Split("; ")
                    .Select(_ => _.Split(", "))
                    .Select(_ => {
                        var cubes = new int[3];
                        foreach (var entry in _)
                        {
                            var parts = entry.Split(' ');
                            cubes[_GetColorIndex(parts[1])] = Int32.Parse(parts[0]);
                        }

                        return cubes;
                    })
                    .ToArray()
            ))
            .ToArray();

        [PartOne("2268")]
        [PartTwo("63542")]
        public async IAsyncEnumerable<string> ComputeAsync()
        {
            var possibleGames = 0;
            var minimumCubes = 0;

            foreach (var game in _games)
            {
                var maxRed = game.rounds.Max(_ => _[_RedIndex]);
                var maxGreen = game.rounds.Max(_ => _[_GreenIndex]);
                var maxBlue = game.rounds.Max(_ => _[_BlueIndex]);

                if (maxRed <= _RedLimit && maxGreen <= _GreenLimit && maxBlue <= _BlueLimit) possibleGames += game.number;

                minimumCubes += maxRed * maxGreen * maxBlue;
            }

            yield return $"{possibleGames}";

            yield return $"{minimumCubes}";

            await Task.CompletedTask;
        }

        private static int _GetColorIndex(string color) => color switch
        {
            "red" => _RedIndex,
            "green" => _GreenIndex,
            "blue" => _BlueIndex,
            _ => throw new Exception($"Unhandled color: {color}")
        };
    }
}
