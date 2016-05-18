using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RogueSharp;

namespace CQuest.Content
{
    public class Player : GameObject
    {

        public bool HandleInput(InputState inputState, IMap map)
        {
            // Základní pohyb
            if (inputState.IsLeft(PlayerIndex.One))
            {
                int tempX = X - 1;
                if (map.IsWalkable(tempX, Y))
                {
                    var enemy = Global.CombatManager.EnemyAt(tempX, Y);
                    if (enemy == null)
                    {
                        X = tempX;
                    }
                    else
                    {
                        Global.CombatManager.Attack(this, enemy);
                    }
                    return true;
                }
            }
            else if (inputState.IsRight(PlayerIndex.One))
            {
                int tempX = X + 1;
                if (map.IsWalkable(tempX, Y))
                {
                    var enemy = Global.CombatManager.EnemyAt(tempX, Y);
                    if (enemy == null)
                    {
                        X = tempX;
                    }
                    else
                    {
                        Global.CombatManager.Attack(this, enemy);
                    }
                    return true;
                }
            }
            else if (inputState.IsUp(PlayerIndex.One))
            {
                int tempY = Y - 1;
                if (map.IsWalkable(X, tempY))
                {
                    var enemy = Global.CombatManager.EnemyAt(X, tempY);
                    if (enemy == null)
                    {
                        Y = tempY;
                    }
                    else
                    {
                        Global.CombatManager.Attack(this, enemy);
                    }
                    return true;
                }
            }
            else if (inputState.IsDown(PlayerIndex.One))
            {
                int tempY = Y + 1;
                if (map.IsWalkable(X, tempY))
                {
                    var enemy = Global.CombatManager.EnemyAt(X, tempY);
                    if (enemy == null)
                    {
                        Y = tempY;
                    }
                    else
                    {
                        Global.CombatManager.Attack(this, enemy);
                    }
                    return true;
                }
            }
                return false;
            }
        }
    }
