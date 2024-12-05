namespace Moyba.AdventOfCode.Year2024
{
    using Rule = (int before, int after);

    public class Day5(string[] _data) : IPuzzle
    {
        private readonly HashSet<Rule> _rules = _data
            .TakeWhile(_ => !String.IsNullOrEmpty(_))
            .Select(_ => _.Split('|'))
            .Select(_ => (Int32.Parse(_[0]), Int32.Parse(_[1])))
            .ToHashSet<Rule>();

        private readonly int[][] _updates = _data
            .SkipWhile(_ => !String.IsNullOrEmpty(_))
            .Skip(1)
            .Select(_ => _.Split(',').Select(x => Int32.Parse(x)).ToArray())
            .ToArray();

        [PartOne("4905")]
        [PartTwo("6204")]
        public async IAsyncEnumerable<string?> ComputeAsync()
        {
            var correctSum = 0;
            var incorrectSum = 0;
            foreach (var update in _updates)
            {
                var lookup = new Dictionary<int, int>();
                for (var i = 0; i < update.Length; i++)
                {
                    lookup.Add(update[i], i);
                }

                var ruleViolated = false;
                var foundViolation = false;
                do
                {
                    foundViolation = false;

                    foreach (var rule in _rules)
                    {
                        if (lookup.ContainsKey(rule.before)
                            && lookup.ContainsKey(rule.after)
                            && lookup[rule.before] > lookup[rule.after])
                        {
                            ruleViolated = true;
                            foundViolation = true;

                            var beforeIndex = lookup[rule.before];
                            var afterIndex = lookup[rule.after];

                            lookup[rule.before] = afterIndex;
                            lookup[rule.after] = beforeIndex;

                            update[beforeIndex] = rule.after;
                            update[afterIndex] = rule.before;
                        }
                    }
                } while (foundViolation);

                if (ruleViolated) incorrectSum += update[update.Length >> 1];
                else correctSum += update[update.Length >> 1];
            }

            yield return $"{correctSum}";

            yield return $"{incorrectSum}";

            await Task.CompletedTask;
        }
    }
}
