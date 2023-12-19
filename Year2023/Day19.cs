using System.Text.RegularExpressions;

namespace Moyba.AdventOfCode.Year2023
{
    using WorkflowRaw = (string name, string rules, string defaultWorkflow);
    using Workflow = (ICollection<(Day19.WorkflowCondition condition, string nextWorkflow)> rules, string defaultWorkflow);
    using Part = IDictionary<char, long>;
    using PartRange = IDictionary<char, SortedList<long, long>>;

    public class Day19 : IPuzzle
    {
        private static readonly Regex _WorkflowParser = new Regex(@"^([a-z]+){(.*),([ARa-z]+)}$");
        private static readonly Regex _PartParser = new Regex(@"^{x=(\d+),m=(\d+),a=(\d+),s=(\d+)}$", RegexOptions.Compiled);

        private static readonly HashSet<string> _FinalWorkflows = new HashSet<string> { "A", "R" };

        private readonly IDictionary<string, Workflow> _workflows;
        private readonly Part[] _parts;

        public Day19(string[] data)
        {
            var clusters = data.Cluster().ToArray();

            _workflows = clusters[0]
                .Transform<WorkflowRaw>(_WorkflowParser)
                .ToDictionary<WorkflowRaw, string, Workflow>(
                    _ => _.name,
                    _ =>
                    (
                        _.rules
                            .Split(',')
                            .Select(_ => _.Split(':'))
                            .Select(_ => (new WorkflowCondition(_[0]), _[1]))
                            .ToArray(),
                        _.defaultWorkflow
                    )
                );

            _parts = clusters[1]
                .Transform<(long x, long m, long a, long s)>(_PartParser)
                .Select(_ => new Dictionary<char, long> { { 'x', _.x }, { 'm', _.m }, { 'a', _.a }, { 's', _.s } })
                .ToArray();
        }

        [PartOne("367602")]
        [PartTwo("125317461667458")]
        public async IAsyncEnumerable<string?> ComputeAsync()
        {
            var acceptedPartsSum = 0L;
            foreach (var part in _parts)
            {
                var workflowName = "in";
                do
                {
                    var workflow = _workflows[workflowName];
                    workflowName = workflow.defaultWorkflow;
                    foreach (var rule in workflow.rules)
                    {
                        if (rule.condition.Evaluate(part))
                        {
                            workflowName = rule.nextWorkflow;
                            break;
                        }
                    }
                }
                while (!_FinalWorkflows.Contains(workflowName));

                if (workflowName.Equals("A")) acceptedPartsSum += part.Values.Sum();
            }

            yield return $"{acceptedPartsSum}";

            var acceptedRatings = 0L;
            var queue = new Queue<(string workflowName, PartRange parts)>();
            var allParts = new Dictionary<char, SortedList<long, long>>
            {
                { 'x', new SortedList<long, long> { { 1, 4001 } } },
                { 'm', new SortedList<long, long> { { 1, 4001 } } },
                { 'a', new SortedList<long, long> { { 1, 4001 } } },
                { 's', new SortedList<long, long> { { 1, 4001 } } },
            };
            queue.Enqueue(("in", allParts));
            while (queue.TryDequeue(out var entry))
            {
                (var workflowName, var parts) = entry;

                switch (workflowName)
                {
                    case "A":
                        acceptedRatings += parts.Values.Aggregate(1L, (product, ratings) => product * ratings.Sum(rating => rating.Value - rating.Key));
                        continue;

                    case "R":
                        continue;
                }

                var workflow = _workflows[workflowName];
                foreach (var rule in workflow.rules)
                {
                    var matchingParts = rule.condition.Split(parts);
                    if (matchingParts.Values.All(_ => _.Count > 0)) queue.Enqueue((rule.nextWorkflow, matchingParts));
                }

                if (parts.Values.All(_ => _.Count > 0)) queue.Enqueue((workflow.defaultWorkflow, parts));
            }

            yield return $"{acceptedRatings}";

            await Task.CompletedTask;
        }

        internal class WorkflowCondition
        {
            public WorkflowCondition(string serialized)
            {
                this.Key = serialized[0];
                this.Operation = serialized[1];
                this.Value = Int64.Parse(serialized[2..]);

                this.Evaluate = this.Operation switch
                {
                    '<' => (Part part) => part[this.Key] < this.Value,
                    '>' => (Part part) => part[this.Key] > this.Value,
                    _ => throw new Exception($"Unhandled condition operation ({this.Operation}).")
                };
            }

            public char Key { get; }
            public char Operation { get; }
            public long Value { get; }

            public Func<Part, bool> Evaluate { get; }

            public PartRange Split(PartRange parts)
            {
                var matchingParts = new Dictionary<char, SortedList<long, long>>();

                // copy all ratings which aren't being evaluated
                foreach (var key in parts.Keys)
                {
                    if (key == this.Key) matchingParts.Add(key, new SortedList<long, long>());
                    else matchingParts.Add(key, new SortedList<long, long>(parts[key]));
                }

                switch (this.Operation)
                {
                    case '<':
                        this.SplitForLessThan(parts, matchingParts);
                        break;

                    case '>':
                        this.SplitForGreaterThan(parts, matchingParts);
                        break;
                }

                return matchingParts;
            }

            private void SplitForGreaterThan(PartRange source, PartRange target)
            {
                var sourceRanges = source[this.Key];
                var targetRanges = target[this.Key];

                var splitValue = this.Value + 1;
                for (var index = sourceRanges.Count - 1; index >= 0; index--)
                {
                    var sourceRangeStart = sourceRanges.GetKeyAtIndex(index);
                    var sourceRangeEnd = sourceRanges[sourceRangeStart];

                    if (sourceRangeEnd <= splitValue) return;

                    if (sourceRangeStart < splitValue)
                    {
                        targetRanges.Add(splitValue, sourceRangeEnd);
                        sourceRanges[sourceRangeStart] = splitValue;
                        return;
                    }

                    sourceRanges.Remove(sourceRangeStart);
                    targetRanges.Add(sourceRangeStart, sourceRangeEnd);
                }
            }

            private void SplitForLessThan(PartRange source, PartRange target)
            {
                var sourceRanges = source[this.Key];
                var targetRanges = target[this.Key];

                var splitValue = this.Value;
                for (var index = 0; index < sourceRanges.Count; index++)
                {
                    var sourceRangeStart = sourceRanges.GetKeyAtIndex(index);
                    var sourceRangeEnd = sourceRanges[sourceRangeStart];

                    if (sourceRangeStart >= splitValue) return;

                    sourceRanges.Remove(sourceRangeStart);

                    if (sourceRangeEnd > splitValue)
                    {
                        targetRanges.Add(sourceRangeStart, splitValue);
                        sourceRanges.Add(splitValue, sourceRangeEnd);
                        return;
                    }

                    targetRanges.Add(sourceRangeStart, sourceRangeEnd);
                }
            }
        }
    }
}
