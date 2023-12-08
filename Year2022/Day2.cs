namespace Moyba.AdventOfCode.Year2022
{
    using Play = (int opponent, int my);

    public class Day2(string[] _data) : IPuzzle
    {
        private readonly Play[] _plays = _data.Select(_ => (_[0] - 'A', _[2] - 'X')).ToArray();

        [PartOne("11150")]
        [PartTwo("8295")]
        public async IAsyncEnumerable<string> ComputeAsync()
        {
            var score = 0L;
            foreach ((var opponentPlay, var myPlay) in _plays)
            {
                // A - rock, B - paper, C - scissors
                // X - rock, Y - paper, C - scissors
                score += myPlay + 1;
                if (myPlay == opponentPlay)
                {
                    score += 3;
                }
                else if (myPlay == opponentPlay + 1 || myPlay == opponentPlay - 2)
                {
                    score += 6;
                }
            }

            yield return $"{score}";

            score = 0;
            foreach ((var opponentPlay, var myPlay) in _plays)
            {
                // A - rock, B - paper, C - scissors
                // X - lose, Y - draw, Z - win
                score += myPlay * 3;
                score += ((opponentPlay + myPlay + 2) % 3) + 1;
            }

            yield return $"{score}";

            await Task.CompletedTask;
        }
    }
}
