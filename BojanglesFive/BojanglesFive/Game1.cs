using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

//NOTES:
//Viewport = 32 x 18
//Tiles = 80 x 45 + 5 (for UI)
//Tile size = window width / number of tiles horizontally

namespace Roguelike
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    { 
        
        KeyboardState oldState;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        private enum gameState
        {
            menu = 0,
            overworld = 1,
            combat = 2,
        };

        private gameState currentState;

        private Texture2D background;
        private Texture2D wall;
        private Texture2D grate;
        private Texture2D gameOverScreen;
        private Texture2D enemy1;
        private Texture2D hpBar;
        private Texture2D fog;
        private SpriteFont uiText;
        private SpriteFont GOText;

        //private Brain brain;

        private int tileSize;
        private Vector2 spawn;
        private Vector2 cPos;
        private Vector2 vPort = new Vector2(32, 18); //80, 48 = zoomed out, easier to navigate. //32, 18 = zoomed in, more mazelike

        EnemyBase[] enemyList = new EnemyBase[30];

        private int[,] Tiles;
        private int[,] FoW;
        int pDist;

        int score;

        private bool gameOver;
        private bool fMode;

        private Player player;

        private float moveCD;

        private MapGenV2 level1;

        public Game1()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1600; //1600
            graphics.PreferredBackBufferHeight = 900; //1000
            graphics.ApplyChanges();

            Content.RootDirectory = "Content";
        }

        /// <combat variables>
        private EnemyBase cEnemy;
        /// </combat variables>

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            Tiles = new int[Consts.mapWidth, Consts.mapHeight];
            FoW = new int[Consts.mapWidth, Consts.mapHeight];
            pDist = 5;
            tileSize = (graphics.PreferredBackBufferWidth - 250) / (int)vPort.X;
            score = 0;
            fMode = true;
            this.IsMouseVisible = true;
            moveCD = 0;

            //brain = new Brain();

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

            // TODO: use this.Content to load your game content here
            background = Content.Load<Texture2D>("stars"); // change these names to the names of your images
            wall = Content.Load<Texture2D>("wall");
            grate = Content.Load<Texture2D>("grate");
            gameOverScreen = Content.Load<Texture2D>("GameOver");
            enemy1 = Content.Load<Texture2D>("RedEnemy");
            hpBar = Content.Load<Texture2D>("healthblob");
            fog = Content.Load<Texture2D>("FoW grey");
            uiText = Content.Load<SpriteFont>("font");
            GOText = Content.Load<SpriteFont>("bigfont");
            currentState = gameState.overworld;

            level1 = new MapGenV2();
            Tiles = level1.map();
            enemyList = level1.getEnemies();
            spawn = level1.getSpawn();

            player = new Player((int)spawn.X * 10, (int)spawn.Y * 10);
            cPos = new Vector2(spawn.X - (vPort.X/2), spawn.Y - (vPort.Y/2));
            player.setExit(level1.getExit());

            clearFoW();

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (currentState == gameState.overworld)
            {
                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                    Exit();

                // TODO: Add your update logic here
                OUInput(gameTime);
                player.Update(gameTime);

                base.Update(gameTime);

                if (player.isCleared())
                {
                    Tiles = level1.map();
                    player.newLevel();
                    player.jumpRegardless(level1.getSpawn());
                    player.setExit(level1.getExit());
                    cPos = new Vector2(spawn.X - (vPort.X / 2), spawn.Y - (vPort.Y / 2) + 1);
                    enemyList = level1.getEnemies();
                    FoW = new int[Consts.mapWidth, Consts.mapHeight];
                    score += 100;
                }

                if (!player.isMove)
                {
                    for (int i = 0; i < enemyList.Length; i++)
                    {
                        if (enemyList[i] != null)
                        {
                            if (player.TargetPosition / 10 == enemyList[i].GridPosition)
                            {
                                if (enemyList[i].hit(player.Dmg) <= 0)
                                {
                                    score += (enemyList[i].SV);
                                    enemyList[i] = enemyDeath(enemyList[i]);
                                    continue;
                                }
                            }


                            //get next tile from brain.
                            //brain.Build(Tiles, enemyList[i], player);

                            int numAtks = enemyList[i].doMonsterTurn(player, Tiles);

                            if (numAtks != 0)
                            {
                                //replace with "go to combat screen for that enemy and player".
                                if (player.hit(enemyList[i].Atk * numAtks) <= 0)
                                {
                                    gameOver = true;
                                }
                            }
                        }
                    }
                    player.isMove = true;
                }

                clearFoW();
            }
            else if (currentState == gameState.combat)
            {
                //COMBAT UPDATE JUMPTO
            }
        }

        private void OUInput(GameTime gameTime)  //Input handler for overworld.
        {
            KeyboardState newState = Keyboard.GetState();

            bool movingDown = false;
            if (newState.IsKeyDown(Keys.W) || newState.IsKeyDown(Keys.A) || newState.IsKeyDown(Keys.S) || newState.IsKeyDown(Keys.D))
            {
                if (moveCD <= 0)
                    movingDown = true;
                else
                    moveCD -= gameTime.ElapsedGameTime.Milliseconds;
            }
            else
                moveCD = 0;

            if (newState != oldState || movingDown)
            {

                bool moved = false;
                Vector2 pGP = new Vector2(player.GridPosition.X / 10, player.GridPosition.Y / 10);

                if (newState.IsKeyDown(Keys.W))
                {
                    pGP.Y -= 1;
                    moveCD = 120;
                    moved = true;
                }
                else if (newState.IsKeyDown(Keys.A))
                {
                    pGP.X -= 1;
                    moveCD = 120;
                    moved = true;
                }
                else if (newState.IsKeyDown(Keys.S))
                {
                    pGP.Y += 1;
                    moveCD = 120;
                    moved = true;
                }
                else if (newState.IsKeyDown(Keys.D))
                {
                    pGP.X += 1;
                    moveCD = 120;
                    moved = true;
                }

                if (newState.IsKeyDown(Keys.F3))
                    fMode = !fMode;

                if (newState.IsKeyDown(Keys.F2)) //DEBUG ONLY, REMOVE FOR RELEASE.
                {

                    Tiles = level1.map();
                    Vector2 spawn = level1.getSpawn();
                    player.jumpRegardless(level1.getSpawn());
                    player.setExit(level1.getExit());
                    player.respawn();
                    cPos = new Vector2(spawn.X - (vPort.X / 2), spawn.Y - (vPort.Y / 2) + 1);
                    FoW = new int[Consts.mapWidth, Consts.mapHeight];
                    score -= 100;
                    if (score < 0) score = 0;
                }

                if (newState.IsKeyDown(Keys.R))
                {
                    if (gameOver)
                    {
                        gameOver = false;

                        Tiles = level1.map();

                        Vector2 spawn = level1.getSpawn();
                        player.jumpRegardless(level1.getSpawn());
                        player.setExit(level1.getExit());
                        player.respawn();
                        cPos = new Vector2(spawn.X - (vPort.X / 2), spawn.Y - (vPort.Y / 2) + 1);
                        FoW = new int[Consts.mapWidth, Consts.mapHeight];
                        score = 0;
                    }
                }

                if (moved)
                {
                    Vector2 temp = player.GridPosition;
                    player.jump((int)pGP.X, (int)pGP.Y, Tiles[(int)pGP.X, (int)pGP.Y]);
                    cPos -= ((temp - player.GridPosition) / 10);
                    
                    foreach (EnemyBase enemy in enemyList)
                    {
                        if (enemy != null && Tiles[(int)pGP.X, (int)pGP.Y] != 1) enemy.AddAP(100);
                    }
                }

                // Update saved state.
                oldState = newState;
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here
            spriteBatch.Begin();

            if (gameOver)
            {
                spriteBatch.Draw(gameOverScreen, new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight), Color.White);
                spriteBatch.DrawString(GOText, "Score: " + score, new Vector2(graphics.PreferredBackBufferWidth * 4.5f/10, graphics.PreferredBackBufferHeight * 6.5f / 10), Color.Red);
            }
            else if (currentState == gameState.overworld)
            {
                //fill viewport with "empty" colour.
                spriteBatch.Draw(wall, new Rectangle(0, 0, graphics.PreferredBackBufferWidth, tileSize * (int)(vPort.Y-1)), Color.SeaGreen);

                //draw the tiles the player can see.
                for (int i = 0; i < (int)vPort.X; i++)
                {
                    for (int j = 0; j < (int)vPort.Y - 1; j++)
                    {
                        int iC = (int)(i + cPos.X );
                        int jC = (int)(j + cPos.Y );

                        if (iC < 0  || iC >= Consts.mapWidth|| jC < 0 || jC >= Consts.mapHeight)
                        {
                            spriteBatch.Draw(wall, new Rectangle(i * tileSize, j * tileSize, tileSize, tileSize), Color.SlateGray);
                        }
                        else if (FoW[iC, jC] == 1 || FoW[iC, jC] == -1 || !fMode)
                        {
                            switch (Tiles[iC, jC])
                            {
                                case -1:
                                    spriteBatch.Draw(wall, new Rectangle(i * tileSize, j * tileSize, tileSize, tileSize), Color.SlateGray);
                                    break;

                                case 1:
                                case 2:
                                    spriteBatch.Draw(wall, new Rectangle(i * tileSize, j * tileSize, tileSize, tileSize), Color.SandyBrown);
                                    break;
                                                                    
                                case 3:
                                    spriteBatch.Draw(grate, new Rectangle(i * tileSize, j * tileSize, tileSize, tileSize), Color.White);
                                    break;

                                case 11:
                                    spriteBatch.Draw(enemy1, new Rectangle(i * tileSize, j * tileSize, tileSize, tileSize), Color.White);
                                    break;

                                case 12:
                                    spriteBatch.Draw(wall, new Rectangle(i * tileSize, j * tileSize, tileSize, tileSize), Color.Yellow);
                                    break;

                                case 13:
                                    spriteBatch.Draw(wall, new Rectangle(i * tileSize, j * tileSize, tileSize, tileSize), Color.Green);
                                    break;
                            }

                            for (int e = 0; e < enemyList.Length; e++)
                            {
                                if (enemyList[e] == null)
                                    continue;
                                if (enemyList[e].GridPosition == new Vector2(iC, jC))
                                {
                                    spriteBatch.Draw(hpBar, new Rectangle((i + (1 / 40)) * tileSize, (j + (1 / 40)) * tileSize, (int)(tileSize * 38 / 40 * enemyList[e].HP), tileSize * 8 / 40), Color.White);
                                    break;
                                }
                            }
                        }
                        //fow: 0 = unseen, 1 = fog, 2, visible
                        else if (FoW[iC, jC] == 2 && fMode)
                        {
                            if (Tiles[iC, jC] != -1)
                            {
                                switch (Tiles[iC, jC])
                                {
                                    case 1:
                                    case 2:
                                        spriteBatch.Draw(wall, new Rectangle(i * tileSize, j * tileSize, tileSize, tileSize), Color.SandyBrown);
                                        break;

                                    case 3:
                                        spriteBatch.Draw(grate, new Rectangle(i * tileSize, j * tileSize, tileSize, tileSize), Color.White);
                                        break;
                                }

                                spriteBatch.Draw(fog, new Rectangle(i * tileSize, j * tileSize, tileSize, tileSize), Color.White);
                            }
                            else spriteBatch.Draw(wall, new Rectangle(i * tileSize, j * tileSize, tileSize, tileSize), Color.SlateGray);
                        }
                        else if (fMode) spriteBatch.Draw(wall, new Rectangle(i * tileSize, j * tileSize, tileSize, tileSize), Color.SlateGray);                        
                    }
                }

                spriteBatch.Draw(wall, new Rectangle(1340,0,260,260), Color.White);

                for (int i = 0; i < Consts.mapWidth; i++)
                {
                    for (int j = 0; j < Consts.mapHeight; j++)
                    {
                        if (FoW[i, j] != 0 || !fMode)
                        {
                            switch (Tiles[i, j])
                            {
                                case 1:
                                    spriteBatch.Draw(wall, new Rectangle(1342 + (2 * i), 2 * (j + 1), 2, 2), Color.Black);
                                    break;
                                case 2:
                                    spriteBatch.Draw(wall, new Rectangle(1342 + (2 * i), 2 * (j+1), 2, 2), Color.White);
                                    break;

                                case 3:
                                    spriteBatch.Draw(wall, new Rectangle(1342 + (2 * i), 2 * (j+1), 2, 2), Color.Green);
                                    break;

                                case 11:
                                case 12:
                                case 13:
                                    if(FoW[i,j] == 1)
                                        spriteBatch.Draw(enemy1, new Rectangle(1342 + (2 * i), 2 * (j + 1), 2, 2), Color.Red);
                                    break;
                            }
                        }
                    }
                }

                spriteBatch.Draw(wall, new Rectangle((int)(player.ScreenPosition.X / 10 - cPos.X) * tileSize, (int)(player.ScreenPosition.Y / 10 - cPos.Y) * tileSize, tileSize, tileSize), Color.White);
                spriteBatch.Draw(wall, new Rectangle(1342 + (int)(player.ScreenPosition.X / 5), (int)(player.ScreenPosition.Y / 5), 2, 2), Color.White);
                //                spriteBatch.Draw(wall, new Rectangle(780, 480, tileSize, tileSize), Color.White);


                spriteBatch.DrawString(uiText, "Health: " + player.Health, new Vector2(tileSize / 4, graphics.PreferredBackBufferHeight * 0.875f), Color.White);
                spriteBatch.DrawString(uiText, "Score: " + score, new Vector2(tileSize / 4, graphics.PreferredBackBufferHeight * 0.925f), Color.White);
            }
            else if (currentState == gameState.combat)
            {
                //COMBAT DRAW JUMPTO
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        void getSubTiles(Layouts layout)
        {

            int[,] subTile = new int[30, 30];

            subTile = layout.getLayout();

            for (int i = 0; i < layout.getWidth(); i++)
            {
                for (int j = 0; j < layout.getHeight(); j++)
                {
                    int left = layout.getLeft();
                    int top = layout.getTop();
                    Tiles[i + left, j + top] = subTile[i, j];
                }
            }
        }

        public int getTile(int x, int y)
        {
            return Tiles[x, y];
        }

        private EnemyBase enemyDeath(EnemyBase enemy)
        {
            Vector2 temp = enemy.GridPosition;
            Tiles[(int)temp.X, (int)temp.Y] = 0;
            enemy = null;
            return null;
        }

        void clearFoW()
        {
            Vector2 temp = new Vector2(player.GridPosition.X / 10, player.GridPosition.Y / 10);

            for (int i = 0; i < Consts.mapWidth; i++)
            {
                for (int j = 0; j < Consts.mapHeight; j++)
                {
                    if (FoW[i, j] == 1 && Math.Abs(temp.X - i) + Math.Abs(temp.Y - j) > pDist) FoW[i, j] = 2;
                    else if ((FoW[i, j] == 0 || FoW[i, j] == 2) && Math.Abs(temp.X - i) + Math.Abs(temp.Y - j) <= pDist)
                    {
                        FoW[i, j] = 1;
                    }
                }
            }

            for (int i = (0 - Consts.spawnWidth / 2); i < (Consts.spawnWidth / 2); i++)
            {
                for (int j = (0 - Consts.spawnHeight / 2); j < (Consts.spawnHeight / 2) + 1; j++)
                {
                    FoW[(int)spawn.X + i, (int)spawn.Y + j] = -1;
                }
            }

            //int temp = (int)(Math.Abs(gridPosition.X - (target.GridPosition.X / 10)) + Math.Abs(gridPosition.Y - (target.GridPosition.Y / 10)));
            //if(temp < pDist
        }

    }
}
