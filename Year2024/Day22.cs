namespace Moyba.AdventOfCode.Year2024
{
    using Sequence = (long, long, long, long);

    public class Day22(string[] _data) : IPuzzle
    {
        private readonly long[] _secrets = _data.Select(Int64.Parse).ToArray();

        [PartOne("17960270302")]
        [PartTwo("2042")]
        public async IAsyncEnumerable<string?> ComputeAsync()
        {
            var numbers = _secrets.Select(_GenerateSecretNumbers).Select(_ => _.ToArray()).ToArray();

            var part1 = numbers.Sum(_ => _.Last());

            yield return $"{part1}";

            var sequenceBananas = new Dictionary<Sequence, long>();
            foreach (var numberSequence in numbers)
            {
                long s1 = Int64.MaxValue, s2 = Int64.MaxValue, s3 = Int64.MaxValue, s4 = Int64.MaxValue;
                var visited = new HashSet<Sequence>();
                var previous = numberSequence[0] % 10;
                for (var index = 1; index < numberSequence.Length; index++)
                {
                    var bananas = numberSequence[index] % 10;

                    s1 = s2;
                    s2 = s3;
                    s3 = s4;
                    s4 = bananas - previous;
                    previous = bananas;

                    if (s1 == Int64.MaxValue) continue;

                    var sequence = (s1, s2, s3, s4);
                    if (visited.Contains(sequence)) continue;

                    visited.Add(sequence);

                    if (!sequenceBananas.ContainsKey(sequence)) sequenceBananas.Add(sequence, 0);
                    sequenceBananas[sequence] += bananas;
                }
            }

            yield return $"{sequenceBananas.Values.Max()}";

            await Task.CompletedTask;
        }

        private static IEnumerable<long> _GenerateSecretNumbers(long secret)
        {
            yield return secret;
            for (var iteration = 0; iteration < 2000; iteration++)
            {
                secret = _GenerateNewSecret(secret);
                yield return secret;
            }
        }

        private static long _GenerateNewSecret(long secret)
        {
            secret = _Prune(_Mix(secret << 6, secret));
            secret = _Prune(_Mix(secret >> 5, secret));
            secret = _Prune(_Mix(secret << 11, secret));

            return secret;
        }

        private static long _Mix(long value, long secret)
            => secret ^ value;

        private static long _Prune(long secret)
            // => secret % 16777216;
            => secret & 0xffffff;
    }
}
