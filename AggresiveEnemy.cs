using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RogueSharp;

namespace CQuest
{
    public class AggressiveEnemy : GameObject
    {
        private readonly PathToPlayer path;
        private readonly IMap map;
        private bool isAwareOfPlayer;

        public AggressiveEnemy(IMap map, PathToPlayer path)
        {
            this.path = path;
            this.map = map;
        }

        public new void Draw(SpriteBatch spriteBatch)
        {
            float multiplier = Sprite.Width;
            spriteBatch.Draw(Sprite, new Vector2(X * multiplier, Y * multiplier), null, null, null, 0.0f, new Vector2(1, 1), Color.White, SpriteEffects.None, 0.5f);
            path.Draw(spriteBatch);
        }

        public void Update()
        {
            if (isAwareOfPlayer)
            {
                path.CreateFrom(X, Y);
                if (Global.CombatManager.IsPlayerAt(path.FirstCell.X, path.FirstCell.Y))
                {
                    Global.CombatManager.Attack(this,
                    Global.CombatManager.FigureAt(path.FirstCell.X, path.FirstCell.Y));
                }
                else
                {
                    X = path.FirstCell.X;
                    Y = path.FirstCell.Y;
                }
            }
        }
    }
 }

