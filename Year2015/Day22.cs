using GameState = (int manaSpent, int bossHP, int playerHP, int playerMana, int shieldTurns, int poisonTurns, int rechargeTurns);

namespace Moyba.AdventOfCode.Year2015
{
    public class Day22 : SolutionBase
    {
        private const int TooMuchMana = 1_000_000;

        private int _bossHP;
        private int _bossDamage;
        private bool _isHardMode = false;

        private HashSet<GameState> _states = new HashSet<GameState>();

        [Expect("900")]
        protected override string SolvePart1()
        {
            _isHardMode = false;

            _states.Clear();
            _states.Add((0, _bossHP, 50, 500, 0, 0, 0));
            while (_states.Any())
            {
                var minManaSpent = _states.Min(_ => _.manaSpent);
                var currentState = _states.First(_ => _.manaSpent == minManaSpent);
                if (currentState.bossHP <= 0) return $"{minManaSpent}";

                _states.Remove(currentState);
                this.OptimizePlayerTurn(currentState);
            }

            throw new Exception("No solution found.");
        }

        [Expect("1216")]
        protected override string SolvePart2()
        {
            _isHardMode = true;

            _states.Clear();
            _states.Add((0, _bossHP, 49, 500, 0, 0, 0));
            while (_states.Any())
            {
                var minManaSpent = _states.Min(_ => _.manaSpent);
                var currentState = _states.First(_ => _.manaSpent == minManaSpent);
                if (currentState.bossHP <= 0) return $"{minManaSpent}";

                _states.Remove(currentState);
                this.OptimizePlayerTurn(currentState);
            }

            throw new Exception("No solution found.");
        }

        protected override void TransformData(IEnumerable<string> data)
        {
            var bossStats = data.ToArray();
            _bossHP = Int32.Parse(bossStats[0].Substring("Hit Points: ".Length));
            _bossDamage = Int32.Parse(bossStats[1].Substring("Damage: ".Length));
        }

        private void OptimizePlayerTurn(GameState state)
        {
            (var manaSpent, var bossHP, var playerHP, var playerMana, var shieldTurns, var poisonTurns, var rechargeTurns) = state;

            // magic missile
            if (playerMana < 53) return;
            this.ApplyBossTurn(manaSpent + 53, bossHP - 4, playerHP, playerMana - 53, shieldTurns, poisonTurns, rechargeTurns);

            // drain
            if (playerMana < 73) return;
            this.ApplyBossTurn(manaSpent + 73, bossHP - 2, playerHP + 2, playerMana - 73, shieldTurns, poisonTurns, rechargeTurns);

            // shield
            if (playerMana < 113) return;
            if (shieldTurns == 0) this.ApplyBossTurn(manaSpent + 113, bossHP, playerHP, playerMana - 113, 6, poisonTurns, rechargeTurns);

            // poison
            if (playerMana < 173) return;
            if (poisonTurns == 0) this.ApplyBossTurn(manaSpent + 173, bossHP, playerHP, playerMana - 173, shieldTurns, 6, rechargeTurns);

            // recharge
            if (playerMana < 229) return;
            if (rechargeTurns == 0) this.ApplyBossTurn(manaSpent + 229, bossHP, playerHP, playerMana - 229, shieldTurns, poisonTurns, 5);
        }

        private void ApplyBossTurn(int manaSpent, int bossHP, int playerHP, int playerMana, int shieldTurns, int poisonTurns, int rechargeTurns)
        {
            if (poisonTurns > 0)
            {
                bossHP -= 3;
                poisonTurns--;
            }

            if (bossHP <= 0) _states.Add((manaSpent, bossHP, playerHP, playerMana, shieldTurns, poisonTurns, rechargeTurns));

            var damage = _bossDamage;
            if (shieldTurns > 0)
            {
                damage = Math.Max(1, damage - 7);
                shieldTurns--;
            }

            playerHP -= damage;
            if (playerHP <= 0) return;

            if (rechargeTurns > 0)
            {
                playerMana += 101;
                rechargeTurns--;
            }

            this.ApplyPlayerTurn(manaSpent, bossHP, playerHP, playerMana, shieldTurns, poisonTurns, rechargeTurns);
        }

        private void ApplyPlayerTurn(int manaSpent, int bossHP, int playerHP, int playerMana, int shieldTurns, int poisonTurns, int rechargeTurns)
        {
            if (_isHardMode)
            {
                playerHP--;
                if (playerHP <= 0) return;
            }

            if (poisonTurns > 0)
            {
                bossHP -= 3;
                poisonTurns--;
            }

            if (shieldTurns > 0) shieldTurns--;

            if (rechargeTurns > 0)
            {
                playerMana += 101;
                rechargeTurns--;
            }

            _states.Add((manaSpent, bossHP, playerHP, playerMana, shieldTurns, poisonTurns, rechargeTurns));
        }
    }
}
