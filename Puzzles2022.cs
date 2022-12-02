namespace Moyba.AdventOfCode
{
    using strings = IEnumerable<string>;

    internal class Puzzles2022 : PuzzlesBase
    {
        protected override int Year => 2022;

        public override async Task SolveAsync()
        {
            await this.SolveAsync(() => Puzzles2022.Day1);
        }

        private static (string, string) Day1(strings input)
        {
            return ("", "");
        }
    }
}