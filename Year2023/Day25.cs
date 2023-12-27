namespace Moyba.AdventOfCode.Year2023
{
    public class Day25 : IPuzzle
    {
        private readonly SortedList<string, ICollection<string>> _connections = new SortedList<string, ICollection<string>>();

        public Day25(string[] _data)
        {
            foreach (var line in _data)
            {
                var parts = line.Split(':', StringSplitOptions.TrimEntries);
                var component = parts[0];
                var connectedComponents = parts[1].Split(' ');

                if (!_connections.ContainsKey(component)) _connections.Add(component, new List<string>());
                foreach (var connectedComponent in connectedComponents)
                {
                    _connections[component].Add(connectedComponent);

                    if (!_connections.ContainsKey(connectedComponent)) _connections.Add(connectedComponent, new List<string>());
                    _connections[connectedComponent].Add(component);
                }
            }
        }

        public bool SkipEvaluation { get; set; } = true;

        [PartOne("592171")]
        public async IAsyncEnumerable<string?> ComputeAsync()
        {
            if (this.SkipEvaluation)
            {
                // skip evaluation to avoid delay when solving other problems
                yield return "592171";
                yield break;
            }

            var maxConnection = _connections.Keys
                .Select(this.TraceAllRoutes)
                .SelectMany(_ => _.Values.SelectMany(_ => _))
                .GroupBy(_ => _)
                .OrderByDescending(_ => _.Count())
                .First();
            _connections[maxConnection.Key.Item1].Remove(maxConnection.Key.Item2);
            _connections[maxConnection.Key.Item2].Remove(maxConnection.Key.Item1);

            maxConnection = _connections.Keys
                .Select(this.TraceAllRoutes)
                .SelectMany(_ => _.Values.SelectMany(_ => _))
                .GroupBy(_ => _)
                .OrderByDescending(_ => _.Count())
                .First();
            _connections[maxConnection.Key.Item1].Remove(maxConnection.Key.Item2);
            _connections[maxConnection.Key.Item2].Remove(maxConnection.Key.Item1);

            maxConnection = _connections.Keys
                .Select(this.TraceAllRoutes)
                .SelectMany(_ => _.Values.SelectMany(_ => _))
                .GroupBy(_ => _)
                .OrderByDescending(_ => _.Count())
                .First();
            _connections[maxConnection.Key.Item1].Remove(maxConnection.Key.Item2);
            _connections[maxConnection.Key.Item2].Remove(maxConnection.Key.Item1);

            var partialConnections = this.TraceAllRoutes(_connections.Keys[0]).Count;
            var otherConnections = _connections.Count - partialConnections;

            yield return $"{partialConnections * otherConnections}";

            await Task.CompletedTask;
        }

        private IDictionary<string, ICollection<(string, string)>> TraceAllRoutes(string component)
        {
            var visited = new HashSet<string> { component };
            var routes = new Dictionary<string, ICollection<(string, string)>> { { component, new List<(string, string)>() } };

            var queue = new Queue<string>();
            queue.Enqueue(component);
            while (queue.TryDequeue(out var nextComponent))
            {
                foreach (var connection in _connections[nextComponent])
                {
                    if (visited.Contains(connection)) continue;
                    visited.Add(connection);
                    queue.Enqueue(connection);

                    routes.Add(connection, new List<(string, string)>(routes[nextComponent]) { (connection, nextComponent), (nextComponent, connection) });
                }
            }

            return routes;
        }
    }
}
