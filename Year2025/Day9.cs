using Moyba.AdventOfCode.Utility;

namespace Moyba.AdventOfCode.Year2025
{
    using Range = (int start, int end);
    using Segment = (long index, long start, long end);

    public class Day9(string[] _data) : IPuzzle
    {
        private readonly Coordinate[] _redTiles = _data
            .Select(_ => _.Split(','))
            .Select(_ => new Coordinate(_[0], _[1]))
            .ToArray();

        [PartOne("4749838800")]
        [PartTwo("1624057680")]
        public async IAsyncEnumerable<string?> ComputeAsync()
        {
            var puzzle1 = 0L;
            for (var n1 = 0; n1 < _redTiles.Length - 1; n1++)
            {
                var c1 = _redTiles[n1];
                for (var n2 = n1 + 1; n2 < _redTiles.Length; n2++)
                {
                    var c2 = _redTiles[n2];
                    var area = _GetArea(c1, c2);
                    puzzle1 = Math.Max(puzzle1, area);
                }
            }

            yield return $"{puzzle1}";

            var allX = _redTiles.Select(_ => _.x).Distinct().Order().ToArray();
            var allY = _redTiles.Select(_ => _.y).Distinct().Order().ToArray();

            var xIndices = allX.Select((_, index) => (_, index)).ToDictionary(_ => _.Item1, _ => _.Item2);
            var yIndices = allY.Select((_, index) => (_, index)).ToDictionary(_ => _.Item1, _ => _.Item2);

            var horizontal = Enumerable.Range(0, allY.Length).Select(_ => new List<Range>()).ToArray();
            var vertical = Enumerable.Range(0, allX.Length).Select(_ => new List<Range>()).ToArray();
            for (var index = 0; index < _redTiles.Length; index++)
            {
                var c1 = _redTiles[index];
                var c2 = _redTiles[(index + 1) % _redTiles.Length];
                _AddRange(xIndices[c1.x], yIndices[c1.y], xIndices[c2.x], yIndices[c2.y], horizontal, vertical);
            }

            var rectangles = Enumerable.Range(0, allY.Length - 1).Select(_ => new bool[allX.Length - 1]).ToArray();
            for (var xIndex = 0; xIndex < allX.Length - 1; xIndex++)
            {
                for (var yIndex = 0; yIndex < allY.Length - 1; yIndex++)
                {
                    var hCount = horizontal[0..(yIndex + 1)].Count(_ => _.Any(_ => _.start <= xIndex && _.end > xIndex));
                    if (hCount % 2 != 1) continue;

                    var vCount = vertical[0..(xIndex + 1)].Count(_ => _.Any(_ => _.start <= yIndex && _.end > yIndex));
                    if (vCount % 2 != 1) continue;

                    rectangles[yIndex][xIndex] = true;
                }
            }

            var puzzle2 = 0L;
            for (var n1 = 0; n1 < _redTiles.Length - 1; n1++)
            {
                var c1 = _redTiles[n1];
                for (var n2 = n1 + 1; n2 < _redTiles.Length; n2++)
                {
                    var c2 = _redTiles[n2];
                    var area = _GetArea(c1, c2);
                    if (area <= puzzle2) continue;

                    // check if all sub-rectangles are within the shape
                    var xMin = Math.Min(c1.x, c2.x);
                    var xMax = Math.Max(c1.x, c2.x);
                    var yMin = Math.Min(c1.y, c2.y);
                    var yMax = Math.Max(c1.y, c2.y);
                    if (!Enumerable.Range(xIndices[xMin], xIndices[xMax] - xIndices[xMin]).All(xIndex =>
                        Enumerable.Range(yIndices[yMin], yIndices[yMax] - yIndices[yMin]).All(yIndex => rectangles[yIndex][xIndex]))) continue;

                    puzzle2 = area;
                }
            }

            yield return $"{puzzle2}";

            await Task.CompletedTask;
        }

        private static void _AddRange(int xIndex1, int yIndex1, int xIndex2, int yIndex2, List<Range>[] horizontal, List<Range>[] vertical)
        {
            if (xIndex1 == xIndex2)
            {
                vertical[xIndex1].Add((Math.Min(yIndex1, yIndex2), Math.Max(yIndex1, yIndex2)));
            }
            else if (yIndex1 == yIndex2)
            {
                horizontal[yIndex1].Add((Math.Min(xIndex1, xIndex2), Math.Max(xIndex1, xIndex2)));
            }
            else
            {
                throw new Exception($"Unexpected diagonal line: ({xIndex1}, {yIndex1}), ({xIndex2}, {yIndex2})");
            }
        }

        private static void _GenerateSegment(Coordinate a, Coordinate b, List<Segment> horizontal, List<Segment> vertical)
        {
            if (a.x == b.x)
            {
                vertical.Add((a.x, Math.Min(a.y, b.y), Math.Max(a.y, b.y)));
            }
            else if (a.y == b.y)
            {
                horizontal.Add((a.y, Math.Min(a.x, b.x), Math.Max(a.x, b.x)));
            }
            else
            {
                throw new Exception($"Unexpected diagonal line: {a}, {b}");
            }
        }

        private static long _GetArea(Coordinate a, Coordinate b)
        => (Math.Abs(a.x - b.x) + 1) * (Math.Abs(a.y - b.y) + 1);
    }
}
