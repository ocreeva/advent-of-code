using Gear = (int cost, int damage, int defense);

namespace Moyba.AdventOfCode.Year2015
{
    public class Day21 : SolutionBase
    {
        private const int MaxCost = 1_000_000;

        private static readonly Gear[] Weapons = [
            (8, 4, 0),
            (10, 5, 0),
            (25, 6, 0),
            (40, 7, 0),
            (74, 8, 0),
        ];

        private static readonly Gear[] Armor = [
            (13, 0, 1),
            (31, 0, 2),
            (53, 0, 3),
            (75, 0, 4),
            (102, 0, 5),
        ];

        private static readonly Gear[] DamageRings = [
            (25, 1, 0),
            (50, 2, 0),
            (100, 3, 0),
        ];

        private static readonly Gear[] DefenseRings = [
            (20, 0, 1),
            (40, 0, 2),
            (80, 0, 3),
        ];

        private static readonly IDictionary<int, (int ring0, int ring1, int ring2)> MinDamageCosts = new Dictionary<int, (int, int, int)>();
        private static readonly IDictionary<int, (int ring0, int ring1, int ring2)> MinDefenseCosts = new Dictionary<int, (int, int, int)>();

        private static readonly IDictionary<int, (int ring0, int ring1, int ring2)> MaxDamageCosts = new Dictionary<int, (int, int, int)>();
        private static readonly IDictionary<int, (int ring0, int ring1, int ring2)> MaxDefenseCosts = new Dictionary<int, (int, int, int)>();

        private readonly double _playerHP = 100;
        private int _bossHP;
        private int _bossDamage;
        private int _bossArmor;

        [Expect("91")]
        protected override string SolvePart1()
        {
            var minCost = MaxCost;
            for (var playerArmor = 0; playerArmor < _bossDamage && playerArmor < 10; playerArmor++)
            {
                if (!MinDefenseCosts.ContainsKey(playerArmor)) continue;

                var playerAttacks = Math.Floor(1 + (_playerHP - 1) / (_bossDamage - playerArmor));
                var playerDamage = (int)Math.Ceiling(_bossHP / playerAttacks + _bossArmor);

                if (!MinDamageCosts.ContainsKey(playerDamage)) continue;

                var damageCost = MinDamageCosts[playerDamage];
                var defenseCost = MinDefenseCosts[playerArmor];

                minCost = Math.Min(minCost, damageCost.ring0 + defenseCost.ring2);
                minCost = Math.Min(minCost, damageCost.ring1 + defenseCost.ring1);
                minCost = Math.Min(minCost, damageCost.ring2 + defenseCost.ring0);
            }

            return $"{minCost}";
        }

        [Expect("158")]
        protected override string SolvePart2()
        {
            var maxCost = 0;
            for (var playerArmor = 0; playerArmor < _bossDamage && playerArmor < 10; playerArmor++)
            {
                if (!MaxDefenseCosts.ContainsKey(playerArmor)) continue;

                var playerAttacks = Math.Ceiling(_playerHP / (_bossDamage - playerArmor));
                var playerDamage = (int)Math.Floor((_bossHP - 1) / playerAttacks + _bossArmor);

                if (!MaxDamageCosts.ContainsKey(playerDamage)) continue;

                var damageCost = MaxDamageCosts[playerDamage];
                var defenseCost = MaxDefenseCosts[playerArmor];

                maxCost = Math.Max(maxCost, damageCost.ring0 + defenseCost.ring2);
                maxCost = Math.Max(maxCost, damageCost.ring1 + defenseCost.ring1);
                maxCost = Math.Max(maxCost, damageCost.ring2 + defenseCost.ring0);
            }

            return $"{maxCost}";
        }

