namespace Moyba.AdventOfCode.Year2024
{
    public class Day23 : IPuzzle
    {
        private readonly string[] _computers;
        private readonly IDictionary<int, HashSet<int>> _connections = new Dictionary<int, HashSet<int>>(260);

        private readonly int _tStartIndex, _tEndIndex;

        public Day23(string[] data)
        {
            var rawConnections = data
                .Where(_ => !String.IsNullOrEmpty(_))
                .Select(_ => _.Split('-'))
                .ToArray();

            _computers = rawConnections
                .SelectMany(_ => _)
                .Distinct()
                .Order()
                .ToArray();

            var indexLookup = _computers.Select((_, i) => (_, i)).ToDictionary(_ => _.Item1, _ => _.Item2);

            foreach (var rawConnection in rawConnections)
            {
                var i1 = indexLookup[rawConnection[0]];
                var i2 = indexLookup[rawConnection[1]];

                if (!_connections.ContainsKey(i1)) _connections.Add(i1, new HashSet<int>(13));
                if (!_connections.ContainsKey(i2)) _connections.Add(i2, new HashSet<int>(13));

                _connections[i1].Add(i2);
                _connections[i2].Add(i1);
            }

            for (_tStartIndex = 0; !_computers[_tStartIndex].StartsWith('t'); _tStartIndex++);
            for (_tEndIndex = _tStartIndex + 1; _computers[_tEndIndex].StartsWith('t'); _tEndIndex++);
        }

        [PartOne("1366")]
        [PartTwo()]
        public async IAsyncEnumerable<string?> ComputeAsync()
        {
            var part1 = 0;
            for (var c1 = _tStartIndex; c1 < _tEndIndex; c1++)
            {
                var connected = _connections[c1].ToArray();
                for (var i2 = 0; i2 < connected.Length; i2++)
                {
                    var c2 = connected[i2];
                    if (c2 >= _tStartIndex && c2 < c1) continue;

                    for (var i3 = i2 + 1; i3 < connected.Length; i3++)
                    {
                        var c3 = connected[i3];
                        if (c3 >= _tStartIndex && c3 < c1) continue;
                        if (!_connections[c2].Contains(c3)) continue;

                        part1++;
                    }
                }
            }

            yield return $"{part1}";

            var maxLength = 3; // assume the max length will be more than 3, since we've already found many 3-sets
            var part2 = String.Empty;
            var included = new int[14];
            this.FindLargestCluster(included, 0, 0, ref maxLength, ref part2);

            yield return $"{part2}";

            await Task.CompletedTask;
        }

        private void FindLargestCluster(int[] included, int includedIndex, int currentIndex, ref int maxLength, ref string password)
        {
            for (var next = currentIndex; next < _computers.Length; next++)
            {
                if (!included.Take(includedIndex).All(_ => _connections[_].Contains(next))) continue;

                included[includedIndex] = next;
                this.FindLargestCluster(included, includedIndex + 1, next + 1, ref maxLength, ref password);
                included[includedIndex] = -1;
            }

            if (includedIndex > maxLength)
            {
                maxLength = includedIndex;
                password = String.Join(',', included.Take(includedIndex).Select(_ => _computers[_]));
            }
        }
    }
}
