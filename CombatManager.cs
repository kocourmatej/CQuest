using CQuest.Content;
using RogueSharp.DiceNotation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace CQuest
{
    public class CombatManager
    {
        private readonly Player player;
        private readonly List<AggressiveEnemy> aggressiveEnemies;

        public CombatManager(Player player, List<AggressiveEnemy> aggressiveEnemies)
        {
            this.player = player;
            this.aggressiveEnemies = aggressiveEnemies;
        }


        public void Attack(GameObject attacker, GameObject defender)
        {
            var attackDie = Dice.Parse("d20");
            if (attackDie.Roll(Global.Random).Value + attacker.AttackBonus >= defender.ArmorClass)
            {
               
                int damage = attacker.Damage.Roll().Value;
                defender.Health -= damage;
                Debug.WriteLine("{0} hit {1} for {2} and he has {3} health remaining.",
                  attacker.Name, defender.Name, damage, defender.Health);
                if (defender.Health <= 0)
                {
                    if (defender is AggressiveEnemy)
                    {
                        var enemy = defender as AggressiveEnemy;
                        aggressiveEnemies.Remove(enemy);
                    }
                    Debug.WriteLine("{0} killed {1}", attacker.Name, defender.Name);
                }
            }
            else
            {
                Debug.WriteLine("{0} missed {1}", attacker.Name, defender.Name);
            }
        }

        public GameObject FigureAt(int x, int y)
        {
            if (IsPlayerAt(x, y))
            {
                return player;
            }
            return EnemyAt(x, y);
        }

        public bool IsPlayerAt(int x, int y)
        {
            return (player.X == x && player.Y == y);
        }

        public AggressiveEnemy EnemyAt(int x, int y)
        {
            foreach (var enemy in aggressiveEnemies)
            {
                if (enemy.X == x && enemy.Y == y)
                {
                    return enemy;
                }
            }
            return null;
        }

        public bool IsEnemyAt(int x, int y)
        {
            return EnemyAt(x, y) != null;
        }
    }
}
