using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace Moyba.AdventOfCode
{
    internal class Puzzles2022 : PuzzlesBase
    {
        protected override int Year => 2022;

        public override async Task SolveAsync()
        {
            await this.SolveAsync(() => Puzzles2022.Day1, LineDelimited, AsBatchesOfLongs);
            await this.SolveAsync(() => Puzzles2022.Day2);
            await this.SolveAsync(() => Puzzles2022.Day3);
            await this.SolveAsync(() => Puzzles2022.Day4, LineDelimited, new Regex(@"(?<Min1>\d+)\-(?<Max1>\d+),(?<Min2>\d+)-(?<Max2>\d+)"));
            await this.SolveAsync(() => Puzzles2022.Day5);
            await this.SolveAsync(() => Puzzles2022.Day6, LineDelimited, AsString);
            await this.SolveAsync(() => Puzzles2022.Day7);
            await this.SolveAsync(() => Puzzles2022.Day8);
            await this.SolveAsync(() => Puzzles2022.Day9);
            await this.SolveAsync(() => Puzzles2022.Day10);
            await this.SolveAsync(() => Puzzles2022.Day11, LineDelimited, AsBatchesOfStrings);
            await this.SolveAsync(() => Puzzles2022.Day12);
            await this.SolveAsync(() => Puzzles2022.Day13, LineDelimited, AsBatchesOfStrings);
            await this.SolveAsync(() => Puzzles2022.Day14);
            //await this.SolveAsync(() => Puzzles2022.Day15, LineDelimited, new Regex(@"Sensor at x=(?<sx>-?\d+), y=(?<sy>-?\d+): closest beacon is at x=(?<bx>-?\d+), y=(?<by>-?\d+)"));
            await this.SolveAsync(() => Puzzles2022.Day16, LineDelimited, new Regex(@"Valve (?<Name>[A-Z]+) has flow rate=(?<Flow>\d+); tunnels? leads? to valves? (?<Exits>.*)"));
            await this.SolveAsync(() => Puzzles2022.Day17, LineDelimited, AsString);
            await this.SolveAsync(() => Puzzles2022.Day18);
            await this.SolveAsync(() => Puzzles2022.Day19, LineDelimited, new Regex(@"Blueprint (?<ID>\d+): Each ore robot costs (?<c00>\d+) ore. Each clay robot costs (?<c10>\d+) ore. Each obsidian robot costs (?<c20>\d+) ore and (?<c21>\d+) clay. Each geode robot costs (?<c30>\d+) ore and (?<c32>\d+) obsidian."));
            await this.SolveAsync(() => Puzzles2022.Day20, LineDelimited, AsLongs);
        }

        private class CoordinateEncryptionNode
        {
            public long Value { get; set; }
            public CoordinateEncryptionNode Previous { get; set; }
            public CoordinateEncryptionNode Next { get; set; }

            public void Move(long steps)
            {
                this.Previous.Next = this.Next;
                this.Next.Previous = this.Previous;

                for ( ; steps > 0; steps--) this.Next = this.Next.Next;
                this.Previous = this.Next.Previous;

                for ( ; steps < 0; steps++) this.Previous = this.Previous.Previous;
                this.Next = this.Previous.Next;

                this.Next.Previous = this;
                this.Previous.Next = this;
            }
        }

        [Answer("2203", "6641234038999")]
        private static (string, string) Day20(IEnumerable<long> input)
        {
            var values = input.ToArray();
            var length = values.Length;
            var lengthLess1 = length - 1;
            var nodes1 = CreateCoordinateEncryptionList(values);
            var nodes2 = CreateCoordinateEncryptionList(values, 811589153);

            for (var index = 0; index < length; index++)
            {
                var node = nodes1[index];
                node.Move(node.Value % lengthLess1);
            }

            for (var iteration = 0; iteration < 10; iteration++)
            {
                for (var index = 0; index < length; index++)
                {
                    var node = nodes2[index];
                    node.Move(node.Value % lengthLess1);
                }
            }

            long puzzle1 = 0L, puzzle2 = 0L;
            CoordinateEncryptionNode node1 = nodes1.Single(n => n.Value == 0), node2 = nodes2.Single(n => n.Value == 0);
            for (var iteration = 0; iteration < 3; iteration++)
            {
                for (var index = 0; index < 1000; index++)
                {
                    node1 = node1.Next;
                    node2 = node2.Next;
                }

                puzzle1 += node1.Value;
                puzzle2 += node2.Value;
            }

            return ($"{puzzle1}", $"{puzzle2}");
        }

        private static List<CoordinateEncryptionNode> CreateCoordinateEncryptionList(IEnumerable<long> input, long multiplier = 1)
        {
            var nodes = input.Select(value => new CoordinateEncryptionNode { Value = value * multiplier }).ToList();

            for (var index = 0; index < nodes.Count; index++)
            {
                nodes[index].Next = nodes[(index + 1) % nodes.Count];
                nodes[index].Previous = nodes[index == 0 ? nodes.Count - 1 : index - 1];
            }

            return nodes;
        }

        private class RobotBlueprint
        {
            private readonly int[,] _costs = new int[4, 3];
            public int ID { get; set; }
            public int[,] Costs => _costs;
        }

        [Answer("1115", "25056")]
        private static (string, string) Day19(IEnumerable<Match> input)
        {
            long puzzle1 = 0L, puzzle2 = 1L;
            var blueprints = new List<RobotBlueprint>();

            foreach (var match in input)
            {
                var blueprint = new RobotBlueprint { ID = Int32.Parse(match.Groups["ID"].Value) };

                blueprint.Costs[0, 0] = Int32.Parse(match.Groups["c00"].Value);
                blueprint.Costs[1, 0] = Int32.Parse(match.Groups["c10"].Value);
                blueprint.Costs[2, 0] = Int32.Parse(match.Groups["c20"].Value);
                blueprint.Costs[2, 1] = Int32.Parse(match.Groups["c21"].Value);
                blueprint.Costs[3, 0] = Int32.Parse(match.Groups["c30"].Value);
                blueprint.Costs[3, 2] = Int32.Parse(match.Groups["c32"].Value);

                blueprints.Add(blueprint);
            }

            for (var blueprintIndex = 0; blueprintIndex < blueprints.Count; blueprintIndex++)
            {
                var blueprint = blueprints[blueprintIndex];

                var maxOreRobots = Int32.Max(Int32.Max(blueprint.Costs[0,0], blueprint.Costs[1,0]), Int32.Max(blueprint.Costs[2,0], blueprint.Costs[3,0]));
                var maxClaRobots = blueprint.Costs[2,1];
                var maxObsRobots = blueprint.Costs[3,2];

                var states = new HashSet<(int orR, int clR, int obR, int geR, int ore, int cla, int obs, int geo)> { (1, 0, 0, 0, 0, 0, 0, 0) };
                for (var turn = 0; turn < 24 || (turn < 32 && blueprintIndex <= 2); turn++)
                {
                    var nextStates = new HashSet<(int oreRobots, int claRobots, int obsRobots, int geoRobots, int ore, int cla, int obs, int geo)>();
                    foreach (var state in states)
                    {
                        // greedy, always build a geode robot if we can
                        if (state.ore >= blueprint.Costs[3, 0] && state.obs >= blueprint.Costs[3,2])
                        {
                            AddAndTrimRobotStates(nextStates, (state.orR, state.clR, state.obR, state.geR + 1, state.ore + state.orR - blueprint.Costs[3,0], state.cla + state.clR, state.obs + state.obR - blueprint.Costs[3,2], state.geo + state.geR));
                        }
                        else
                        {
                            // build no robot
                            AddAndTrimRobotStates(nextStates, (state.orR, state.clR, state.obR, state.geR, state.ore + state.orR, state.cla + state.clR, state.obs + state.obR, state.geo + state.geR));

                            // build an ore robot
                            if (state.ore >= blueprint.Costs[0, 0] && state.orR < maxOreRobots)
                            {
                                AddAndTrimRobotStates(nextStates, (state.orR + 1, state.clR, state.obR, state.geR, state.ore + state.orR - blueprint.Costs[0,0], state.cla + state.clR, state.obs + state.obR, state.geo + state.geR));
                            }

                            // build a clay robot
                            if (state.ore >= blueprint.Costs[1, 0] && state.clR < maxClaRobots)
                            {
                                AddAndTrimRobotStates(nextStates, (state.orR, state.clR + 1, state.obR, state.geR, state.ore + state.orR - blueprint.Costs[1,0], state.cla + state.clR, state.obs + state.obR, state.geo + state.geR));
                            }

                            // build an obsidian robot
                            if (state.ore >= blueprint.Costs[2, 0] && state.cla >= blueprint.Costs[2, 1] && state.obR < maxObsRobots)
                            {
                                AddAndTrimRobotStates(nextStates, (state.orR, state.clR, state.obR + 1, state.geR, state.ore + state.orR - blueprint.Costs[2,0], state.cla + state.clR - blueprint.Costs[2,1], state.obs + state.obR, state.geo + state.geR));
                            }
                        }
                    }

                    states = nextStates;

                    if (turn == 23) puzzle1 += blueprint.ID * states.Max(s => s.geo);
                    if (turn == 31) puzzle2 *= states.Max(s => s.geo);
                    else if (turn > 20)
                    {
                        var remainingTurns = blueprintIndex <= 2 ? 30 - turn : 22 - turn;
                        var maxNaturalGeo = states.Max(s => s.geo + remainingTurns * s.geR);
                        var buyEveryTurn = remainingTurns * remainingTurns;
                        var neededNaturalGeo = maxNaturalGeo - buyEveryTurn;
                        var cantCatchUp = states.Where(s => s.geo + remainingTurns * s.geR < neededNaturalGeo).ToArray();
                        foreach (var state in cantCatchUp)
                        {
                            states.Remove(state);
                        }
                    }
                }
            }

            return ($"{puzzle1}", $"{puzzle2}");
        }

        private static void AddAndTrimRobotStates(
            HashSet<(int orR, int clR, int obR, int geR, int ore, int cla, int obs, int geo)> states,
            (int orR, int clR, int obR, int geR, int ore, int cla, int obs, int geo) state)
        {
            if (states.Any(s => s.orR >= state.orR && s.clR >= state.clR && s.obR >= state.obR && s.geR >= state.geR && s.ore >= state.ore && s.cla >= state.cla && s.obs >= state.obs && s.geo >= state.geo)) return;

            var trimStates = states.Where(s => s.orR <= state.orR && s.clR <= state.clR && s.obR <= state.obR && s.geR <= state.geR && s.ore <= state.ore && s.cla <= state.cla && s.obs <= state.obs && s.geo <= state.geo).ToArray();
            foreach (var trimState in trimStates) states.Remove(trimState);

            states.Add(state);
        }

        [Answer("3550", "2028")]
        private static (string, string) Day18(IEnumerable<string> input)
        {
            var cubes = new HashSet<(int x, int y, int z)>();
            foreach (var line in input)
            {
                var parts = line.Split(',');
                cubes.Add((Int32.Parse(parts[0]), Int32.Parse(parts[1]), Int32.Parse(parts[2])));
            }

            var puzzle1 = 0L;
            foreach (var cube in cubes)
            {
                if (!cubes.Contains((cube.x, cube.y, cube.z + 1))) puzzle1++;
                if (!cubes.Contains((cube.x, cube.y, cube.z - 1))) puzzle1++;
                if (!cubes.Contains((cube.x, cube.y + 1, cube.z))) puzzle1++;
                if (!cubes.Contains((cube.x, cube.y - 1, cube.z))) puzzle1++;
                if (!cubes.Contains((cube.x + 1, cube.y, cube.z))) puzzle1++;
                if (!cubes.Contains((cube.x - 1, cube.y, cube.z))) puzzle1++;
            }

            var minX = cubes.Min(c => c.x) - 1;
            var maxX = cubes.Max(c => c.x) + 1;
            var minY = cubes.Min(c => c.y) - 1;
            var maxY = cubes.Max(c => c.y) + 1;
            var minZ = cubes.Min(c => c.z) - 1;
            var maxZ = cubes.Max(c => c.z) + 1;

            var queue = new Queue<(int x, int y, int z)>();
            queue.Enqueue((minX, minY, minZ));

            var empty = new HashSet<(int x, int y, int z)> { (minX, minY, minZ) };
            Action<(int x, int y, int z), (int x, int y, int z)> transform = ((int x, int y, int z) coordinate, (int x, int y, int z) offset) =>
            {
                (int x, int y, int z) test = (coordinate.x + offset.x, coordinate.y + offset.y, coordinate.z + offset.z);
                if (test.x < minX || test.x > maxX || test.y < minY || test.y > maxY || test.z < minZ || test.z > maxZ) return;
                if (cubes.Contains(test) || empty.Contains(test)) return;
                queue.Enqueue(test);
                empty.Add(test);
            };

            while (queue.Count > 0)
            {
                var coordinate = queue.Dequeue();
                transform(coordinate, (0, 0, -1));
                transform(coordinate, (0, 0, 1));
                transform(coordinate, (0, -1, 0));
                transform(coordinate, (0, 1, 0));
                transform(coordinate, (-1, 0, 0));
                transform(coordinate, (1, 0, 0));
            }

            var puzzle2 = 0L;
            foreach (var cube in cubes)
            {
                if (empty.Contains((cube.x, cube.y, cube.z + 1))) puzzle2++;
                if (empty.Contains((cube.x, cube.y, cube.z - 1))) puzzle2++;
                if (empty.Contains((cube.x, cube.y + 1, cube.z))) puzzle2++;
                if (empty.Contains((cube.x, cube.y - 1, cube.z))) puzzle2++;
                if (empty.Contains((cube.x + 1, cube.y, cube.z))) puzzle2++;
                if (empty.Contains((cube.x - 1, cube.y, cube.z))) puzzle2++;
            }

            return ($"{puzzle1}", $"{puzzle2}");
        }

        [Answer("3202", "1591977077352")]
        private static (string, string) Day17(string input)
        {
            var puzzle1 = 0L;

            var state = new Dictionary<(long, int), (long height, long iteration)>();

            (int x, int y)[] current;
            var height = 0L;
            var maxRockHeight = 0;
            var rocks = new HashSet<(int x, int y)> { (0, 0), (1, 0), (2, 0), (3, 0), (4, 0), (5, 0), (6, 0) };

            var inputIndex = 0;
            for (var iteration = 0L; iteration < 1_000_000_000_000L; iteration++)
            {
                var nextIndex = iteration % 5;
                switch (nextIndex)
                {
                    case 0:
                        current = new[] { (2, maxRockHeight + 4), (5, maxRockHeight + 4), (3, maxRockHeight + 4), (4, maxRockHeight + 4) };
                        break;

                    case 1:
                        current = new[] { (2, maxRockHeight + 5), (4, maxRockHeight + 5), (3, maxRockHeight + 4), (3, maxRockHeight + 6) };
                        break;

                    case 2:
                        current = new[] { (2, maxRockHeight + 4), (4, maxRockHeight + 4), (3, maxRockHeight + 4), (4, maxRockHeight + 5), (4, maxRockHeight + 6) };
                        break;

                    case 3:
                        current = new[] { (2, maxRockHeight + 5), (2, maxRockHeight + 6), (2, maxRockHeight + 4), (2, maxRockHeight + 7) };
                        break;

                    default:
                        current = new[] { (2, maxRockHeight + 4), (3, maxRockHeight + 5), (3, maxRockHeight + 4), (2, maxRockHeight + 5) };
                        break;
                }

                bool couldMoveDown;
                do
                {
                    var action = input[inputIndex++ % input.Length];
                    switch (action)
                    {
                        case '<':
                            TryMoveRock(rocks, ((int x, int y) r) => (r.x - 1, r.y), ref current);
                            break;

                        case '>':
                            TryMoveRock(rocks, ((int x, int y) r) => (r.x + 1, r.y), ref current);
                            break;

                        default:
                            throw new Exception($"Unexpected character in input: {action}");
                    }

                    couldMoveDown = TryMoveRock(rocks, ((int x, int y) r) => (r.x, r.y - 1), ref current);
                }
                while (couldMoveDown);

                foreach (var position in current) rocks.Add(position);

                var solidLine = rocks.GroupBy(r => r.y).Where(g => g.Count() == 7).Select(g => g.First().y).Max();
                if (solidLine > 0)
                {
                    height += solidLine;
                    rocks = rocks
                        .Where(r => r.y >= solidLine)
                        .Select(r => (r.x, r.y - solidLine))
                        .ToHashSet<(int x, int y)>();
                }

                maxRockHeight = rocks.Max(r => r.y);

                if (iteration == 2021) puzzle1 = height + maxRockHeight;
                else if (iteration > 2021)
                {
                    var key = (iteration % 5, inputIndex % input.Length);
                    if (state.ContainsKey(key))
                    {
                        var history = state[key];
                        var distance = iteration - history.iteration;
                        var repetition = (1_000_000_000_000L - iteration) / distance;

                        iteration += repetition * distance;
                        height += (height - history.height) * repetition;
                    }
                    else
                    {
                        state[key] = (height, iteration);
                    }
                }
            }

            return ($"{puzzle1}", $"{height + maxRockHeight}");
        }

        private static bool TryMoveRock(HashSet<(int x, int y)> rocks, Func<(int x, int y), (int x, int y)> transform, ref (int x, int y)[] current)
        {
            var next = current.Select(r => transform(r)).ToArray();
            if (next[0].x == -1) return false;
            if (next[1].x == 7) return false;
            if (next[2].y == 0) return false;
            if (next.Any(r => rocks.Contains(r))) return false;

            current = next;
            return true;
        }

        private class Valve
        {
            private readonly Dictionary<long, long> _destinations = new Dictionary<long, long>();
            public long Id { get; set; }
            public int Index { get; set; }
            public long Flow { get; set; }
            public string[] Exits { get; set; }
            public Dictionary<long, long> Destinations => _destinations;
        }

        [Answer("2077", "2741")]
        private static (string, string) Day16(IEnumerable<Match> input)
        {
            int index = 0, id = 0;
            var allValves = new Dictionary<string, Valve>();
            foreach (var match in input)
            {
                if (!match.Success) throw new Exception("Regex failure!");

                var valve = new Valve { Index = index++, Flow = Int64.Parse(match.Groups["Flow"].Value), Exits = match.Groups["Exits"].Value.Split(", ") };
                if (valve.Flow > 0) valve.Id = 1 << id++;

                var name = match.Groups["Name"].Value;
                allValves.Add(name, valve);
            }

            var distance = new int[allValves.Count, allValves.Count];
            for (var x = 0; x < allValves.Count; x++)
            {
                for (var y = 0; y < allValves.Count; y++) distance[x, y] = allValves.Count;
            }

            foreach (var valve in allValves.Values)
            {
                distance[valve.Index, valve.Index] = 0;
                foreach (var exit in valve.Exits.Select(e => allValves[e]))
                {
                    distance[valve.Index, exit.Index] = 1;
                    distance[exit.Index, valve.Index] = 1;
                }
            }

            var numberOfValves = allValves.Count;
            for (var x = 0; x < numberOfValves; x++)
            {
                for (var y = 0; y < numberOfValves; y++)
                {
                    if (x == y) continue;
                    for (var z = 0; z < numberOfValves; z++)
                    {
                        distance[y, z] = Math.Min(distance[y, z], distance[y, x] + distance[x, z]);
                    }
                }
            }

            var start = allValves["AA"];
            var interestingValves = allValves.Values.Where(v => v.Id > 0).ToArray();
            var longestPath = interestingValves.Select(v => distance[start.Index, v.Index]).Max();
            var earliestRound = 29 - interestingValves.Select(v => distance[start.Index, v.Index]).Min();
            var flowsPerRound = new Dictionary<long, Dictionary<long, long>>[earliestRound + 1];
            flowsPerRound[0] = new Dictionary<long, Dictionary<long, long>>();
            for (var round = 1; round <= earliestRound; round++)
            {
                var flows = new Dictionary<long, Dictionary<long, long>>();
                flowsPerRound[round] = flows;

                foreach (var valve in interestingValves)
                {
                    var totalValveFlow = valve.Flow * round;
                    var currentFlows = new Dictionary<long, long>();
                    if (round < longestPath) currentFlows.Add(valve.Id, totalValveFlow);
                    flows[valve.Id] = currentFlows;
                    foreach (var next in interestingValves)
                    {
                        if (next.Id == valve.Id) continue;
                        var nextDistance = distance[valve.Index, next.Index] + 1;
                        if (nextDistance >= round) continue;

                        var nextFlows = flowsPerRound[round - nextDistance][next.Id];
                        foreach (var nextFlow in nextFlows.Where(p => (p.Key & valve.Id) == 0))
                        {
                            var key = nextFlow.Key | valve.Id;
                            var value = nextFlow.Value + totalValveFlow;
                            if (!currentFlows.ContainsKey(key)) currentFlows.Add(key, value);
                            else currentFlows[key] = Math.Max(currentFlows[key], value);
                        }
                    }
                }
            }

            var puzzle1 = 0L;
            foreach (var first in interestingValves)
            {
                var round = 29 - distance[start.Index, first.Index];
                puzzle1 = Math.Max(puzzle1, flowsPerRound[round][first.Id].Max(p => p.Value));
            }

            var puzzle2 = 0L;
            for (var firstIndex = 0; firstIndex < interestingValves.Length - 1; firstIndex++)
            {
                var first = interestingValves[firstIndex];
                var firstRound = 25 - distance[start.Index, first.Index];
                var firstFlows = flowsPerRound[firstRound][first.Id];

                for (var secondIndex = firstIndex + 1; secondIndex < interestingValves.Length; secondIndex++)
                {
                    var second = interestingValves[secondIndex];
                    var secondRound = 25 - distance[start.Index, second.Index];
                    var secondFlows = flowsPerRound[secondRound][second.Id];
                    foreach (var firstFlow in firstFlows)
                    {
                        foreach (var secondFlow in secondFlows)
                        {
                            if ((firstFlow.Key & secondFlow.Key) != 0) continue;

                            puzzle2 = Math.Max(puzzle2, firstFlow.Value + secondFlow.Value);
                        }
                    }
                }
            }

            return ($"{puzzle1}", $"{puzzle2}");
        }

        [Answer("5367037", "11914583249288")]
        private static (string, string) Day15(IEnumerable<Match> input)
        {
            var hasNoBeacons = new HashSet<long>();
            var hasBeacons = new HashSet<long>();
            var allReadings = new List<(long sx, long sy, long distance)>();

            foreach (var match in input)
            {
                var sx = Int64.Parse(match.Groups["sx"].Value);
                var sy = Int64.Parse(match.Groups["sy"].Value);
                var bx = Int64.Parse(match.Groups["bx"].Value);
                var by = Int64.Parse(match.Groups["by"].Value);

                if (by == 2_000_000) hasBeacons.Add(bx);

                var maxDistance = Math.Abs(sx - bx) + Math.Abs(sy - by);
                allReadings.Add((sx, sy, maxDistance));

                var yDistance = Math.Abs(sy - 2_000_000);
                var xDistance = maxDistance - yDistance;
                if (xDistance < 0) continue;
                for (var x = sx - xDistance; x <= sx + xDistance; x++)
                {
                    hasNoBeacons.Add(x);
                }
            }

            long puzzle2 = 0;
            for (var y = 0L; y <= 4_000_000 && puzzle2 == 0; y++)
            {
                for (var x = 0L; x <= 4_000_000; )
                {
                    var reading = allReadings.Where(r => Math.Abs(r.sx - x) + Math.Abs(r.sy - y) < r.distance).FirstOrDefault();
                    if (reading == default((long sx, long sy, long distance)))
                    {
                        puzzle2 = x * 4_000_000 + y;
                        break;
                    }

                    x = reading.sx + reading.distance - Math.Abs(reading.sy - y) + 1;
                }
            }

            return ($"{hasNoBeacons.Count - hasBeacons.Count}", $"{puzzle2}");
        }

        [Answer("913", "30762")]
        private static (string, string) Day14(IEnumerable<string> input)
        {
            var occupied = new HashSet<(int x, int y)>();

            foreach (var line in input)
            {
                var points = line.Split(" -> ").Select(p => p.Split(',').Select(Int32.Parse).ToArray()).ToArray();
                for (var index = 1; index < points.Length; index++)
                {
                    var start = points[index - 1];
                    var end = points[index];
                    for (int x = start[0], y = start[1]; x != end[0] || y != end[1]; x += Math.Sign(end[0] - start[0]), y += Math.Sign(end[1] - start[1]))
                    {
                        occupied.Add((x, y));
                    }

                    occupied.Add((end[0], end[1]));
                }
            }

            var bottom = occupied.Max(o => o.y);
            (int x, int y) current = (500, 0);
            int puzzle1 = 0, puzzle2 = 0;
            var puzzle1Complete = false;
            while (true)
            {
                if (current.y == bottom + 1)
                {
                    occupied.Add(current);
                    current = (500, 0);
                    puzzle2++;
                    puzzle1Complete = true;
                }
                else if (!occupied.Contains((current.x, current.y + 1)))
                {
                    current = (current.x, current.y + 1);
                }
                else if (!occupied.Contains((current.x - 1, current.y + 1)))
                {
                    current = (current.x - 1, current.y + 1);
                }
                else if (!occupied.Contains((current.x + 1, current.y + 1)))
                {
                    current = (current.x +  1, current.y + 1);
                }
                else if (current.y == 0)
                {
                    puzzle2++;
                    break;
                }
                else
                {
                    occupied.Add(current);
                    current = (500, 0);
                    if (!puzzle1Complete) puzzle1++;
                    puzzle2++;
                }
            }

            return ($"{puzzle1}", $"{puzzle2}");
        }

        [Answer("5003", "20280")]
        private static (string, string) Day13(IEnumerable<IEnumerable<string>> input)
        {
            var puzzle1 = 0L;
            var index = 0;
            var allSignals = new List<DistressSignal>();

            foreach (var pair in input.Select(x => x.ToArray()))
            {
                index++;
                var a = ParseDistressSignal(pair[0]);
                var b = ParseDistressSignal(pair[1]);
                allSignals.Add(a);
                allSignals.Add(b);

                if (CompareDistressSignals(a, b) < 0) puzzle1 += index;
            }

            var lessThan2 = allSignals.Where(s => CompareDistressSignals(s, new DistressSignal { Value = 2 }) < 0).Count();
            var lessThan6 = allSignals.Where(s => CompareDistressSignals(s, new DistressSignal { Value = 6 }) < 0).Count();
            var puzzle2 = (lessThan2 + 1) * (lessThan6 + 2);

            return ($"{puzzle1}", $"{puzzle2}");
        }

        private static DistressSignal ParseDistressSignal(string input)
        {
            var signal = new DistressSignal();

            var current = signal;
            for (var index = 1; index < input.Length; index++)
            {
                switch (input[index])
                {
                    case '[':
                        var next = new DistressSignal { Parent = current };
                        current.Children.Add(next);
                        current = next;
                        break;

                    case ']':
                        current = current.Parent;
                        break;

                    case ',':
                        break;

                    default:
                        var value = Int32.Parse(String.Join("", input.Skip(index).TakeWhile(ch => Char.IsDigit(ch))));
                        current.Children.Add(new DistressSignal { Parent = current, Value = value });
                        break;
                }
            }

            return signal;
        }

        private class DistressSignal
        {
            private readonly List<DistressSignal> _children = new List<DistressSignal>();
            public DistressSignal Parent { get; set; }
            public int Value { get; set; } = -1;
            public IList<DistressSignal> Children => _children;
        }

        private static int CompareDistressSignals(DistressSignal a, DistressSignal b)
        {
            if (a.Value > -1 && b.Value > -1) return a.Value.CompareTo(b.Value);

            if (a.Value > -1)
            {
                a.Children.Add(new DistressSignal { Parent = a, Value = a.Value });
                a.Value = -1;
                return CompareDistressSignals(a, b);
            }

            if (b.Value > -1)
            {
                b.Children.Add(new DistressSignal { Parent = b, Value = b.Value });
                b.Value = -1;
                return CompareDistressSignals(a, b);
            }

            for (var index = 0; index < a.Children.Count && index < b.Children.Count; index++)
            {
                var partial = CompareDistressSignals(a.Children[index], b.Children[index]);
                if (partial != 0) return partial;
            }

            return a.Children.Count.CompareTo(b.Children.Count);
        }

        private class Coordinate
        {
            public Coordinate(char value, int x, int y)
            {
                if (value == 'S')
                {
                    IsStart = true;
                    Height = 0;
                }
                else if (value == 'E')
                {
                    IsEnd = true;
                    Height = 25;
                }
                else
                {
                    Height = value - 'a';
                }

                this.X = x;
                this.Y = y;
                this.StepsAway = Int32.MaxValue;
            }

            public bool IsStart;
            public bool IsEnd;
            public int Height;
            public int X;
            public int Y;

            public int StepsAway;
        }

        [Answer("468", "459")]
        private static (string, string) Day12(IEnumerable<string> input)
        {
            var map = input.Select((line, y) => line.Select((value, x) => new Coordinate(value, x, y)).ToArray()).ToArray();
            var allCoordinates = map.SelectMany(x => x);
            var width = map[0].Length;
            var height = map.Length;

            var end = allCoordinates.Single(x => x.IsEnd);
            end.StepsAway = 0;

            var testing = new Queue<Coordinate>();
            testing.Enqueue(end);

            while (testing.Any())
            {
                var node = testing.Dequeue();

                var nextSteps = node.StepsAway + 1;

                if (node.X > 0)
                {
                    var next = map[node.Y][node.X - 1];
                    if (next.StepsAway > nextSteps && node.Height <= next.Height + 1)
                    {
                        next.StepsAway = nextSteps;
                        testing.Enqueue(next);
                    }
                }

                if (node.Y > 0)
                {
                    var next = map[node.Y - 1][node.X];
                    if (next.StepsAway > nextSteps && node.Height <= next.Height + 1)
                    {
                        next.StepsAway = nextSteps;
                        testing.Enqueue(next);
                    }
                }

                if (node.X < width - 1)
                {
                    var next = map[node.Y][node.X + 1];
                    if (next.StepsAway > nextSteps && node.Height <= next.Height + 1)
                    {
                        next.StepsAway = nextSteps;
                        testing.Enqueue(next);
                    }
                }

                if (node.Y < height - 1)
                {
                    var next = map[node.Y + 1][node.X];
                    if (next.StepsAway > nextSteps && node.Height <= next.Height + 1)
                    {
                        next.StepsAway = nextSteps;
                        testing.Enqueue(next);
                    }
                }
            }

            var puzzle1 = allCoordinates.Single(c => c.IsStart).StepsAway;
            var puzzle2 = allCoordinates.Where(c => c.Height == 0).Min(c => c.StepsAway);

            return ($"{puzzle1}", $"{puzzle2}");
        }

        private class Monkey
        {
            private readonly Queue<long> _items1 = new Queue<long>();
            private readonly Queue<long> _items2 = new Queue<long>();

            public Queue<long> Items1 => _items1;
            public Queue<long> Items2 => _items2;
            public Func<long, long> Operation1 { get; set; }
            public Func<long, long> Operation2 { get; set; }
            public long TestDivisor { get; set; }
            public int TrueMonkey { get; set; }
            public int FalseMonkey { get; set; }
            public long Inspections1 { get; set; }
            public long Inspections2 { get; set; }
        }

        [Answer("99840", "20683044837")]
        private static (string, string) Day11(IEnumerable<IEnumerable<string>> input)
        {
            const string ItemsPrefix = "  Starting items: ";
            const string OperationPrefix = "  Operation: new = old ";
            const string TestPrefix = "  Test: divisible by ";
            const string TruePrefix = "    If true: throw to monkey ";
            const string FalsePrefix = "    If false: throw to monkey ";

            var monkeys = new List<Monkey>();

            var commonDivisor = 1L;
            foreach (var lines in input.Select(x => x.ToArray()))
            {
                var monkey = new Monkey();

                if (!lines[1].StartsWith(ItemsPrefix)) throw new Exception("Starting items failure.");
                var items = lines[1].Substring(ItemsPrefix.Length).Split(", ");
                foreach (var item in items)
                {
                    var parsedItem = Int64.Parse(item);
                    monkey.Items1.Enqueue(parsedItem);
                    monkey.Items2.Enqueue(parsedItem);
                }

                if (!lines[2].StartsWith(OperationPrefix)) throw new Exception("Operation failure.");
                var parts = lines[2].Substring(OperationPrefix.Length).Split(' ');
                var parameter = Expression.Parameter(typeof(long));
                Expression second = parts[1] == "old" ? parameter : Expression.Constant(Int64.Parse(parts[1]));
                switch (parts[0])
                {
                    case "+":
                        monkey.Operation1 = Expression.Lambda<Func<long, long>>(Expression.Divide(Expression.Add(parameter, second), Expression.Constant(3L)), parameter).Compile();
                        monkey.Operation2 = Expression.Lambda<Func<long, long>>(Expression.Add(parameter, second), parameter).Compile();
                        break;

                    case "*":
                        monkey.Operation1 = Expression.Lambda<Func<long, long>>(Expression.Divide(Expression.Multiply(parameter, second), Expression.Constant(3L)), parameter).Compile();
                        monkey.Operation2 = Expression.Lambda<Func<long, long>>(Expression.Multiply(parameter, second), parameter).Compile();
                        break;

                    default:
                        throw new Exception("Unrecognized operator.");
                }

                if (!lines[3].StartsWith(TestPrefix)) throw new Exception("Test failure.");
                monkey.TestDivisor = Int64.Parse(lines[3].Substring(TestPrefix.Length));
                commonDivisor = LCM(commonDivisor, monkey.TestDivisor);

                if (!lines[4].StartsWith(TruePrefix)) throw new Exception("True monkey failure.");
                monkey.TrueMonkey = Int32.Parse(lines[4].Substring(TruePrefix.Length));

                if (!lines[5].StartsWith(FalsePrefix)) throw new Exception("False monkey failure.");
                monkey.FalseMonkey = Int32.Parse(lines[5].Substring(FalsePrefix.Length));

                monkeys.Add(monkey);
            }

            for (var iteration = 0; iteration < 20; iteration++)
            {
                for (var index = 0; index < monkeys.Count; index++)
                {
                    var monkey = monkeys[index];
                    while (monkey.Items1.Any())
                    {
                        var item = monkey.Items1.Dequeue();
                        item = monkey.Operation1(item);
                        if (item % monkey.TestDivisor == 0)
                        {
                            monkeys[monkey.TrueMonkey].Items1.Enqueue(item);
                        }
                        else
                        {
                            monkeys[monkey.FalseMonkey].Items1.Enqueue(item);
                        }

                        monkey.Inspections1++;
                    }
                }
            }

            for (var iteration = 0; iteration < 10_000; iteration++)
            {
                for (var index = 0; index < monkeys.Count; index++)
                {
                    var monkey = monkeys[index];
                    while (monkey.Items2.Any())
                    {
                        var item = monkey.Items2.Dequeue();
                        item = monkey.Operation2(item);
                        if (item % monkey.TestDivisor == 0)
                        {
                            monkeys[monkey.TrueMonkey].Items2.Enqueue(item % commonDivisor);
                        }
                        else
                        {
                            monkeys[monkey.FalseMonkey].Items2.Enqueue(item % commonDivisor);
                        }

                        monkey.Inspections2++;
                    }
                }
            }

            var puzzle1 = monkeys.OrderByDescending(monkey => monkey.Inspections1).Take(2).Aggregate(1L, (x, monkey) => x * monkey.Inspections1);
            var puzzle2 = monkeys.OrderByDescending(monkey => monkey.Inspections2).Take(2).Aggregate(1L, (x, monkey) => x * monkey.Inspections2);

            return ($"{puzzle1}", $"{puzzle2}");
        }

        [Answer("13480")]
        private static (string, string) Day10(IEnumerable<string> input)
        {
            var cycle = 0;
            var register = 1;
            var puzzle1 = 0;
            var puzzle2 = new char[6][];
            for (var row = 0; row < 6; row++) puzzle2[row] = new char[40];

            foreach (var line in input)
            {
                switch (line[0])
                {
                    case 'n':
                        puzzle2[cycle/40][cycle%40] = Math.Abs(register - (cycle % 40)) <= 1 ? '#' : '.';
                        if (++cycle % 40 == 20) puzzle1 += register * cycle;
                        break;

                    case 'a':
                        puzzle2[cycle/40][cycle%40] = Math.Abs(register - (cycle % 40)) <= 1 ? '#' : '.';
                        if (++cycle % 40 == 20) puzzle1 += register * cycle;
                        puzzle2[cycle/40][cycle%40] = Math.Abs(register - (cycle % 40)) <= 1 ? '#' : '.';
                        if (++cycle % 40 == 20) puzzle1 += register * cycle;
                        register += Int32.Parse(line.Substring(5));
                        break;

                    default:
                        throw new Exception($"Unexpected input: {line}");
                }

                if (cycle >= 240) break;
            }

            var puzzle2Result = String.Join('\n', puzzle2.Select(x => String.Join("", x)));

            return ($"{puzzle1}", $"\n{puzzle2Result}");
        }

        [Answer("6337", "2455")]
        private static (string, string) Day9(IEnumerable<string> input)
        {
            var knots = new (int x, int y)[10];
            for (var index = 0; index < 10; index++) knots[index] = (0, 0);

            var locations1 = new HashSet<(int, int)> { knots[1] };
            var locations2 = new HashSet<(int, int)> { knots[9] };

            foreach (var line in input)
            {
                Func<(int x, int y), (int x, int y)> direction;
                switch (line[0])
                {
                    case 'R':
                        direction = ((int x, int y) head) => (head.x + 1, head.y);
                        break;

                    case 'L':
                        direction = ((int x, int y) head) => (head.x - 1, head.y);
                        break;

                    case 'U':
                        direction = ((int x, int y) head) => (head.x, head.y + 1);
                        break;

                    case 'D':
                        direction = ((int x, int y) head) => (head.x, head.y - 1);
                        break;

                    default:
                        throw new Exception($"Unhandled direction ({line[0]}) in input.");
                }

                var count = Int32.Parse(line.Substring(2));
                for (var iteration = 0; iteration < count; iteration++)
                {
                    knots[0] = direction(knots[0]);
                    for (var index = 1; index < 10; index++)
                    {
                        if (Math.Abs(knots[index - 1].x - knots[index].x) <= 1 && Math.Abs(knots[index - 1].y - knots[index].y) <= 1) break;

                        knots[index] = (knots[index].x + Math.Sign(knots[index - 1].x - knots[index].x), knots[index].y + Math.Sign(knots[index - 1].y - knots[index].y));
                        if (index == 1) locations1.Add(knots[index]);
                        else if (index == 9) locations2.Add(knots[index]);
                    }
                }
            }

            return ($"{locations1.Count}", $"{locations2.Count}");
        }

        [Answer("1818", "368368")]
        private static (string, string) Day8(IEnumerable<string> input)
        {
            var treeArray = input.Select(row => row.ToCharArray().Select(c => c - '0').ToArray()).ToArray();
            var height = treeArray.Length;
            var width = treeArray[0].Length;

            var visibleArray = new bool[height][];

            var scenicArray = new int[height][];
            for (var rowIndex = 0; rowIndex < height; rowIndex++)
            {
                scenicArray[rowIndex] = new int[width];
                for (var columnIndex = 0; columnIndex < width; columnIndex++) scenicArray[rowIndex][columnIndex] = 1;
            }

            for (var rowIndex = 0; rowIndex < height; rowIndex++)
            {
                visibleArray[rowIndex] = new bool[width];
                scenicArray[rowIndex] = new int[width];

                for (var columnIndex = 0; columnIndex < width; columnIndex++)
                {
                    if (rowIndex == 0 || rowIndex == height - 1 || columnIndex == 0 || columnIndex == width - 1)
                    {
                        visibleArray[rowIndex][columnIndex] = true;
                        scenicArray[rowIndex][columnIndex] = 0;
                        continue;
                    }

                    scenicArray[rowIndex][columnIndex] = 1;

                    for (var rowSeek = rowIndex - 1; rowSeek >= 0; rowSeek--)
                    {
                        if (treeArray[rowIndex][columnIndex] <= treeArray[rowSeek][columnIndex])
                        {
                            scenicArray[rowIndex][columnIndex] *= rowIndex - rowSeek;
                            break;
                        }

                        if (rowSeek == 0)
                        {
                            visibleArray[rowIndex][columnIndex] = true;
                            scenicArray[rowIndex][columnIndex] *= rowIndex - rowSeek;
                        }
                    }

                    for (var rowSeek = rowIndex + 1; rowSeek < height; rowSeek++)
                    {
                        if (treeArray[rowIndex][columnIndex] <= treeArray[rowSeek][columnIndex])
                        {
                            scenicArray[rowIndex][columnIndex] *= rowSeek - rowIndex;
                            break;
                        }

                        if (rowSeek == height - 1)
                        {
                            visibleArray[rowIndex][columnIndex] = true;
                            scenicArray[rowIndex][columnIndex] *= rowSeek - rowIndex;
                        }
                    }

                    for (var columnSeek = columnIndex - 1; columnSeek >= 0; columnSeek--)
                    {
                        if (treeArray[rowIndex][columnIndex] <= treeArray[rowIndex][columnSeek])
                        {
                            scenicArray[rowIndex][columnIndex] *= columnIndex - columnSeek;
                            break;
                        }

                        if (columnSeek == 0)
                        {
                            visibleArray[rowIndex][columnIndex] = true;
                            scenicArray[rowIndex][columnIndex] *= columnIndex - columnSeek;
                        }
                    }

                    for (var columnSeek = columnIndex + 1; columnSeek < width; columnSeek++)
                    {
                        if (treeArray[rowIndex][columnIndex] <= treeArray[rowIndex][columnSeek])
                        {
                            scenicArray[rowIndex][columnIndex] *= columnSeek - columnIndex;
                            break;
                        }

                        if (columnSeek == width - 1)
                        {
                            visibleArray[rowIndex][columnIndex] = true;
                            scenicArray[rowIndex][columnIndex] *= columnSeek - columnIndex;
                        }
                    }
                }
            }

            var puzzle1 = visibleArray.Sum(x => x.Where(y => y).Count());
            var puzzle2 = scenicArray.Max(x => x.Max());

            return ($"{puzzle1}", $"{puzzle2}");
        }

        private class File
        {
            public string Name { get; set; }
            public long Size { get; set; }
        }

        private class Directory
        {
            public Directory() => (Files, SubDirectories) = (new List<File>(), new Dictionary<string, Directory>());

            public string Name { get; set; }
            public Directory Parent { get; set; }
            public IList<File> Files { get; }
            public IDictionary<string, Directory> SubDirectories { get; }

            public long Size => this.Files.Sum(f => f.Size) + this.SubDirectories.Sum(d => d.Value.Size);
        }

        [Answer("1844187", "4978279")]
        private static (string, string) Day7(IEnumerable<string> input)
        {
            var root = new Directory { Name = "/" };
            var current = root;

            var allDirectories = new List<Directory> { root };

            foreach (var line in input)
            {
                if (String.IsNullOrWhiteSpace(line)) continue;

                var parts = line.Split(' ');
                switch (parts[0])
                {
                    case "$":
                        switch (parts[1])
                        {
                            case "cd":
                                switch (parts[2])
                                {
                                    case "/":
                                        current = root;
                                        break;

                                    case "..":
                                        current = current.Parent;
                                        break;

                                    default:
                                        current = current.SubDirectories[parts[2]];
                                        break;
                                }
                                break;

                            case "ls":
                                break;
                        }
                        break;

                    case "dir":
                        var newDirectory = new Directory { Name = parts[1], Parent = current };
                        current.SubDirectories.Add(parts[1], newDirectory);
                        allDirectories.Add(newDirectory);
                        break;

                    default:
                        current.Files.Add(new File { Name = parts[1], Size = Int64.Parse(parts[0]) });
                        break;
                }
            }

            var puzzle1 = allDirectories.Where(d => d.Size <= 100000).Sum(d => d.Size);

            var availableSpace = 70_000_000 - root.Size;
            var neededSpace = 30_000_000 - availableSpace;
            var puzzle2 = allDirectories.Where(d => d.Size > neededSpace).Min(d => d.Size);

            return ($"{puzzle1}", $"{puzzle2}");
        }

        [Answer("1356", "2564")]
        private static (string, string) Day6(string input)
        {
            long puzzle1 = 0L, puzzle2 = 0L;

            for (var index = 0; index < input.Length - 3; index++)
            {
                if (puzzle1 == 0)
                {
                    var distinctCount = input[index..(index+4)].Distinct().Count();
                    if (distinctCount == 4) puzzle1 = index + 4;
                }

                var distinctCount2 = input[index..(index+14)].Distinct().Count();
                if (distinctCount2 == 14) puzzle2 = index + 14;

                if (puzzle2 != 0) break;
            }

            return ($"{puzzle1}", $"{puzzle2}");
        }

        [Answer("HBTMTBSDC", "PQTJRSHWS")]
        private static (string, string) Day5(IEnumerable<string> input)
        {
            var instructionRegex = new Regex(@"^move (?<Count>\d+) from (?<Source>\d+) to (?<Destination>\d+)$");

            Stack<char>[] stacks1 = new Stack<char>[0], stacks2 = new Stack<char>[0], stacks = new Stack<char>[9];
            for (var index = 0; index < stacks.Length; index++) stacks[index] = new Stack<char>();

            var isParsingStacks = true;
            foreach (var line in input)
            {
                if (String.IsNullOrWhiteSpace(line))
                {
                    stacks1 = stacks.Select(stack => new Stack<char>(stack)).ToArray();
                    stacks2 = stacks.Select(stack => new Stack<char>(stack)).ToArray();
                    isParsingStacks = false;
                    continue;
                }

                if (isParsingStacks)
                {
                    if (!line.Contains('[')) continue;

                    for (var index = 1; index < line.Length; index += 4)
                    {
                        var value = line[index];
                        if (!Char.IsLetter(value)) continue;
                        stacks[(index - 1) / 4].Push(value);
                    }

                    continue;
                }

                var match = instructionRegex.Match(line);
                if (!match.Success) throw new Exception($"Unexpected failed to parse input: {line}");

                var count = Int32.Parse(match.Groups["Count"].Value);
                var source = Int32.Parse(match.Groups["Source"].Value) - 1;
                var destination = Int32.Parse(match.Groups["Destination"].Value) - 1;

                for (var x = 0; x < count; x++)
                {
                    stacks1[destination].Push(stacks1[source].Pop());
                }

                var tempStack = new Stack<char>();
                for (var x = 0; x < count; x++) tempStack.Push(stacks2[source].Pop());
                for (var x = 0; x < count; x++) stacks2[destination].Push(tempStack.Pop());
            }

            var puzzle1 = String.Join("", stacks1.Select(stack => stack.Peek()));
            var puzzle2 = String.Join("", stacks2.Select(stack => stack.Peek()));

            return (puzzle1, puzzle2);
        }

        [Answer("507", "897")]
        private static (string, string) Day4(IEnumerable<Match> input)
        {
            long puzzle1 = 0L, puzzle2 = 0L;

            foreach (var match in input)
            {
                var min1 = Int64.Parse(match.Groups["Min1"].Value);
                var max1 = Int64.Parse(match.Groups["Max1"].Value);
                var min2 = Int64.Parse(match.Groups["Min2"].Value);
                var max2 = Int64.Parse(match.Groups["Max2"].Value);

                if (min1 <= min2 && max1 >= max2) puzzle1++;
                else if (min1 >= min2 && max1 <= max2) puzzle1++;

                if (min1 <= max2 && min2 <= max1) puzzle2++;
            }

            return ($"{puzzle1}", $"{puzzle2}");
        }

        [Answer("7863", "2488")]
        private static (string, string) Day3(IEnumerable<string> input)
        {
            long priority1 = 0L, priority2 = 0L;

            var index = 0;
            char[][] rucksacks = new char[3][];
            foreach (var line in input)
            {
                var packSize = line.Length;
                var compartmentSize = packSize >> 1;
                var rucksack = line.ToCharArray();

                var overlap = rucksack[0..compartmentSize].Intersect(rucksack[compartmentSize..packSize]).Single();
                priority1 += (Char.IsLower(overlap) ? (1 + overlap - 'a') : (27 + overlap - 'A'));

                rucksacks[index++] = rucksack;
                if (index == 3)
                {
                    index = 0;
                    var badge = rucksacks[0].Intersect(rucksacks[1]).Intersect(rucksacks[2]).Single();
                    priority2 += (Char.IsLower(badge) ? (1 + badge - 'a') : (27 + badge - 'A'));
                }
            }

            return ($"{priority1}", $"{priority2}");
        }

        [Answer("11150", "8295")]
        private static (string, string) Day2(IEnumerable<string> input)
        {
            // A - rock, B - paper, C - scissors
            // X - rock, Y - paper, C - scissors

            var score1 = 0L;
            var score2 = 0L;
            foreach (var line in input)
            {
                var opponentPlay = line[0] - 'A';
                var myPlay = line[2] - 'X';

                score1 += myPlay + 1;
                if (myPlay == opponentPlay)
                {
                    score1 += 3;
                }
                else if (myPlay == opponentPlay + 1 || myPlay == opponentPlay - 2)
                {
                    score1 += 6;
                }

                // X - lose, Y - draw, Z - win
                score2 += myPlay * 3;
                score2 += ((opponentPlay + myPlay + 2) % 3) + 1;
            }

            return ($"{score1}", $"{score2}");
        }

        [Answer("71924", "210406")]
        private static (string, string) Day1(IEnumerable<IEnumerable<long>> input)
        {
            var top3 = input
                .Select(batch => batch.Sum())
                .OrderByDescending(x => x)
                .Take(3);

            return ($"{top3.First()}", $"{top3.Sum()}");
        }
    }
}
