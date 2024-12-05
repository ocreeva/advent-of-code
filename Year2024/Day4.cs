namespace Moyba.AdventOfCode.Year2024
{
    using Index = (int row, int column);

    public class Day4 : IPuzzle
    {
        private static readonly HashSet<Index> _Directions = new HashSet<Index>
        {
            (  0, -1 ),
            (  1, -1 ),
            (  1,  0 ),
            (  1,  1 ),
            (  0,  1 ),
            ( -1,  1 ),
            ( -1,  0 ),
            ( -1, -1 ),
        };

        private static readonly HashSet<Index> _DiagonalDirections = new HashSet<Index>
        {
            (  1, -1 ),
            (  1,  1 ),
            ( -1,  1 ),
            ( -1, -1 ),
        };

        private readonly string[] _data;
        private readonly HashSet<Index> _indicesOfA = new HashSet<Index>();

        public Day4(string[] data)
        {
            // surround the data with two meaningless characters, so I don't have to worry about checking edges
            var lineLength = data[0].Length;
            var emptyLine = new string(Enumerable.Repeat('.', lineLength + 4).ToArray());
            _data = data.Select(_ => $"..{_}..")
                .Prepend(emptyLine)
                .Prepend(emptyLine)
                .Append(emptyLine)
                .Append(emptyLine)
                .ToArray();

            // find all of the 'A' characters in the data
            for (var row = 0; row < data.Length; row++)
            {
                var line = data[row];
                for (var column = 0; column < line.Length; column++)
                {
                    if (line[column] != 'A') continue;

                    // add two to account for the prepended '.' characters
                    _indicesOfA.Add((row + 2, column + 2));
                }
            }
        }

        [PartOne("2358")]
        [PartTwo("1737")]
        public async IAsyncEnumerable<string?> ComputeAsync()
        {
            var part1 = _indicesOfA.Sum(i => this.CountXmasAt(i));

            yield return $"{part1}";

            var part2 = _indicesOfA.Count(i => this.HasXMasAt(i));

            yield return $"{part2}";

            await Task.CompletedTask;
        }

        private int CountXmasAt(Index indexOfA)
            => _Directions.Count(d => this.DataHasMAS(indexOfA, d.row, d.column, includeXCheck: true));

        private bool HasXMasAt(Index indexOfA)
            => _DiagonalDirections.Count(d => this.DataHasMAS(indexOfA, d.row, d.column, includeXCheck: false)) == 2;

        private bool DataHasMAS(Index indexOfA, int rowOffset, int columnOffset, bool includeXCheck)
            => _data[indexOfA.row + rowOffset][indexOfA.column + columnOffset] == 'S'
            && _data[indexOfA.row - rowOffset][indexOfA.column - columnOffset] == 'M'
            && (!includeXCheck || _data[indexOfA.row - 2 * rowOffset][indexOfA.column - 2 * columnOffset] == 'X');
    }
}
