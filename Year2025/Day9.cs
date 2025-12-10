using Moyba.AdventOfCode.Utility;

namespace Moyba.AdventOfCode.Year2025
{
    using Segment = (long index, long start, long end);
    using Rectangle = (long xMin, long yMin, long xMax, long yMax);

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

            // create reverse lookups, for perf
            var xIndices = allX.Select((_, index) => (_, index)).ToDictionary(_ => _.Item1, _ => _.Item2);
            var yIndices = allY.Select((_, index) => (_, index)).ToDictionary(_ => _.Item1, _ => _.Item2);

            var horizontal = new List<Segment>(_redTiles.Length >> 1);
            var vertical = new List<Segment>(_redTiles.Length >> 1);
            for (var index = 0; index < _redTiles.Length; index++) _GenerateSegment(_redTiles[index], _redTiles[(index + 1) % _redTiles.Length], horizontal, vertical);

            var rectangles = new HashSet<Rectangle>();
            for (var xIndex = 0; xIndex < allX.Length - 1; xIndex++)
            {
                var xMin = allX[xIndex];
                var xMax = allX[xIndex + 1];

                for (var yIndex = 0; yIndex < allY.Length - 1; yIndex++)
                {
                    var yMin = allY[yIndex];
                    var yMax = allY[yIndex + 1];

                    // count the horizontal segments above yMin, which cover xMin to xMax
                    var horizontalCount = horizontal.Count(_ => _.index <= yMin && _.start <= xMin && _.end >= xMax);
                    if (horizontalCount % 2 != 1) continue;

                    // same thing with vertical
                    var verticalCount = vertical.Count(_ => _.index <= xMin && _.start <= yMin && _.end >= yMax);
                    if (verticalCount % 2 != 1) continue;

                    // success, we found a rectangle that's 'inside'
                    rectangles.Add((xMin, yMin, xMax, yMax));
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
                    for (var xIndex = xIndices[xMin]; xIndex < xIndices[xMax]; xIndex++)
                    {
                        for (var yIndex = yIndices[yMin]; yIndex < yIndices[yMax]; yIndex++)
                        {
                            var rectangle = (allX[xIndex], allY[yIndex], allX[xIndex + 1], allY[yIndex + 1]);
                            if (!rectangles.Contains(rectangle)) goto SkipToTheEnd;
                        }
                    }

                    puzzle2 = area;

                    SkipToTheEnd: ;
                }
            }

            yield return $"{puzzle2}";

            await Task.CompletedTask;
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