        protected override void TransformData(IEnumerable<string> data)
        {
            var bossStats = data.ToArray();
            _bossHP = Int32.Parse(bossStats[0].Substring("Hit Points: ".Length));
            _bossDamage = Int32.Parse(bossStats[1].Substring("Damage: ".Length));
            _bossArmor = Int32.Parse(bossStats[2].Substring("Armor: ".Length));

            foreach (var weapon in Weapons)
            {
                var cost = weapon.cost;
                var damage = weapon.damage;
                if (!MinDamageCosts.ContainsKey(damage)) MinDamageCosts[damage] = (MaxCost, MaxCost, MaxCost);
                if (!MaxDamageCosts.ContainsKey(damage)) MaxDamageCosts[damage] = (0, 0, 0);
                MinDamageCosts[damage] = (cost, Math.Min(cost, MinDamageCosts[damage].ring1), Math.Min(cost, MinDamageCosts[damage].ring2));
                MaxDamageCosts[damage] = (cost, Math.Max(cost, MaxDamageCosts[damage].ring1), Math.Max(cost, MaxDamageCosts[damage].ring2));

                foreach (var ring1 in DamageRings)
                {
                    cost = weapon.cost + ring1.cost;
                    damage = weapon.damage + ring1.damage;
                    if (!MinDamageCosts.ContainsKey(damage)) MinDamageCosts[damage] = (MaxCost, MaxCost, MaxCost);
                    if (!MaxDamageCosts.ContainsKey(damage)) MaxDamageCosts[damage] = (0, 0, 0);
                    MinDamageCosts[damage] = (MinDamageCosts[damage].ring0, Math.Min(cost, MinDamageCosts[damage].ring2), Math.Min(cost, MinDamageCosts[damage].ring2));
                    MaxDamageCosts[damage] = (MaxDamageCosts[damage].ring0, Math.Max(cost, MaxDamageCosts[damage].ring2), Math.Max(cost, MaxDamageCosts[damage].ring2));

                    foreach (var ring2 in DamageRings)
                    {
                        if (ring1.cost == ring2.cost) continue;

                        cost = weapon.cost + ring1.cost + ring2.cost;
                        damage = weapon.damage + ring1.damage + ring2.damage;
                        if (!MinDamageCosts.ContainsKey(damage)) MinDamageCosts[damage] = (MaxCost, MaxCost, MaxCost);
                        if (!MaxDamageCosts.ContainsKey(damage)) MaxDamageCosts[damage] = (0, 0, 0);
                        MinDamageCosts[damage] = (MinDamageCosts[damage].ring0, MinDamageCosts[damage].ring1, Math.Min(cost, MinDamageCosts[damage].ring2));
                        MaxDamageCosts[damage] = (MaxDamageCosts[damage].ring0, MaxDamageCosts[damage].ring1, Math.Max(cost, MaxDamageCosts[damage].ring2));
                    }
                }
            }

            MinDefenseCosts[0] = (0, 0, 0);
            MaxDefenseCosts[0] = (0, 0, 0);
            foreach (var armor in Armor)
            {
                var cost = armor.cost;
                var defense = armor.defense;
                if (!MinDefenseCosts.ContainsKey(defense)) MinDefenseCosts[defense] = (MaxCost, MaxCost, MaxCost);
                if (!MaxDefenseCosts.ContainsKey(defense)) MaxDefenseCosts[defense] = (0, 0, 0);
                MinDefenseCosts[defense] = (cost, Math.Min(cost, MinDefenseCosts[defense].ring1), Math.Min(cost, MinDefenseCosts[defense].ring2));
                MaxDefenseCosts[defense] = (cost, Math.Max(cost, MaxDefenseCosts[defense].ring1), Math.Max(cost, MaxDefenseCosts[defense].ring2));

                foreach (var ring1 in DefenseRings)
                {
                    cost = armor.cost + ring1.cost;
                    defense = armor.defense + ring1.defense;
                    if (!MinDefenseCosts.ContainsKey(defense)) MinDefenseCosts[defense] = (MaxCost, MaxCost, MaxCost);
                    if (!MaxDefenseCosts.ContainsKey(defense)) MaxDefenseCosts[defense] = (0, 0, 0);
                    MinDefenseCosts[defense] = (MinDefenseCosts[defense].ring0, Math.Min(cost, MinDefenseCosts[defense].ring2), Math.Min(cost, MinDefenseCosts[defense].ring2));
                    MaxDefenseCosts[defense] = (MaxDefenseCosts[defense].ring0, Math.Max(cost, MinDefenseCosts[defense].ring2), Math.Max(cost, MinDefenseCosts[defense].ring2));

                    foreach (var ring2 in DefenseRings)
                    {
                        if (ring1.cost == ring2.cost) continue;

                        cost = armor.cost + ring1.cost + ring2.cost;
                        defense = armor.defense + ring1.defense + ring2.defense;
                        if (!MinDefenseCosts.ContainsKey(defense)) MinDefenseCosts[defense] = (MaxCost, MaxCost, MaxCost);
                        if (!MaxDefenseCosts.ContainsKey(defense)) MaxDefenseCosts[defense] = (0, 0, 0);
                        MinDefenseCosts[defense] = (MinDefenseCosts[defense].ring0, MinDefenseCosts[defense].ring1, Math.Min(cost, MinDefenseCosts[defense].ring2));
                        MaxDefenseCosts[defense] = (MaxDefenseCosts[defense].ring0, MaxDefenseCosts[defense].ring1, Math.Max(cost, MaxDefenseCosts[defense].ring2));
                    }
                }
            }
        }
    }
}
