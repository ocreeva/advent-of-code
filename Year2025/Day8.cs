using Moyba.AdventOfCode.Utility;

namespace Moyba.AdventOfCode.Year2025
{
    public class Day8(string[] _data) : IPuzzle
    {
        private readonly Coordinate[] _boxes = _data.Select(_ => _.Split(',')).Select(_ => new Coordinate(_[0], _[1], _[2])).ToArray();

        [PartOne("97384")]
        [PartTwo("9003685096")]
        public async IAsyncEnumerable<string?> ComputeAsync()
        {
            var circuits = _boxes.ToDictionary(_ => _, _ => new HashSet<Coordinate> { _ });

            var distances = new SortedDictionary<long, (Coordinate, Coordinate)>();
            for (var aIndex = 0; aIndex < _boxes.Length - 1; aIndex++)
            {
                var a = _boxes[aIndex];
                for (var bIndex = aIndex + 1; bIndex < _boxes.Length; bIndex++)
                {
                    var b = _boxes[bIndex];
                    distances.Add((a - b).GetSquareMagnitude(), (a, b));
                }
            }

            for (var iteration = 0; iteration < 1000; iteration++)
            {
                var closestDistance = distances.Keys.First();
                var closestBoxes = distances[closestDistance];
                distances.Remove(closestDistance);

                var circuit1 = circuits[closestBoxes.Item1];
                if (circuit1.Contains(closestBoxes.Item2)) continue;

                var circuit2 = circuits[closestBoxes.Item2];

                circuit1.UnionWith(circuit2);
                foreach (var box in circuit2) circuits[box] = circuit1;
            }

            var puzzle1 = circuits.Values.ToHashSet().Select(_ => _.Count).OrderDescending().Take(3).Aggregate(1L, (product, value) => product * value);

            yield return $"{puzzle1}";

            while (true)
            {
                var closestDistance = distances.Keys.First();
                var closestBoxes = distances[closestDistance];
                distances.Remove(closestDistance);

                var circuit1 = circuits[closestBoxes.Item1];
                if (circuit1.Contains(closestBoxes.Item2)) continue;

                var circuit2 = circuits[closestBoxes.Item2];

                circuit1.UnionWith(circuit2);
                if (circuit1.Count == _boxes.Length)
                {
                    yield return $"{closestBoxes.Item1.x * closestBoxes.Item2.x}";
                    break;
                }

                foreach (var box in circuit2) circuits[box] = circuit1;
            }

            await Task.CompletedTask;
        }
    }
}
