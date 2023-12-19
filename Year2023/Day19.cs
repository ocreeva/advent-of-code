using System.Text.RegularExpressions;

namespace Moyba.AdventOfCode.Year2023
{
    using WorkflowRaw = (string name, string rules, string defaultWorkflow);
    using Workflow = (ICollection<(Day19.WorkflowCondition condition, string nextWorkflow)> rules, string defaultWorkflow);
    using Part = IDictionary<char, long>;

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
        [PartTwo()]
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

            yield return $"";

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
        }
    }
}
