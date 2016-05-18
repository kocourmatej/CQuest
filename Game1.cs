using CQuest.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RogueSharp;
using RogueSharp.DiceNotation;
using RogueSharp.MapCreation;
using RogueSharp.Random;
using System.Collections.Generic;

namespace CQuest
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // Zdroje [TODO - REFACTOR]
        Texture2D sprFloor;
        Texture2D sprWall;

        // Mapa
        IMap map;

        // Objekty
        Player player;
        List<AggressiveEnemy> aggressiveEnemies = new List<AggressiveEnemy>();

        // Vstup
        InputState inputState;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        private void AddAggressiveEnemies(int numberOfEnemies)
        {
            for (int i = 0; i < numberOfEnemies; i++)
            {
                Cell enemyCell = GetRandomEmptyCell();
                var pathFromAggressiveEnemy = new PathToPlayer(player, map, Content.Load<Texture2D>("White"));
                pathFromAggressiveEnemy.CreateFrom(enemyCell.X, enemyCell.Y);
                var enemy = new AggressiveEnemy(map, pathFromAggressiveEnemy)
                {
                    X = enemyCell.X,
                    Y = enemyCell.Y,
                    Sprite = Content.Load<Texture2D>("Hound"),
                    ArmorClass = 10,
                    AttackBonus = 0,
                    Damage = Dice.Parse("d3"),
                    Health = 10,
                    Name = "Hunting Hound"
                };
                // Add each enemy to list of enemies
                aggressiveEnemies.Add(enemy);
            }
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            IMapCreationStrategy<Map> mapCreationStrategy = new RandomRoomsMapCreationStrategy<Map>(50, 30, 10, 7, 3);
            map = Map.Create(mapCreationStrategy);
            inputState = new InputState();
            Global.GameState = Global.GameStates.PlayerTurn;

            Global.Camera.ViewportWidth = graphics.GraphicsDevice.Viewport.Width;
            Global.Camera.ViewportHeight = graphics.GraphicsDevice.Viewport.Height;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            sprFloor = Content.Load<Texture2D>("floor");
            sprWall = Content.Load<Texture2D>("wall");

            // TODO: use this.Content to load your game content here
            Cell startingCell = GetRandomEmptyCell();

            // Hráč
            player = new Player
            {
                X = startingCell.X,
                Y = startingCell.Y,
                Sprite = Content.Load<Texture2D>("Player"),
                ArmorClass = 15,
                AttackBonus = 1,
                Damage = Dice.Parse("2d4"),
                Health = 50,
                Name = "Lord of Flies"
            };

            // Nepřátelé
            startingCell = GetRandomEmptyCell();
            AddAggressiveEnemies(10);
            UpdatePlayerFOV();
            Global.Camera.CenterOn(startingCell);

            Global.CombatManager = new CombatManager(player, aggressiveEnemies);

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        private Cell GetRandomEmptyCell()
        {
            IRandom random = new DotNetRandom();

            while (true)
            {
                int x = Global.Random.Next(49);
                int y = Global.Random.Next(29);
                if (map.IsWalkable(x, y))
                {
                    return map.GetCell(x, y);
                }
            }
        }

        private void UpdatePlayerFOV()
        {
            map.ComputeFov(player.X, player.Y, 30, true);

            foreach(Cell cell in map.GetAllCells())
            {
                if (map.IsInFov(cell.X, cell.Y))
                {
                    map.SetCellProperties(cell.X, cell.Y, cell.IsTransparent, cell.IsWalkable, true);
                }
            }
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (inputState.IsExitGame(PlayerIndex.One))
            {
                Exit();
            }
            else if (inputState.IsSpace(PlayerIndex.One)) // Debug
            {
                if (Global.GameState == Global.GameStates.PlayerTurn)
                {
                    Global.GameState = Global.GameStates.Debugging;
                }
                else if (Global.GameState == Global.GameStates.Debugging)
                {
                    Global.GameState = Global.GameStates.PlayerTurn;
                }
            }

            if (Global.GameState == Global.GameStates.PlayerTurn && player.HandleInput(inputState, map))
            {
                UpdatePlayerFOV();
                Global.Camera.CenterOn(map.GetCell(player.X, player.Y));
                Global.GameState = Global.GameStates.EnemyTurn;
            }
            if (Global.GameState == Global.GameStates.EnemyTurn)
            {
                foreach (var enemy in aggressiveEnemies)
                {
                    enemy.Update();
                }
                Global.GameState = Global.GameStates.PlayerTurn;
            }


            // TODO: Add your update logic here
            inputState.Update();
            Global.Camera.HandleInput(inputState, PlayerIndex.One);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            int cellSize = 64;
            float cellScale = .25f;

            // TODO: Add your drawing code here
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, null, null, null, null, Global.Camera.TranslationMatrix);


            // Layer - Tiles
            foreach (Cell cell in map.GetAllCells())
            {
                Vector2 position = new Vector2(cell.X * cellSize, cell.Y * cellSize);
                Color color = Color.White;

                if (!cell.IsExplored && Global.GameState != Global.GameStates.Debugging)
                {
                    continue;
                }

                if (!cell.IsInFov && Global.GameState != Global.GameStates.Debugging)
                {
                    color = Color.Gray;
                }

                // Průchozí pole [TODO - Entity image]
                if (cell.IsWalkable)
                {                  
                    spriteBatch.Draw(sprFloor, position, null, null, null, 0.0f, new Vector2(1, 1), color, SpriteEffects.None, 0.8f);
                }
                else
                {
                    spriteBatch.Draw(sprWall, position, null, null, null, 0.0f, new Vector2(1, 1), color, SpriteEffects.None, 0.8f);
                }
            }

            // Layer - Objects
            player.Draw(spriteBatch);


            foreach (var enemy in aggressiveEnemies)
            {
                if (Global.GameState == Global.GameStates.Debugging || map.IsInFov(enemy.X, enemy.Y))
                {
                    enemy.Draw(spriteBatch);
                }
            }

            // Layer - Effects

            // Layer - GUI

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
