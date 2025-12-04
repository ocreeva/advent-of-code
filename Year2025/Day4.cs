using Moyba.AdventOfCode.Utility;

namespace Moyba.AdventOfCode.Year2025
{
    public class Day4 : IPuzzle
    {
        private readonly IDictionary<Coordinate, HashSet<Coordinate>> _adjacency;

        public Day4(string[] data)
        {
            var coordinates = data.SelectMany(_GetRollCoordinates).ToHashSet();
            _adjacency = coordinates.ToDictionary(
                _ => _,
                _ => _.Adjacent.Where(coordinates.Contains).ToHashSet());
        }

        [PartOne("1478")]
        [PartTwo("9120")]
        public async IAsyncEnumerable<string?> ComputeAsync()
        {
            var accessible = _adjacency.Keys.Where(_ => _adjacency[_].Count < 4).ToArray();
            var puzzle1 = accessible.Length;

            yield return $"{puzzle1}";

            var originalCount = _adjacency.Count;
            var queue = new Queue<Coordinate>(accessible);
            while (queue.TryDequeue(out Coordinate coordinate))
            {
                foreach (var adjacent in _adjacency[coordinate])
                {
                    _adjacency[adjacent].Remove(coordinate);
                    if (_adjacency[adjacent].Count == 3) queue.Enqueue(adjacent);
                }

                _adjacency.Remove(coordinate);
            }

            var puzzle2 = originalCount - _adjacency.Count;

            yield return $"{puzzle2}";

            await Task.CompletedTask;
        }

        private static IEnumerable<Coordinate> _GetRollCoordinates(string data, int y)
        => data
            .Select((c, x) => (c, x))
            .Where(_ => _.c == '@')
            .Select(_ => new Coordinate(_.x, y));
    }
}
