namespace Moyba.AdventOfCode
{
    internal class Puzzles2022 : PuzzlesBase
    {
        protected override int Year => 2022;

        public override async Task SolveAsync()
        {
            await this.SolveAsync(() => Puzzles2022.Day1, LineDelimited, AsBatchesOfLongs);
            await this.SolveAsync(() => Puzzles2022.Day2);
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
