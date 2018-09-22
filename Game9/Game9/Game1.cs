using Microsoft.Xna.Framework;
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
        Texture2D ball, redPlayer, bluePlayer;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Vector2 ballBegin = new Vector2(screenwidth/2, screenheight/2);
        const float ballAcceleration = 0.002f; //extra speed gained per gametick.
        const short screenwidth = 1200, screenheight = 800, blueStartX = screenwidth - 60, redStartX = 60, 
            playerStartY = screenheight/2, playerLength = 95, playerWidth = 15, ballLength = 15, ballWidth = 15; //bluestartx: x coordinate where the blue player starts. redStartX: x coordinate where the red player starts.
        //playerlength: length of the rectangle either player is controlling. Playerwidth: how wide the rectangle is the player is controlling.
        Ball objball;
        Vector2 objlives;
        Player objBluePlayer, objRedPlayer;
        Random rnd = new Random();
        int scoreBlue = 0, scoreRed = 0;


        public Game1() 
        {
            this.Window.Position = new Point(400, 100);
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferHeight = screenheight;
            graphics.PreferredBackBufferWidth = screenwidth;

            objball = new Ball(1, 0, ballBegin);
            objBluePlayer = new Player(5, blueStartX, playerStartY);
            objRedPlayer = new Player(5, redStartX, playerStartY);
            objlives = new Vector2(0, 0);

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
            objball.Speed = 5;
            objball.Position = ballBegin;
            //objBluePlayer.X = blueStartX;
            //objBluePlayer.Y = playerStartY;
            //objRedPlayer.X = redStartX;
            //objRedPlayer.Y = playerStartY;
            if (rnd.Next(1, 3) == 1)
                objball.Direction = rnd.Next(-75, 75);
            else
                objball.Direction = rnd.Next(105, 255);
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
        public bool IsVectorInRectangle(Vector2 vect, Vector2 topLeft, Vector2 bottomRight)
        {
            if ((vect.X >= topLeft.X && vect.X <= bottomRight.X) && (vect.Y >= topLeft.Y && vect.Y <= bottomRight.Y))
            {
                return true;
            }

            else { return false; }
        }
        public bool IsRectangleInRectangle(Vector2 topLeft1, Vector2 bottomRight1, Vector2 topLeft2, Vector2 bottomRight2)
        {
            Vector2 topRight1 = new Vector2(bottomRight1.X, topLeft1.Y);
            Vector2 bottomLeft1 = new Vector2(topLeft1.X, bottomRight1.Y);
            if (IsVectorInRectangle(topLeft1, topLeft2, bottomRight2) || IsVectorInRectangle(bottomLeft1, topLeft2, bottomRight2) ||
                IsVectorInRectangle(topRight1, topLeft2, bottomRight2) || IsVectorInRectangle(bottomRight1, topLeft2, bottomRight2))
                return true;
            else
                return false;
        }

        protected Vector2 CalculateNewballPos()
        {
            Vector2 pos = objball.Position;
            float x = pos.X;
            float y = pos.Y;
            double dir = objball.Direction * Math.PI/180;
            float speed = objball.Speed;
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
            objball.Position = CalculateNewballPos();
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
            if (y >= screenheight - ballLength || y <= 0)
                objball.Direction = - objball.Direction;

            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (Keyboard.GetState().IsKeyDown(Keys.W))
                if (objRedPlayer.Y - objRedPlayer.Speed > 0)
                    objRedPlayer.Y -= objRedPlayer.Speed;
            
            if (Keyboard.GetState().IsKeyDown(Keys.S))
                if (objRedPlayer.Y + playerLength + objRedPlayer.Speed < screenheight)
                    objRedPlayer.Y += objRedPlayer.Speed;

            if (Keyboard.GetState().IsKeyDown(Keys.Up))
                if (objBluePlayer.Y - objBluePlayer.Speed > 0)
                    objBluePlayer.Y -= objBluePlayer.Speed;

            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                if (objBluePlayer.Y + playerLength + objBluePlayer.Speed < screenheight)
                {
                    objBluePlayer.Y += objBluePlayer.Speed;
                }
            }

            if (IsRectangleInRectangle(CalculateNewballPos(), new Vector2(CalculateNewballPos().X + ballWidth, CalculateNewballPos().Y + ballLength), new Vector2(objBluePlayer.X, objBluePlayer.Y), new Vector2(objBluePlayer.X + playerWidth, objBluePlayer.Y + playerLength))
                || IsRectangleInRectangle(CalculateNewballPos(), new Vector2(CalculateNewballPos().X + ballWidth, CalculateNewballPos().Y + ballLength), new Vector2(objRedPlayer.X, objRedPlayer.Y), new Vector2(objRedPlayer.X + playerWidth, objRedPlayer.Y + playerLength)))
            {
                objball.Direction = 180 - objball.Direction;
                // objball.Direction += rnd.Next(-10, 10); //zorgt er voor dat de bal iets willekeuriger terugkaatst.
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

            switch (scoreBlue)
            {
                case 0:
                    spriteBatch.Draw(ball, objlives, Color.White);
                    spriteBatch.Draw(ball, new Vector2(objlives.X + 15, objlives.Y), Color.White);
                    spriteBatch.Draw(ball, new Vector2(objlives.X + 30, objlives.Y), Color.White);
                    break;
                case 1:
                    spriteBatch.Draw(ball, objlives, Color.White);
                    spriteBatch.Draw(ball, new Vector2(objlives.X + 15, objlives.Y), Color.White);
                    break;
                case 2:
                    spriteBatch.Draw(ball, objlives, Color.White);
                    break;
                case 3:
                    Exit();
                    break;
            }

            switch (scoreRed)
            {
                case 0:
                    spriteBatch.Draw(ball, new Vector2(screenwidth - 15, objlives.Y), Color.White);
                    spriteBatch.Draw(ball, new Vector2(screenwidth - 30, objlives.Y), Color.White);
                    spriteBatch.Draw(ball, new Vector2(screenwidth - 45, objlives.Y), Color.White);
                    break;
                case 1:
                    spriteBatch.Draw(ball, new Vector2(screenwidth - 15, objlives.Y), Color.White);
                    spriteBatch.Draw(ball, new Vector2(screenwidth - 30, objlives.Y), Color.White);
                    break;
                case 2:
                    spriteBatch.Draw(ball, new Vector2(screenwidth - 15, objlives.Y), Color.White);
                    break;
                case 3:
                    Exit();
                    break;
            }

            spriteBatch.Draw(bluePlayer, new Vector2(objBluePlayer.X, objBluePlayer.Y), Color.White);
            spriteBatch.Draw(redPlayer, new Vector2(objRedPlayer.X, objRedPlayer.Y), Color.White);
            spriteBatch.End();

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
