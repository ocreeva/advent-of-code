using System.Text.RegularExpressions;

namespace Moyba.AdventOfCode.Year2015
{
    public class Day9 : SolutionBase
    {
        private static readonly Regex Parser = new Regex(@"^(.+) to (.+) = (\d+)$", RegexOptions.Compiled);
        private readonly IDictionary<int, IDictionary<int, long>> _data = new Dictionary<int, IDictionary<int, long>>();

        [Expect("207")]
        protected override string SolvePart1()
        {
            var shortest = _data.Keys.Min(start => FindShortestDistance(start, 0));
            return $"{shortest}";
        }

        [Expect("804")]
        protected override string SolvePart2()
        {
            var longest = _data.Keys.Max(start => FindLongestDistance(start, 0));
            return $"{longest}";
        }

        private long FindShortestDistance(int current, int visited)
        {
            var shortest = long.MaxValue;
            var distances = _data[current];
            foreach (var destination in distances.Keys)
            {
                if ((destination & visited) != 0) continue;

                var distance = distances[destination] + FindShortestDistance(destination, visited | current);
                shortest = Math.Min(shortest, distance);
            }

            if (shortest == long.MaxValue) return 0;
            return shortest;
        }

        private long FindLongestDistance(int current, int visited)
        {
            var longest = 0L;
            var distances = _data[current];
            foreach (var destination in distances.Keys)
            {
                if ((destination & visited) != 0) continue;

                var distance = distances[destination] + FindLongestDistance(destination, visited | current);
                longest = Math.Max(longest, distance);
            }

            return longest;
        }

        protected override void TransformData(IEnumerable<string> data)
        {
            var locationKey = 0x1;
            var locations = new Dictionary<string, int>();

            foreach (var value in data)
            {
                var match = Parser.Match(value);
                if (!match.Success) throw new Exception($"Unhandled input: {value}");

                var city1 = match.Groups[1].Value;
                if (!locations.ContainsKey(city1))
                {
                    _data[locationKey] = new Dictionary<int, long>();
                    locations[city1] = locationKey;
                    locationKey <<= 1;
                }

                var city2 = match.Groups[2].Value;
                if (!locations.ContainsKey(city2))
                {
                    _data[locationKey] = new Dictionary<int, long>();
                    locations[city2] = locationKey;
                    locationKey <<= 1;
                }

                var distance = Int64.Parse(match.Groups[3].Value);
                _data[locations[city1]][locations[city2]] = distance;
                _data[locations[city2]][locations[city1]] = distance;
            }
        }
    }
}
