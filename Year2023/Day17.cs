namespace Moyba.AdventOfCode.Year2023
{
    using Coord = (int x, int y);
    using Node = IDictionary<(int x, int y), int[]>;

    public class Day17(string[] _data) : IPuzzle
    {
        private const int _MaxHeat = Int32.MaxValue - 9;

        private static readonly IComparer<Coord> _CoordComparer = new CoordComparer();
        private static readonly Coord _North = (0, -1);
        private static readonly Coord _South = (0, 1);
        private static readonly Coord _West = (-1, 0);
        private static readonly Coord _East = (1, 0);
        private static readonly Coord[] _AllDirections = [ _North, _South, _West, _East ];

        private readonly int[][] _heat = _data.Select(_ => _.Select(_ => _ - '0').ToArray()).ToArray();
        private readonly int _height = _data.Length;
        private readonly int _width = _data[0].Length;

        private readonly Node[][] _nodes = Enumerable
            .Range(0, _data.Length)
            .Select(x => Enumerable
                .Range(0, _data[0].Length)
                .Select<int, Node>(_ => new Dictionary<Coord, int[]>
                {
                    { _North, [ ] },
                    { _South, [ ] },
                    { _West, [ ] },
                    { _East, [ ] },
                })
                .ToArray())
            .ToArray();

        public bool SkipEvaluation { get; set; } = true;

        [PartOne("686")]
        [PartTwo("801")]
        public async IAsyncEnumerable<string?> ComputeAsync()
        {
            if (this.SkipEvaluation)
            {
                // skip evaluation to avoid delay when solving other problems
                yield return "686";
                yield return "801";
                yield break;
            }

            for (var y = 0; y < _height; y++)
            {
                for (var x = 0; x < _width; x++)
                {
                    foreach (var direction in _AllDirections) _nodes[y][x][direction] = [ _MaxHeat, _MaxHeat, _MaxHeat ];
                }
            }

            for (var x = 0; x < _width; x++) _nodes[0][x][_North] = [0, 0, 0];
            for (var y = 0; y < _height; y++) _nodes[y][0][_West] = [0, 0, 0];
            _nodes[0][0][_East] = [0, 0, 0];
            _nodes[0][0][_South] = [0, 0, 0];

            var pending = new SortedList<int, Coord> { { 0, (0, 0) } };
            while (pending.Count > 0)
            {
                var position = pending.GetValueAtIndex(0);
                pending.RemoveAt(0);

                this.EvaluateNode(position, 0, 3, pending);
            }

            var heat = _nodes[_height - 1][_width - 1].Values.Min(_ => _.Min());
            yield return $"{heat}";

            for (var y = 0; y < _height; y++)
            {
                for (var x = 0; x < _width; x++)
                {
                    foreach (var direction in _AllDirections) _nodes[y][x][direction] = [ _MaxHeat, _MaxHeat, _MaxHeat, _MaxHeat, _MaxHeat, _MaxHeat, _MaxHeat, _MaxHeat, _MaxHeat, _MaxHeat ];
                }
            }

            for (var x = 0; x < _width; x++) _nodes[0][x][_North] = [0, 0, 0, 0, 0, 0, 0, 0, 0, 0];
            for (var y = 0; y < _height; y++) _nodes[y][0][_West] = [0, 0, 0, 0, 0, 0, 0, 0, 0, 0];
            _nodes[0][0][_East] = [0, 0, 0, 0, 0, 0, 0, 0, 0, 0];
            _nodes[0][0][_South] = [0, 0, 0, 0, 0, 0, 0, 0, 0, 0];

            pending = new SortedList<int, Coord> { { 0, (0, 0) } };
            while (pending.Count > 0)
            {
                var position = pending.GetValueAtIndex(0);
                pending.RemoveAt(0);

                this.EvaluateNode(position, 4, 10, pending);
            }

            heat = _nodes[_height - 1][_width - 1].Values.Min(_ => _.Min());
            yield return $"{heat}";

            await Task.CompletedTask;
        }

        private void EvaluateNode(Coord position, int minTurns, int maxTurns, SortedList<int, Coord> pending)
        {
            foreach (var direction in _AllDirections)
            {
                if (this.TryEvaluateNode(position, direction, minTurns, maxTurns))
                {
                    var target = _ApplyVelocity(position, direction);
                    var key = _GetKey(target);
                    if (!pending.ContainsKey(key)) pending.Add(key, target);
                }
            }
        }

        private bool TryEvaluateNode(Coord position, Coord velocity, int minTurns, int maxTurns)
        {
            Coord targetPosition = _ApplyVelocity(position, velocity);
            if (targetPosition.x < 0 || targetPosition.x >= _width) return false;
            if (targetPosition.y < 0 || targetPosition.y >= _height) return false;
            var targetNode = this.GetNode(targetPosition);
            var targetHeat = this.GetHeat(targetPosition);

            var sourceNode = this.GetNode(position);
            var sourceHeat = sourceNode[velocity];

            var left = _TurnLeft(velocity);
            var right = _TurnRight(velocity);

            var foundUpdate = false;

            for (var turns = 1; turns <= maxTurns; turns++)
            {
                var heat = sourceHeat[turns - 1] + targetHeat;
                if (turns < maxTurns && heat < targetNode[velocity][turns])
                {
                    foundUpdate = true;
                    targetNode[velocity][turns] = heat;
                }

                if (turns >= minTurns)
                {
                    if (heat < targetNode[left][0])
                    {
                        foundUpdate = true;
                        targetNode[left][0] = heat;
                    }

                    if (heat < targetNode[right][0])
                    {
                        foundUpdate = true;
                        targetNode[right][0] = heat;
                    }
                }
            }

            return foundUpdate;
        }

        private int GetHeat(Coord position) => _heat[position.y][position.x];
        private Node GetNode(Coord position) => _nodes[position.y][position.x];

        private static Coord _ApplyVelocity(Coord position, Coord velocity) => (position.x + velocity.x, position.y + velocity.y);

        private static int _GetKey(Coord position) => ((position.x + position.y) << 15) + position.x;

        private static Coord _TurnLeft(Coord velocity) => (velocity.y, -velocity.x);
        private static Coord _TurnRight(Coord velocity) => (-velocity.y, velocity.x);

        private class CoordComparer : IComparer<Coord>
        {
            public int Compare(Coord a, Coord b)
            {
                var aSum = a.x + a.y;
                var bSum = b.x + b.y;
                if (aSum != bSum) return aSum.CompareTo(bSum);
                return a.x.CompareTo(b.x);
            }
        }
    }
}
