namespace Moyba.AdventOfCode.Year2023
{
    using Coord = (long x, long y);
    using Range = (long start, long end);
    using Instruction = ((long x, long y) direction, long distance);
    using Plan = (char direction, long distance, string color);

    public class Day18(string[] _data) : IPuzzle
    {
        private static readonly Coord _Up = (0, -1);
        private static readonly Coord _Down = (0, 1);
        private static readonly Coord _Left = (-1, 0);
        private static readonly Coord _Right = (1, 0);
        private static readonly IDictionary<char, Coord> _directions = new Dictionary<char, Coord>
        {
            { 'U', _Up },
            { 'D', _Down },
            { 'L', _Left },
            { 'R', _Right },
        };

        private readonly Plan[] _plan = _data
            .Select(_ => _.Split(' '))
            .Select<string[], Plan>(_ => (_[0][0], Int64.Parse(_[1]), _[2]))
            .ToArray();

        [PartOne("92758")]
        [PartTwo("62762509300678")]
        public async IAsyncEnumerable<string?> ComputeAsync()
        {
            yield return $"{_MeasureArea(_plan.Select(_ExtractInstructionFromPlan))}";

            yield return $"{_MeasureArea(_plan.Select(_ExtractInstructionFromColor))}";

            await Task.CompletedTask;
        }

        private static long _MeasureArea(IEnumerable<Instruction> instructions)
        {
            var horizontals = new SortedList<long, SortedList<long, Range>>();

            Coord position = (0L, 0L);
            foreach ((var direction, var distance) in instructions)
            {
                var next = _ApplyDirection(position, direction, distance);
                if (position.y == next.y)
                {
                    horizontals.TryAdd(position.y, new SortedList<long, Range>());

                    Range range = (Math.Min(position.x, next.x), Math.Max(position.x, next.x));
                    horizontals[position.y].Add(range.start, range);
                }

                position = next;
            }

            var current = new SortedList<long, long>();
            var area = 0L;
            var previousY = horizontals.GetKeyAtIndex(0);
            for (var index = 0; index < horizontals.Count; index++)
            {
                var y = horizontals.GetKeyAtIndex(index);
                area += (y - previousY) * current.Sum(_ => _.Value - _.Key + 1);
                // _ResolveHorizontals(current, horizontals.GetValueAtIndex(index), ref area);
                foreach (var horizontal in horizontals.GetValueAtIndex(index).Values) area += _ApplyRange(current, horizontal.start, horizontal.end);
                previousY = y;
            }

            return area;
        }

        private static long _ApplyRange(SortedList<long, long> ranges, long start, long end)
        {
            var areaRemoved = 0L;

            if (ranges.ContainsKey(start))
            {
                var newStart = Math.Min(end, ranges[start]);
                var newEnd = Math.Max(end, ranges[start]);
                ranges.Remove(start);

                areaRemoved += newStart - start;
                if (newStart == newEnd) return areaRemoved + 1;

                start = newStart;
                end = newEnd;
            }

            ranges.Add(start, end);

            // can we collapse with the previous entry?
            var index = ranges.IndexOfKey(start);
            if (index > 0)
            {
                var previousStart = ranges.GetKeyAtIndex(index - 1);
                var previousEnd = ranges.GetValueAtIndex(index - 1);
                if (previousEnd == end)
                {
                    ranges[previousStart] = start;
                    ranges.Remove(start);
                    return areaRemoved + end - start;
                }

                if (previousEnd > end)
                {
                    ranges[previousStart] = start;
                    ranges[end] = previousEnd;
                    ranges.Remove(start);
                    return areaRemoved + end - start - 1;
                }

                if (previousEnd == start)
                {
                    ranges.Remove(start);
                    start = ranges.GetKeyAtIndex(index - 1);
                    ranges[start] = end;
                }
            }

            // do we overlap with the subsequent entries?
            index = ranges.IndexOfKey(start);
            while (index < ranges.Count - 1)
            {
                var nextStart = ranges.GetKeyAtIndex(index + 1);
                if (nextStart > end) break;
                if (nextStart == end)
                {
                    end = ranges[nextStart];
                    ranges[start] = end;
                    ranges.Remove(nextStart);
                    break;
                }

                areaRemoved += ranges[nextStart] - nextStart - 1;
                ranges[start] = nextStart;
                start = ranges[nextStart];
                ranges.Remove(nextStart);

                if (start == end) return areaRemoved + 1;

                ranges.Add(start, end);
                index = ranges.IndexOfKey(start);
            }

            return areaRemoved;
        }

