using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RogueSharp;
using RogueSharp.DiceNotation;

namespace CQuest
{
    public class GameObject
    {
        public int X { get; set; }
        public int Y { get; set; }
        public float Scale { get; set; }
        public Texture2D Sprite { get; set; }

        public int AttackBonus { get; set; }
        public int ArmorClass { get; set; }
        public DiceExpression Damage { get; set; }
        public int Health { get; set; }
        public string Name { get; set; }

        public void Draw(SpriteBatch spriteBatch)
        {
            float multiplier = Sprite.Width;
            spriteBatch.Draw(Sprite, new Vector2(X * multiplier, Y * multiplier), null, null, null, 0.0f, new Vector2(1, 1), Color.White, SpriteEffects.None, 0.5f);
        }
    }
}
