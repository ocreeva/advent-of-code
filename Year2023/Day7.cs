namespace Moyba.AdventOfCode.Year2023
{
    using Camel = (char[] cards, int bid);
    using Hand = (int type, int[] cards, int bid);

    public class Day7(string[] _data) : IPuzzle
    {
        private static readonly IComparer<Hand> _HandComparer = new HandComparer();

        private readonly Camel[] _camels = _data
            .Select(_ => _.Split(' '))
            .Select(_ => (_[0].ToCharArray(), Int32.Parse(_[1])))
            .ToArray();

        [PartOne("253954294")]
        [PartTwo("254837398")]
        public async IAsyncEnumerable<string> ComputeAsync()
        {
            var sortedHands = _camels.Select(_ => _TransformCamelToHand(_, _TransformCard)).Order(_HandComparer).ToArray();

            var totalWinnings = 0;
            for (var index = 0; index < sortedHands.Length; )
            {
                totalWinnings += sortedHands[index].bid * ++index;
            }

            yield return $"{totalWinnings}";

            sortedHands = _camels.Select(_ => _TransformCamelToHand(_, _TransformCardWithJokers)).Order(_HandComparer).ToArray();

            totalWinnings = 0;
            for (var index = 0; index < sortedHands.Length; )
            {
                totalWinnings += sortedHands[index].bid * ++index;
            }

            yield return $"{totalWinnings}";

            await Task.CompletedTask;
        }

        private class HandComparer : IComparer<Hand>
        {
            public int Compare(Hand hand1, Hand hand2)
            {
                if (hand1.type != hand2.type) return hand1.type.CompareTo(hand2.type);

                for (var index = 0; index < 5; index++)
                {
                    if (hand1.cards[index] != hand2.cards[index]) return hand1.cards[index].CompareTo(hand2.cards[index]);
                }

                return 0;
            }
        }

        private static Hand _TransformCamelToHand(Camel camel, Func<char, int> transformCard)
        {
            var cards = camel.cards.Select(transformCard).ToArray();

            var counts = cards
                .Where(_ => _ != 0)
                .GroupBy(_ => _)
                .Select(_ => _.Count())
                .OrderDescending()
                .ToArray();
            var jokers = 5 - counts.Sum();
            if (jokers == 5) return ((int)HandType.FiveOfAKind, cards, camel.bid);

            var type = (counts[0] + jokers) switch
            {
                5 => HandType.FiveOfAKind,
                4 => HandType.FourOfAKind,
                3 => counts[1] switch
                {
                    2 => HandType.FullHouse,
                    _ => HandType.ThreeOfAKind,
                },
                2 => counts[1] switch
                {
                    2 => HandType.TwoPair,
                    _ => HandType.OnePair,
                },
                _ => HandType.HighCard,
            };

            return ((int)type, cards, camel.bid);
        }

        private static int _TransformCard(char _) => _ switch
        {
            'A' => 14,
            'K' => 13,
            'Q' => 12,
            'J' => 11,
            'T' => 10,
            _ => _ - '0'
        };

        private static int _TransformCardWithJokers(char _) => _ switch
        {
            'A' => 14,
            'K' => 13,
            'Q' => 12,
            'J' => 0,
            'T' => 10,
            _ => _ - '0'
        };

        private enum HandType
        {
            HighCard = 0,
            OnePair,
            TwoPair,
            ThreeOfAKind,
            FullHouse,
            FourOfAKind,
            FiveOfAKind,
        }
    }
}
