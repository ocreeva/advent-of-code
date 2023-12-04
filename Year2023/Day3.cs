namespace Moyba.AdventOfCode.Year2023
{
    using Coord = (int x, int y);

    public class Day3(IEnumerable<string> data) : IPuzzle
    {
        private readonly string[] _data = data.ToArray();
        private readonly int _lineLength = data.First().Length;

        private int _sumOfParts;
        private int _gearRatios;

        public Task ComputeAsync()
        {
            var gears = new Dictionary<Coord, IList<int>>();

            for (var lineIndex = 0; lineIndex < _data.Length; lineIndex++)
            {
                var line = _data[lineIndex];
                for (var characterIndex = 0; characterIndex < line.Length; characterIndex++)
                {
                    var character = line[characterIndex];
                    if (!Char.IsDigit(character)) continue;

                    var number = 0;
                    var startIndex = characterIndex;
                    while (Char.IsDigit(character))
                    {
                        number = 10 * number + (character - '0');

                        if (++characterIndex >= _lineLength) break;
                        character = line[characterIndex];
                    }

                    var adjacentSymbols = this.FindAdjacentSymbols(lineIndex, startIndex, characterIndex - 1).ToArray();
                    if (adjacentSymbols.Length > 0) _sumOfParts += number;
                    foreach (var gearLocation in adjacentSymbols.Where(_ => _.value == '*').Select(_ => _.location))
                    {
                        if (!gears.ContainsKey(gearLocation)) gears.Add(gearLocation, new List<int>());
                        gears[gearLocation].Add(number);
                    }
                }
            }

            _gearRatios = gears.Values.Where(_ => _.Count == 2).Select(_ => _[0] * _[1]).Sum();

            return Task.CompletedTask;
        }

        [Solution("556367")]
        public string SolvePartOne() => $"{_sumOfParts}";

        [Solution("89471771")]
        public string SolvePartTwo() => $"{_gearRatios}";

        private IEnumerable<(char value, Coord location)> FindAdjacentSymbols(int lineIndex, int startIndex, int endIndex)
        {
            char symbol;
            if (this.TryGetSymbol(lineIndex, startIndex - 1, out symbol)) yield return (symbol, (lineIndex, startIndex - 1));
            if (this.TryGetSymbol(lineIndex, endIndex + 1, out symbol)) yield return (symbol, (lineIndex, endIndex + 1));

            foreach (var characterIndex in Enumerable.Range(startIndex - 1, endIndex - startIndex + 3))
            {
                if (this.TryGetSymbol(lineIndex - 1, characterIndex, out symbol)) yield return (symbol, (lineIndex - 1, characterIndex));
                if (this.TryGetSymbol(lineIndex + 1, characterIndex, out symbol)) yield return (symbol, (lineIndex + 1, characterIndex));
            }
        }

        private bool TryGetSymbol(int lineIndex, int characterIndex, out char symbol)
        {
            symbol = '.';

            if (lineIndex < 0) return false;
            if (lineIndex >= _data.Length) return false;

            if (characterIndex < 0) return false;
            if (characterIndex >= _lineLength) return false;

            symbol = _data[lineIndex][characterIndex];
            return symbol != '.' && !Char.IsDigit(symbol);
        }
    }
}
