namespace Moyba.AdventOfCode.Year2016
{
    using Coord = (int x, int y);
    using Instruction = (char turn, int distance);

    public class Day1(string[] data) : IPuzzle
    {
        private static readonly IDictionary<char, Func<Coord, Coord>> _TurnLookup = new Dictionary<char, Func<Coord, Coord>>
        {
            { 'L', _ => (-_.y, _.x) },
            { 'R', _ => (_.y, -_.x) },
        };

        private readonly Instruction[] _instructions = data
            .Single()
            .Split(", ")
            .Select<string, Instruction>(_ => (_[0], Int32.Parse(_[1..])))
            .ToArray();


        [PartOne("307")]
        [PartTwo("165")]
        public async IAsyncEnumerable<string> ComputeAsync()
        {
            Coord position = (0, 0);
            Coord duplicate = (0, 0);
            bool foundDuplicate = false;

            var visited = new HashSet<Coord>();
            Coord direction = (0, 1);

            foreach ((var turn, var distance) in _instructions)
            {
                direction = _TurnLookup[turn](direction);

                var step = 0;
                if (!foundDuplicate)
                {
                    while (step++ < distance)
                    {
                        position = (position.x + direction.x, position.y + direction.y);
                        if (visited.Contains(position))
                        {
                            foundDuplicate = true;
                            duplicate = position;
                            break;
                        }

                        visited.Add(position);
                    }
                }

                while (step++ < distance) position = (position.x + direction.x, position.y + direction.y);
            }

            yield return $"{Math.Abs(position.x) + Math.Abs(position.y)}";

            yield return $"{Math.Abs(duplicate.x) + Math.Abs(duplicate.y)}";

            await Task.CompletedTask;
        }
    }
}
