using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
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
        SoundEffect wall, miss, paddle;
        Vector2 ballBegin = new Vector2(screenwidth/2, screenheight/2);
        const float ballAcceleration = 0.005f; //extra speed gained per gametick.
        const short screenwidth = 1200, screenheight = 800, blueStartX = screenwidth - 60, redStartX = 60, 
            playerStartY = screenheight/2, playerLength = 95, playerWidth = 15, ballLength = 15, ballWidth = 15;
        //bluestartx: x coordinate where the blue player starts. redStartX: x coordinate where the red player starts.
        //playerlength: length of the rectangle either player is controlling. Playerwidth: how wide the rectangle is the player is controlling.
        Ball objball;
        Vector2 objlives;
        Player objBluePlayer, objRedPlayer;
        SpriteFont font;
        short redLives = 3, blueLives = 3;

        bool SpaceReady = false;

        public Game1() 
        {
            this.Window.Position = new Point(400, 100);
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferHeight = screenheight;
            graphics.PreferredBackBufferWidth = screenwidth;

            objball = new Ball(0, 0, ballBegin);
            objBluePlayer = new Player(10, blueStartX, playerStartY);
            objRedPlayer = new Player(10, redStartX, playerStartY);
            objlives = new Vector2(0, 0);
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
            objball.Speed = 7;
            objball.Position = ballBegin;
            //objBluePlayer.X = blueStartX;
            //objBluePlayer.Y = playerStartY;
            //objRedPlayer.X = redStartX;
            //objRedPlayer.Y = playerStartY;
            //G: Kunnen we deze vier dan gewoon weggooien?
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
            miss = Content.Load<SoundEffect>("Audio/PONG.SOUND_MISS");
            paddle = Content.Load<SoundEffect>("Audio/PONG.SOUND_PADDLE");
            wall = Content.Load<SoundEffect>("Audio/PONG.SOUND_WALL");
            font = Content.Load<SpriteFont>("Font");
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
            //if (x > screenwidth)
            if (x + ballWidth > blueStartX)
            {
                blueLives--;
                miss.Play();
                ResetGame();
            }
            //if (x < 0)
            if (x < redStartX + playerWidth)
            {
                redLives--;
                miss.Play();
                ResetGame();
            }
            if (y >= screenheight - ballLength || y <= 0)
            {
                objball.Direction = -objball.Direction;
                wall.Play();
            }
            // Up above is mentioned the code which calls the ball position.

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
            // Up above is mentioned the reactions on the players' actions.

            if (IsRectangleInRectangle(CalculateNewballPos(), new Vector2(CalculateNewballPos().X + ballWidth, CalculateNewballPos().Y + ballLength), new Vector2(objBluePlayer.X, objBluePlayer.Y), new Vector2(objBluePlayer.X + playerWidth, objBluePlayer.Y + playerLength))
                || IsRectangleInRectangle(CalculateNewballPos(), new Vector2(CalculateNewballPos().X + ballWidth, CalculateNewballPos().Y + ballLength), new Vector2(objRedPlayer.X, objRedPlayer.Y), new Vector2(objRedPlayer.X + playerWidth, objRedPlayer.Y + playerLength)))
            {
                objball.Direction = 180 - objball.Direction;
                paddle.Play();
                // objball.Direction += rnd.Next(-10, 10);
                //F: Dit zorgt er voor dat de bal iets willekeuriger terugkaatst.
                //G: Oh. Zullen we het dan meteen implementeren?
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Space) && SpaceReady == false)
            {
                SpaceReady = true;
                ResetGame();
            }

            if (blueLives <= 0 || redLives <= 0)
            {
                Exit();
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        /// 

        public void DrawLives(short lives, bool isRed)
        {
            for (int i = 0; i < lives; i++)
            {
                if (isRed) //we are drawing red's lives (their lives should be in topleft corner)
                {
                    spriteBatch.Draw(ball, new Vector2(i * 16, 0), Color.White);
                }
                else //we are drawing blue's lives (their lives should be in topright corner)
                {
                    spriteBatch.Draw(ball, new Vector2(screenwidth - 16 - i * 16, 0), Color.White);
                }
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);
            spriteBatch.Begin();
            spriteBatch.DrawString(font, "Press space to begin", new Vector2(screenwidth/2, screenheight/2), Color.Black);
            if (SpaceReady)
            {
                spriteBatch.Draw(ball, objball.Position, Color.White);
                DrawLives(redLives, true);
                DrawLives(blueLives, false);
                spriteBatch.Draw(bluePlayer, new Vector2(objBluePlayer.X, objBluePlayer.Y), Color.White);
                spriteBatch.Draw(redPlayer, new Vector2(objRedPlayer.X, objRedPlayer.Y), Color.White);
            }
            spriteBatch.End();

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
