using System.Security.Cryptography;

namespace Moyba.AdventOfCode.Year2015
{
    public class Day4 : SolutionBase<string>
    {
        private byte[] _data = Array.Empty<byte>();
        private int _value = 0;
        private int _part1 = 0;
        private int _part2 = 0;

        protected override string ReadInput(IEnumerable<string> input) => input.First();

        [Expect("346386")]
        protected override string SolvePart1()
        {
            return $"{_part1}";
        }

        [Expect("9958218")]
        protected override string SolvePart2()
        {
            return $"{_part2}";
        }

        protected override void TransformData(string data)
        {
            _data = data.Select(c => (byte)c).ToArray();
            Task.WaitAll(Enumerable.Range(0, SolutionBase.ParallelThreads).Select(_ => Task.Run(this.FindHashes)).ToArray());
        }

        private void FindHashes()
        {
            var source = new byte[_data.Length];

            while (_part1 == 0)
            {
                var value = Interlocked.Increment(ref _value);
                this.UpdateSourceArray(ref source, value);
                var hash = MD5.HashData(source);
                if (hash[0] != 0 || hash[1] != 0 || hash[2] > 0xF) continue;

                var part1 = _part1;
                while (part1 == 0 || part1 > value)
                {
                    Interlocked.CompareExchange(ref _part1, value, part1);
                    part1 = _part1;
                }
            }

            while (_part2 == 0)
            {
                var value = Interlocked.Increment(ref _value);
                this.UpdateSourceArray(ref source, value);
                var hash = MD5.HashData(source);
                if (hash[0] != 0 || hash[1] != 0 || hash[2] != 0) continue;

                var part2 = _part2;
                while (part2 == 0 || part2 > value)
                {
                    Interlocked.CompareExchange(ref _part2, value, part2);
                    part2 = _part2;
                }
            }
        }

        private void UpdateSourceArray(ref byte[] source, int value)
        {
            var input = $"{value}";
            if (source.Length < _data.Length + input.Length)
            {
                source = new byte[_data.Length + input.Length];
                _data.CopyTo(source, 0);
            }

            for (int inputIndex = 0, sourceIndex = _data.Length; inputIndex < input.Length; inputIndex++, sourceIndex++)
            {
                source[sourceIndex] = (byte)input[inputIndex];
            }
        }
    }
}
