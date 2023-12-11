using System.Text.RegularExpressions;

namespace Moyba.AdventOfCode.Year2022
{
    using Instruction = (int count, int source, int target);

    public class Day5 : IPuzzle
    {
        private static readonly Regex _InstructionParser = new Regex(@"^move (\d+) from (\d+) to (\d+)$");

        private readonly char[][] _stacks;
        private readonly Instruction[] _instructions;

        public Day5(string[] data)
        {
            var dataSplit = Enumerable.Range(0, data.Length).Where(_ => String.IsNullOrEmpty(data[_])).First();

            var stackSize = (data[dataSplit - 2].Length + 1) / 4;
            _stacks = Enumerable.Range(0, stackSize)
                .Select(_ => 1 + 4 * _)
                .Select(charIndex => Enumerable.Range(0, dataSplit - 1)
                    .Select(_ => data[_][charIndex])
                    .Where(_ => _ != ' ')
                    .Reverse()
                    .ToArray()
                )
                .ToArray();
            
            _instructions = data
                .Skip(dataSplit + 1)
                .Transform<Instruction>(_InstructionParser)
                .ToArray();
        }

        [PartOne("HBTMTBSDC")]
        [PartTwo("PQTJRSHWS")]
        public async IAsyncEnumerable<string?> ComputeAsync()
        {
            var stacks = new Stack<char>[_stacks.Length];
            for (var index = 0; index < stacks.Length; index++) stacks[index] = new Stack<char>(_stacks[index]);

            foreach (var instruction in _instructions)
            {
                var source = stacks[instruction.source - 1];
                var target = stacks[instruction.target - 1];
                for (var count = 0; count < instruction.count; count++) target.Push(source.Pop());
            }

            var crates = String.Join("", stacks.Select(_ => _.Peek()));

            yield return crates;

            for (var index = 0; index < stacks.Length; index++) stacks[index] = new Stack<char>(_stacks[index]);

            var temp = new Stack<char>();
            foreach (var instruction in _instructions)
            {
                var source = stacks[instruction.source - 1];
                var target = stacks[instruction.target - 1];
                for (var count = 0; count < instruction.count; count++) temp.Push(source.Pop());
                for (var count = 0; count < instruction.count; count++) target.Push(temp.Pop());
            }

            crates = String.Join("", stacks.Select(_ => _.Peek()));

            yield return crates;

            await Task.CompletedTask;
        }
    }
}
