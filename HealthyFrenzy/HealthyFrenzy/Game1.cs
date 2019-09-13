//
// Jose Fernandes
// 01/06/2017
//

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using System.Diagnostics;

namespace HealthyFrenzy
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;


        public static Random random;
        public static Rectangle screenBounds;
        

        Player player;

        KeyboardState currentKeyboardState;
        
        Texture2D[] foodTextures;
        Texture2D foodtexture;
        List<FoodItem> foodItemList;

        TimeSpan dropFoodTime;
        TimeSpan previousDropFoodTime;
        TimeSpan scoreTime;
        TimeSpan previousScoreTime;

        SpriteFont mainFont;
        int healthbarWidth = 400;
        int healthbarHeigth;
        Rectangle healthbarRec;
        Vector2 healthbarPos;
        Texture2D healthbarTxr;

        Vector2 scorePos;
        Vector2 gameOverScorePos;

        public static int HealthLevel = 100;
        public static int score = 0;
        int lives = 3;

        const int MAX_FOODS = 10;
        
        // background music
        Song backgroundMusic;

        // Sound effect
        public static SoundEffect LevelUpSoundEffect;
        public static SoundEffect LevelDownSoundEffect;
        SoundEffect newLifeSoundEffect;
        public static SoundEffect GameOverSoundEffect;
        public static SoundEffect HopEffect;


        private enum GameStates
        {
            GameOver,
            Playing,
            Quit,
            Title
        };

        GameStates currentGameState;

        Texture2D playingBackground;
        Texture2D GameOverScreen;
        Texture2D QuitScreen;
        Texture2D TitleScreen;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            //title game window
            this.Window.Title = "Healthy Frenzy - Jose Fernandes";
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 400;
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

            dropFoodTime = TimeSpan.FromSeconds(1.0f);
            previousDropFoodTime = TimeSpan.Zero;

            scoreTime = TimeSpan.FromSeconds(5.0f);
            previousScoreTime = TimeSpan.Zero;

            random = new Random();
            
            screenBounds = GraphicsDevice.Viewport.Bounds;

            foodItemList = new List<FoodItem>();

            currentGameState = GameStates.Title;

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

            GameOverScreen = Content.Load<Texture2D>("backgroundGameOver");
            QuitScreen = Content.Load<Texture2D>("backgroundQuit");
            TitleScreen = Content.Load<Texture2D>("backgroundTitle");



            
            playingBackground = Content.Load<Texture2D>("background");

            player = new Player(Content.Load<Texture2D>("shopcart"));

            foodTextures = new Texture2D[MAX_FOODS];
            foodTextures[0] = Content.Load<Texture2D>("fries");
            foodTextures[1] = Content.Load<Texture2D>("icecream");
            foodTextures[2] = Content.Load<Texture2D>("donut");
            foodTextures[3] = Content.Load<Texture2D>("burger");
            foodTextures[4] = Content.Load<Texture2D>("orange");
            foodTextures[5] = Content.Load<Texture2D>("apple");
            foodTextures[6] = Content.Load<Texture2D>("avocado");
            foodTextures[7] = Content.Load<Texture2D>("pear");
            foodTextures[8] = Content.Load<Texture2D>("drink");
            foodTextures[9] = Content.Load<Texture2D>("bottlewater"); // last

            mainFont = Content.Load<SpriteFont>("mainFont");

            healthbarTxr = Content.Load<Texture2D>("healthBar");
            healthbarHeigth = (int)mainFont.MeasureString("D").Y;
            healthbarTxr = Content.Load<Texture2D>("healthBar");
            healthbarRec = new Rectangle(screenBounds.Width-410, 20, healthbarWidth, healthbarHeigth);
            healthbarPos = new Vector2(healthbarRec.X + 10, healthbarRec.Y);
            scorePos = new Vector2(10, 17);
            gameOverScorePos = new Vector2(325, 100);


            // Background music
            backgroundMusic = Content.Load<Song>("backgroundMusic");
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(backgroundMusic);
            MediaPlayer.Volume = 0.2f;

            // sound effects
            LevelUpSoundEffect = Content.Load<SoundEffect>("LevelUpSound-effect");
            LevelDownSoundEffect = Content.Load<SoundEffect>("failSound");
            GameOverSoundEffect = Content.Load<SoundEffect>("gameOver");
            HopEffect = Content.Load<SoundEffect>("Hop");
            newLifeSoundEffect = Content.Load<SoundEffect>("newLife");

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
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here

            currentKeyboardState = Keyboard.GetState();

            switch (currentGameState)
            {
                case GameStates.GameOver:
                    UpdateGameOver();
                    break;
                case GameStates.Playing:
                    UpdatePlaying(gameTime);
                    break;
                case GameStates.Quit:
                    UpdateQuit();
                    break;
                case GameStates.Title:
                    UpdateTitle();
                    break;
            }


            // playing state



            base.Update(gameTime);
        }

        private void UpdateTitle()
        {
            if (currentKeyboardState.IsKeyDown(Keys.Space))
            {
                StartGame();
            }
            if (currentKeyboardState.IsKeyDown(Keys.Q))
            {
                currentGameState = GameStates.Quit;
            }
        }

        private void StartGame()
        {
            score = 0;
            lives = 3;
            HealthLevel = 100;
            currentGameState = GameStates.Playing;
        }

        private void UpdateQuit()
        {
            if (currentKeyboardState.IsKeyDown(Keys.N))
            {
                currentGameState = GameStates.Title;
            }
            if (currentKeyboardState.IsKeyDown(Keys.Y))
            {
                Exit();
            }
        }

        private void UpdateGameOver()
        {
            
            if (currentKeyboardState.IsKeyDown(Keys.Space))
            {
                StartGame();
            }
            if (currentKeyboardState.IsKeyDown(Keys.Q))
            {
                currentGameState = GameStates.Quit;
            }
        }

        private void UpdatePlaying(GameTime gt)
        {
            player.Update();
            UpdateFoodItem(gt);
            UpdateHealthScore(gt);
        }

        private void UpdateHealthScore(GameTime gt)
        {
            if (gt.TotalGameTime - previousScoreTime > scoreTime)
            {
                previousScoreTime = gt.TotalGameTime;
                score += 10;
                HealthLevel -= 10;
            }
            if (HealthLevel > 100)
            {
                HealthLevel = 100;
            }
            if (HealthLevel < 0)
            {
                HealthLevel = 0;
            }
            healthbarRec.Width = healthbarWidth * HealthLevel / 100;

            if (HealthLevel == 0)
            {
                lives -= 1;
                if (lives > 0)
                {
                    newLifeSoundEffect.Play();
                }
                HealthLevel = 100;
            }

            if (lives == 0)
            {
                GameOverSoundEffect.Play();
                currentGameState = GameStates.GameOver;
            }
        }

        private void UpdateFoodItem(GameTime gt)
        {
            if (gt.TotalGameTime - previousDropFoodTime > dropFoodTime)
            {
                previousDropFoodTime = gt.TotalGameTime;
                addFoodItem();
            }

            foreach(FoodItem f in foodItemList)
            {
                f.Update();
                f.wasCaught(player,f);
            }

            foodItemList.RemoveAll(f => f.isActive == false);
        }

        private void addFoodItem()
        {
            int r = random.Next(0, MAX_FOODS-1);
            int value = Convert.ToInt32(HealthLevel.ToString());
            double newRnd = random.NextDouble();
            if(newRnd>=0.93) // 7% 
            {
                r = MAX_FOODS-1;
            }
            int posX = random.Next(24, Game1.screenBounds.Width - 47);
            switch (r)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                    value = -25;
                    break;
                case 4:
                case 5:
                case 6:
                case 7:
                    value = 5;
                    break;
                case 8:
                    value = -50;
                    break;
                case 9:
                    value = 20;
                    break;
            }
            foodtexture = foodTextures[r];
            FoodItem foodItem = new FoodItem(foodtexture, posX, value);
            foodItemList.Add(foodItem);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            // initializes sprite batch
            spriteBatch.Begin();


            switch (currentGameState)
            {
                case GameStates.Playing:
                    DrawPlaying();
                    break;
                case GameStates.GameOver:
                    DrawGameOver();
                    break;
                case GameStates.Quit:
                    DrawQuit();
                    break;
                case GameStates.Title:
                    DrawTitle();
                    break;
            }



            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawTitle()
        {
            spriteBatch.Draw(TitleScreen, Vector2.Zero, Color.White);
        }

        private void DrawQuit()
        {
            spriteBatch.Draw(QuitScreen, Vector2.Zero, Color.White);
        }

        private void DrawGameOver()
        {
            spriteBatch.Draw(GameOverScreen, Vector2.Zero, Color.White);
            spriteBatch.DrawString(mainFont, "Score:  " + score, gameOverScorePos, Color.Black);
        }

        private void DrawPlaying()
        {
            spriteBatch.Draw(playingBackground, Vector2.Zero, Color.White);
            foreach (FoodItem f in foodItemList)
            {
                f.Draw(spriteBatch);
            }


            if(HealthLevel>50)
            {
                spriteBatch.Draw(healthbarTxr, healthbarRec, Color.Green);
            }
            else if(HealthLevel>10)
            {
                spriteBatch.Draw(healthbarTxr, healthbarRec, Color.Orange);
            }
            else
            {
                spriteBatch.Draw(healthbarTxr, healthbarRec, Color.Red);
            }
            spriteBatch.DrawString(mainFont, HealthLevel.ToString() + "%", healthbarPos, Color.Black);
            spriteBatch.DrawString(mainFont, "Score:  " + score + "     Lives:  " + lives, scorePos, Color.Black);
            player.Draw(spriteBatch);
        }
    }
}