        private static void _ResolveHorizontals(SortedList<long, Range> ranges, SortedList<long, Range> horizontals, ref long area)
        {
            var rangeIndex = 0;
            for (var horizontalIndex = 0; horizontalIndex < horizontals.Count; horizontalIndex++)
            {
                var horizontal = horizontals.GetValueAtIndex(horizontalIndex);

                rangeIndex--;
                Range? range;
                do
                {
                    range = ++rangeIndex < ranges.Count ? ranges.GetValueAtIndex(rangeIndex) : null;
                }
                while (range != null && horizontal.start > range.Value.end);

                if (range == null || horizontal.end < range.Value.start)
                {
                    ranges.Add(horizontal.start, horizontal);
                    continue;
                }

                // overlap or adjacency in all following cases
                ranges.RemoveAt(rangeIndex);

                if (horizontal.end == range.Value.start)
                {
                    ranges.Add(horizontal.start, (horizontal.start, range.Value.end));
                    continue;
                }

                if (horizontal.start == range.Value.end)
                {
                    ranges.Add(range.Value.start, (range.Value.start, horizontal.end));
                    continue;
                }

                if (horizontal.start < range.Value.start) throw new Exception("Unexpected case #1.");

                if (horizontal.start == range.Value.start)
                {
                    if (horizontal.end == range.Value.end)
                    {
                        area += horizontal.end - horizontal.start + 1;
                        continue;
                    }

                    Range newRange = (Math.Min(horizontal.end, range.Value.end), Math.Max(horizontal.end, range.Value.end));
                    ranges.Add(newRange.start, newRange);
                    area += newRange.start - horizontal.start;
                    continue;
                }

                if (horizontal.end == range.Value.end)
                {
                    Range newRange = (Math.Min(horizontal.start, range.Value.start), Math.Max(horizontal.start, range.Value.start));
                    ranges.Add(newRange.start, newRange);
                    area += horizontal.end - newRange.end;
                    continue;
                }

                if (horizontal.end > range.Value.end) throw new Exception($"Unexpected case #2: ({horizontal.start}, {horizontal.end}) ({range.Value.start}, {range.Value.end})");

                // one is inside the other, split into two
                Range newRangeStart = (Math.Min(horizontal.start, range.Value.start), Math.Max(horizontal.start, range.Value.start));
                ranges.Add(newRangeStart.start, newRangeStart);

                Range newRangeEnd = (Math.Min(horizontal.end, range.Value.end), Math.Max(horizontal.end, range.Value.end));
                ranges.Add(newRangeEnd.start, newRangeEnd);

                area += newRangeEnd.start - newRangeStart.end - 1;
            }
        }

        private static Instruction _ExtractInstructionFromPlan(Plan plan) => plan.direction switch
        {
            'U' => (_Up, plan.distance),
            'D' => (_Down, plan.distance),
            'L' => (_Left, plan.distance),
            'R' => (_Right, plan.distance),
            _ => throw new Exception($"Unexpected direction: {plan.direction}")
        };

        private static Instruction _ExtractInstructionFromColor(Plan plan) => plan.color[7] switch
        {
            '0' => (_Right, Int64.Parse(plan.color[2..7], System.Globalization.NumberStyles.HexNumber)),
            '1' => (_Down, Int64.Parse(plan.color[2..7], System.Globalization.NumberStyles.HexNumber)),
            '2' => (_Left, Int64.Parse(plan.color[2..7], System.Globalization.NumberStyles.HexNumber)),
            '3' => (_Up, Int64.Parse(plan.color[2..7], System.Globalization.NumberStyles.HexNumber)),
            _ => throw new Exception($"Unexpected color: {plan.color}")
        };

        private static Coord _ApplyDirection(Coord position, Coord direction, long distance) => (position.x + distance * direction.x, position.y + distance * direction.y);
    }
}
