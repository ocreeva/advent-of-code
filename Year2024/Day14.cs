using System.Text.RegularExpressions;
using Moyba.AdventOfCode.Utility;

namespace Moyba.AdventOfCode.Year2024
{
    using Robot = (Coordinate position, Coordinate velocity);
    using Quadrants = (long nw, long ne, long se, long sw);

    public class Day14(string[] _data) : IPuzzle
    {
        private static readonly Regex _RobotParser = new Regex(@"^p=(?<px>\d+),(?<py>\d+) v=(?<vx>-?\d+),(?<vy>-?\d+)$", RegexOptions.Compiled);
        private static readonly long _Height = 103, _Width = 101;
        private static readonly long _HalfHeight = _Height >> 1, _HalfWidth = _Width >> 1;

        private readonly Robot[] _robots = _data
            .Select(_ => _RobotParser.Match(_))
            .Select(_ => (new Coordinate(_.Groups["px"], _.Groups["py"]), new Coordinate(_.Groups["vx"], _.Groups["vy"])))
            .ToArray();

        [PartOne("222208000")]
        [PartTwo("7623")]
        public async IAsyncEnumerable<string?> ComputeAsync()
        {
            var positions = _robots.Select(_ => _ProjectPosition(_, 100));
            var quadrants = _EvaluateQuadrants(positions);

            yield return $"{quadrants.nw * quadrants.ne * quadrants.se * quadrants.sw}";

            int part2;
            var clusterSize = _robots.Length >> 1;
            for (part2 = 101; true; part2++)
            {
                positions = _robots.Select(_ => _ProjectPosition(_, part2));
                quadrants = _EvaluateQuadrants(positions);

                if (quadrants.nw > clusterSize ||
                    quadrants.ne > clusterSize ||
                    quadrants.se > clusterSize ||
                    quadrants.sw > clusterSize) break;
            }

            yield return $"{part2}";

            await Task.CompletedTask;
        }

        private static Coordinate _ProjectPosition(Robot robot, long seconds)
            => new Coordinate(
                (robot.position.x + (robot.velocity.x * seconds % _Width) + _Width) % _Width,
                (robot.position.y + (robot.velocity.y * seconds % _Height) + _Height) % _Height);

        private static Quadrants _EvaluateQuadrants(IEnumerable<Coordinate> positions)
        {
            int nw = 0, ne = 0, se = 0, sw = 0;
            foreach (var position in positions)
            {
                switch (Math.Sign(position.x - _HalfWidth))
                {
                    case -1:
                        switch (Math.Sign(position.y - _HalfHeight))
                        {
                            case -1:
                                nw++;
                                break;

                            case 1:
                                sw++;
                                break;
                        }
                        break;

                    case 1:
                        switch (Math.Sign(position.y - _HalfHeight))
                        {
                            case -1:
                                ne++;
                                break;

                            case 1:
                                se++;
                                break;
                        }
                        break;
                }
            }

            return (nw, ne, se, sw);
        }
    }
}
