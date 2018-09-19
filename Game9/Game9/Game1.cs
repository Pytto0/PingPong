﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Game9
{

    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        Texture2D ball;
        Texture2D redPlayer;
        Texture2D bluePlayer;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Vector2 ballBegin = new Vector2(screenwidth/2, screenheight/2);
        const double ballAcceleration = 0; //extra speed gained per gametick.
        const int screenwidth = 1200;
        const int screenheight = 800;
        Ball objball;
        int scoreBlue = 0;
        int scoreRed = 0;
        

        public Game1() 
        {
            this.Window.Position = new Point(600, 0);
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferHeight = screenheight;
            graphics.PreferredBackBufferWidth = screenwidth;
            objball = new Ball(1, 0, ballBegin);
            ResetGame();
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

            base.Initialize();
        }
        
        protected void ResetGame()
        {
            Random rnd = new Random();
            objball.Speed = 5;
            objball.Position = ballBegin;
            if (rnd.Next(1, 3) == 1)
                objball.Direction = rnd.Next(-45, 45);
            else
                objball.Direction = rnd.Next(135, 225);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            ball = Content.Load<Texture2D>("Graphics/bal");
            bluePlayer = Content.Load<Texture2D>("Graphics/blauweSpeler");
            redPlayer = Content.Load<Texture2D>("Graphics/rodeSpeler");
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }
        private Vector2 calculateNewballPos()
        {
            Vector2 pos = objball.Position;
            float x = pos.X;
            float y = pos.Y;
            double dir = objball.Direction * Math.PI/180;
            double speed = objball.Speed;
            x = (float) (x + Math.Cos(dir) * speed);
            y = (float) (y + Math.Sin(dir) * speed);
            return new Vector2(x, y);
        }
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            objball.Position = calculateNewballPos();
            objball.Speed += ballAcceleration;
            float x = objball.Position.X;
            float y = objball.Position.Y;
            if (x > screenwidth)
            {
                scoreRed++;
                ResetGame();
            }
            if (x < 0)
            {
                scoreBlue++;
                ResetGame();
            }
            if (y >= screenheight)
            {
               
            }
            if (y <= 0)
            {
                
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }
            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                Exit();
            }
            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                Exit();
            }
            // TEST
            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);
            spriteBatch.Begin();
            spriteBatch.Draw(ball, objball.Position, Color.White);
            spriteBatch.Draw(bluePlayer, new Vector2(60,screenheight/2), Color.White);
            spriteBatch.Draw(redPlayer, new Vector2(screenwidth-60, screenheight/2), Color.White);
            spriteBatch.End();

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
