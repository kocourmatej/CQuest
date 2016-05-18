using CQuest.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RogueSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CQuest
{
    public class PathToPlayer
    {
        private readonly Player player;
        private readonly IMap map;
        private readonly Texture2D sprite;
        private readonly PathFinder pathFinder;
        private IEnumerable<Cell> cells;

        public PathToPlayer(Player player, IMap map, Texture2D sprite)
        {
            this.player = player;
            this.map = map;
            this.sprite = sprite;
            pathFinder = new PathFinder(map);
        }

        public Cell FirstCell
        {
            get
            {
                return cells.First();
            }
        }

        public void CreateFrom(int x, int y)
        {
            cells = pathFinder.ShortestPath(map.GetCell(x, y), map.GetCell(player.X, player.Y)).Steps;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (cells != null && Global.GameState == Global.GameStates.Debugging)
            {
                foreach (Cell cell in cells)
                {
                    if (cell != null)
                    {
                        float scale = .25f;
                        float multiplier = sprite.Width;
                        spriteBatch.Draw(sprite, new Vector2(cell.X * multiplier, cell.Y * multiplier), null, null, null, 0.0f, new Vector2(1, 1), Color.Blue * .2f, SpriteEffects.None, 0.6f);
                    }
                }
            }
        }
    }
}
