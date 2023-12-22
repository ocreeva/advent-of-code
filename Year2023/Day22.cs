namespace Moyba.AdventOfCode.Year2023
{
    using Range = (int start, int end);

    public class Day22(string[] _data) : IPuzzle
    {
        private readonly Brick[] _bricks = _data
            .Select(_ => new Brick(_))
            .ToArray();

        [PartOne("430")]
        [PartTwo("60558")]
        public async IAsyncEnumerable<string?> ComputeAsync()
        {
            var maxX = _bricks.Max(_ => _.XRange.end);
            var maxY = _bricks.Max(_ => _.YRange.end);

            var heights = new SortedList<int, int>[maxX, maxY];
            var brickLookup = new SortedList<int, int>[maxX, maxY];
            for (var x = 0; x < maxX; x++)
                for (var y = 0; y < maxY; y++)
                {
                    heights[x,y] = new SortedList<int, int> { { 0, 1 } };
                    brickLookup[x,y] = new SortedList<int, int> { { Int32.MaxValue, -1 } };
                }

            var minZ = _bricks.Max(_ => _.ZRange.start);
            for (var z = 1; z <= minZ; z++)
            {
                var brickIndexesAtZ = Enumerable.Range(0, _bricks.Length).Where(index => _bricks[index].ZRange.start == z).ToArray();
                foreach (var index in brickIndexesAtZ)
                {
                    var brick = _bricks[index];
                    brick.AddToHeights(heights);
                    if (brick.CanFall(heights, out var fallTo))
                    {
                        brick.RemoveFromHeights(heights);
                        brick.Fall(fallTo);
                        brick.AddToHeights(heights);
                    }

                    foreach (var x in brick.XValues)
                        foreach (var y in brick.YValues)
                            brickLookup[x,y].Add(brick.ZRange.start, index);
                }
            }

            var safeBricks = 0;
            var unsafeBricks = new List<Brick>();
            foreach (var brick in _bricks)
            {
                var bricksAbove = brick.XValues
                    .SelectMany(x => brick.YValues
                        .Select(y => brickLookup[x,y])
                        .Select(_ => _.GetValueAtIndex(_.IndexOfKey(brick.ZRange.start) + 1))
                        .Where(index => index != -1))
                    .Distinct()
                    .Select(index => _bricks[index]);

                brick.RemoveFromHeights(heights);

                if (bricksAbove.All(_ => !_.CanFall(heights, out var _))) safeBricks++;
                else unsafeBricks.Add(brick);

                brick.AddToHeights(heights);
            }

            yield return $"{safeBricks}";

            var fallingBricks = 0;
            foreach (var unsafeBrick in unsafeBricks)
            {
                var removed = new HashSet<Brick>();
                var queue = new Queue<Brick>();
                queue.Enqueue(unsafeBrick);
                while (queue.TryDequeue(out var brick))
                {
                    if (removed.Contains(brick)) continue;

                    removed.Add(brick);
                    brick.RemoveFromHeights(heights);

                    var bricksAbove = brick.XValues
                        .SelectMany(x => brick.YValues
                            .Select(y => brickLookup[x,y])
                            .Select(_ => _.GetValueAtIndex(_.IndexOfKey(brick.ZRange.start) + 1))
                            .Where(index => index != -1))
                        .Distinct()
                        .Select(index => _bricks[index])
                        .Where(_ => !removed.Contains(_));
                    foreach (var brickAbove in bricksAbove) if (brickAbove.CanFall(heights, out var _)) queue.Enqueue(brickAbove);
                }

                fallingBricks += removed.Count - 1;

                foreach (var brickRemoved in removed) brickRemoved.AddToHeights(heights);
            }

            yield return $"{fallingBricks}";

            await Task.CompletedTask;
        }

        private class Brick
        {
            public Brick(string serialized)
            {
                var parts = serialized.Split(',', '~');

                this.XRange = (Int32.Parse(parts[0]), Int32.Parse(parts[3]) + 1);
                if (this.XRange.start >= this.XRange.end) this.XRange = (this.XRange.end - 1, this.XRange.start + 1);

                this.YRange = (Int32.Parse(parts[1]), Int32.Parse(parts[4]) + 1);
                if (this.YRange.start >= this.YRange.end) this.YRange = (this.YRange.end - 1, this.YRange.start + 1);

                this.ZRange = (Int32.Parse(parts[2]), Int32.Parse(parts[5]) + 1);
                if (this.ZRange.start >= this.ZRange.end) this.ZRange = (this.ZRange.end - 1, this.ZRange.start + 1);
            }

            public Range XRange { get; private set; }
            public Range YRange { get; private set; }
            public Range ZRange { get; private set; }

            public IEnumerable<int> XValues => Enumerable.Range(this.XRange.start, this.XRange.end - this.XRange.start);
            public IEnumerable<int> YValues => Enumerable.Range(this.YRange.start, this.YRange.end - this.YRange.start);

            public void AddToHeights(SortedList<int, int>[,] heights)
            {
                foreach (var x in this.XValues)
                    foreach (var y in this.YValues)
                        heights[x,y].Add(this.ZRange.start, this.ZRange.end);
            }

            public void RemoveFromHeights(SortedList<int, int>[,] heights)
            {
                foreach (var x in this.XValues)
                    foreach (var y in this.YValues)
                        heights[x,y].Remove(this.ZRange.start);
            }

            public bool CanFall(SortedList<int, int>[,] heights, out int fallTo)
            {
                fallTo = this.XValues
                    .Max(x => this.YValues
                        .Max(y => heights[x,y].GetValueAtIndex(heights[x,y].IndexOfKey(this.ZRange.start) - 1)));
                return fallTo != this.ZRange.start;
            }

            public void Fall(int fallTo)
            {
                if (fallTo == this.ZRange.start) return;

                var fallBy = this.ZRange.start - fallTo;
                this.ZRange = (fallTo, this.ZRange.end - fallBy);
            }
        }
    }
}
