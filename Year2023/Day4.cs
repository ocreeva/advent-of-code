namespace Moyba.AdventOfCode.Year2023
{
    using Card = (int index, int[] winning, int[] numbers);

    public class Day4(IEnumerable<string> data) : IPuzzle
    {
        private readonly Card[] _cards = data
            .Select(_ => {
                var parts = _.Split(':', 2);
                var index = Int32.Parse(parts[0].Split(' ', StringSplitOptions.RemoveEmptyEntries)[1]);

                parts = parts[1].Split('|', 2);
                var winning = parts[0].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(Int32.Parse).ToArray();
                var numbers = parts[1].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(Int32.Parse).ToArray();

                return (index, winning, numbers);
            })
            .ToArray();

        private int _points;
        private int _totalCards;

        public Task ComputeAsync()
        {
            var cardCount = new int[_cards.Length];

            for (var index = 0; index < _cards.Length; index++)
            {
                var card = _cards[index];

                cardCount[index]++;

                var matches = card.winning.Intersect(card.numbers).Count();
                if (matches == 0) continue;

                _points += (int)Math.Pow(2, matches - 1);
                for (var extra = index + 1; extra <= index + matches && extra < _cards.Length; extra++) cardCount[extra] += cardCount[index];
            }

            _totalCards = cardCount.Sum();

            return Task.CompletedTask;
        }

        [Solution("15205")]
        public string PartOne => $"{_points}";

        [Solution("6189740")]
        public string PartTwo => $"{_totalCards}";
    }
}
