using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Data;
using System.Diagnostics;

namespace Game9
{

    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        Texture2D ball, redPlayer, bluePlayerSprite, powerUpSprite, PU_Plus, PU_Speed, PU_Heart;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SoundEffect wall, miss, paddle;
        Vector2 ballBegin = new Vector2(screenwidth/2, screenheight/2);
        const float ballAcceleration = 0.002f, pwpTime = 15f;//extra speed gained per gametick.
        const short screenwidth = 1200, screenheight = 800, blueStartX = screenwidth - 60, redStartX = 60,
            playerStartY = screenheight / 2, playerLength = 95, playerWidth = 15, ballHeight = 15, ballWidth = 15, pwpHeight = 32, pwpWidth = 32;
        //bluestartx: x coordinate where the blue player starts. redStartX: x coordinate where the red player starts.
        //playerlength: length of the rectangle either player is controlling. Playerwidth: how wide the rectangle is the player is controlling.
        Ball[] objball = new Ball[5];
        Player objBluePlayer, objRedPlayer;
        SpriteFont font;
        short redLives = 5, blueLives = 5, amountOfBalls = 0;
        float powerUpTime;
        PowerUp pwp;
        bool SpaceReady = false;
        Rectangle ballRect, bluePlayerRect, redPlayerRect, pwpRect;


        public Game1() 
        {
            this.Window.Position = new Point(400, 100);
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            objball[0] = new Ball(0, 0, ballBegin);
            graphics.PreferredBackBufferHeight = screenheight;
            graphics.PreferredBackBufferWidth = screenwidth;
            objBluePlayer = new Player(10, blueStartX, playerStartY);
            objRedPlayer = new Player(10, redStartX, playerStartY);
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

        //The following method resets the position of the ball rather than resetting the game.
        //Since the ball is the only object of importance, this should be fine.
        //(As in, it isn't as if the Players are forced to reset their position too after every score anyway.)
        protected void ResetGame()
        {
            Array.Clear(objball, 0, objball.Length);
            powerUpTime = pwpTime;
            Random rnd = new Random();
            objball[0] = new Ball(0, 7, ballBegin);
            amountOfBalls = 1;
            //objball[0].Speed = 7;
            if (rnd.Next(1, 3) == 1)
                objball[0].Direction = rnd.Next(-75, 75);
            else
                objball[0].Direction = rnd.Next(105, 255);

            //Debug.WriteLine("TESTET#%");
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
            bluePlayerSprite = Content.Load<Texture2D>("Graphics/blauweSpeler");
            redPlayer = Content.Load<Texture2D>("Graphics/rodeSpeler");
            PU_Plus = Content.Load<Texture2D>("Graphics/powerup_ballplus");
            PU_Speed = Content.Load<Texture2D>("Graphics/powerup_ballspeed");
            PU_Heart = Content.Load<Texture2D>("Graphics/powerup_heart");
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
        //The following method is meant to regularly check where the position of the ball is, based on its corners.
        public bool IsVectorInRectangle(Vector2 vect, Vector2 topLeft, Vector2 bottomRight)
        {
            if ((vect.X >= topLeft.X && vect.X <= bottomRight.X) && (vect.Y >= topLeft.Y && vect.Y <= bottomRight.Y))
            {
                return true;
            }

            return false;
        }
        //The following method is meant to regularly check whether the ball has touched the Players or not.
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
        //The following method is meant to regularly check where the position of the top-left corner of the ball is.
        protected Vector2 CalculateNewballPos(int ballId)
        { 
            Vector2 pos = objball[ballId].Position;
            float x = pos.X;
            float y = pos.Y;
            double dir = objball[ballId].Direction * Math.PI/180;
            float speed = objball[ballId].Speed;
            x = (float) (x + Math.Cos(dir) * speed);
            y = (float) (y + Math.Sin(dir) * speed);
            return new Vector2(x, y);
        }
        //The following method decides by random which power up is going to be called/spawned.
        public void CreateNewPowerUp()
        {
            pwp = null;
            Random rnd = new Random();
            short choice = (short)rnd.Next(2);
            short x =  (short) (rnd.NextDouble() * (blueStartX - (screenwidth/4 + redStartX))+ redStartX + screenwidth/4);
            short y = (short) Math.Round(rnd.NextDouble()  * (screenheight - pwpHeight));
            switch (choice)
            {
                case 0:
                    powerUpSprite = PU_Plus;
                    break;
                case 1:
                    powerUpSprite = PU_Speed;
                    break;
                case 2:
                    powerUpSprite = PU_Heart;
                    break;
            }
            pwp = new PowerUp(x, y, powerUpSprite);
            //Debug.WriteLine("Hello World.");

        }

        public void ExecuteBallPhysics(short id, GameTime gameTime)
        {
            ballRect = new Rectangle((int) objball[id].Position.X, (int) objball[id].Position.Y, (int) (objball[id].Position.X + ballWidth), (int) (objball[id].Position.Y + ballHeight));
            bluePlayerRect = new Rectangle((int) objBluePlayer.X, );
            objball[id].Position = CalculateNewballPos(id);
            objball[id].Speed += ballAcceleration * (float)gameTime.ElapsedGameTime.TotalSeconds;
            float x = objball[id].Position.X;
            float y = objball[id].Position.Y;
            //The following statement makes the ball bounce the walls by means of fully inverting its direction.
            if (y >= screenheight - ballHeight || y <= 0)
            {
                objball[id].Direction = -objball[id].Direction;
                wall.Play();
            }
            //The following statement makes the ball bounce upon hitting the paddle.
            if (IsRectangleInRectangle(CalculateNewballPos(id), new Vector2(CalculateNewballPos(id).X + ballWidth, CalculateNewballPos(id).Y + ballHeight), new Vector2(objBluePlayer.X, objBluePlayer.Y), new Vector2(objBluePlayer.X + playerWidth, objBluePlayer.Y + playerLength))
            || IsRectangleInRectangle(CalculateNewballPos(id), new Vector2(CalculateNewballPos(id).X + ballWidth, CalculateNewballPos(id).Y + ballHeight), new Vector2(objRedPlayer.X, objRedPlayer.Y), new Vector2(objRedPlayer.X + playerWidth, objRedPlayer.Y + playerLength)))
            {
                Random rnd = new Random();
                objball[id].Direction = 180 - objball[id].Direction;
                paddle.Play();
                objball[id].Direction += rnd.Next(-10, 10);
            }
            //The following statement regularly checks whether the ball hits the power up or not.
            if (pwp != null)
            {
                if (IsRectangleInRectangle(CalculateNewballPos(id), new Vector2(CalculateNewballPos(id).X + ballWidth, CalculateNewballPos(id).Y + ballHeight), new Vector2(pwp.X, pwp.Y), new Vector2(pwp.X + pwpWidth, pwp.Y + pwpHeight)))
                {

                    if (pwp.Sprite == PU_Heart)
                    {
                        if (redLives < 5)
                            redLives++;
                        if (blueLives < 5)
                            blueLives++;
                    }
                    else if (pwp.Sprite == PU_Speed)
                        objball[id].Speed += 2;
                    else if (pwp.Sprite == PU_Plus && amountOfBalls < 5)
                    {
                        objball[amountOfBalls] = new Ball((int)(-0.5 * objball[id].Direction), objball[id].Speed - 2, objball[id].Position);
                        amountOfBalls += 1;
                    }
                    pwp = null;
                }
            }
            //The following statements declare when a player has scored.
            //If the ball passes through a certain x-value, their opponent's life will decrease by one.
            if (x + ballWidth > blueStartX)
            {
                blueLives--;
                miss.Play();
                ResetGame();
            }
            if (x < redStartX + playerWidth)
            {
                redLives--;
                miss.Play();
                ResetGame();
            }
        }

         /// <summary>
         /// Allows the game to run logic such as updating the world,
         /// checking for collisions, gathering input, and playing audio.
         /// </summary>
         /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            for (short i = 0; i < objball.Length; i++)
            {
                if (objball[i] != null)
                {
                    ExecuteBallPhysics(i, gameTime);
                }
            }

            //The escape button terminates the program run.
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

            //The following statements mention the reactions on the players' actions.
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
                if (objBluePlayer.Y + playerLength + objBluePlayer.Speed < screenheight)
                    objBluePlayer.Y += objBluePlayer.Speed;

            //The following statement starts the game when the player(s) have pushed the space-button.
            if (Keyboard.GetState().IsKeyDown(Keys.Space) && SpaceReady == false)
            {
                SpaceReady = true;
                ResetGame();
            }

            //The following statement states how the game will end.
            if (blueLives <= 0 || redLives <= 0)
                Exit();

            //The following statements determine how often the power ups appear.
            powerUpTime -=  (float)gameTime.ElapsedGameTime.TotalSeconds;
            //Debug.WriteLine("PWP: " + powerUpTime);
            if (powerUpTime <= 0)
            {
                //objball[i].Position = new Vector2(objball[i].Position.X, objball[i].Position.Y);
                CreateNewPowerUp();
               // Debug.WriteLine("test12345");
                powerUpTime = pwpTime;
            }
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        /// 

        //The following method draws the amount of lives the players have.
        public void DrawLives(short lives, bool isRed)
        {
            for (int i = 0; i < lives; i++)
            {
                if (isRed) //We are drawing the red player's lives in topleft corner.
                {
                    spriteBatch.Draw(ball, new Vector2(i * 16, 0), Color.White);
                }
                else //We are drawing the blue player's lives in topright corner.
                {
                    spriteBatch.Draw(ball, new Vector2(screenwidth - 16 - i * 16, 0), Color.White);
                }
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);
            spriteBatch.Begin();
            if (!SpaceReady)
            { spriteBatch.DrawString(font, "Press space to begin", new Vector2(screenwidth / 2, screenheight / 2), Color.Black); }
            else
            {
                for (int i = 0; i < objball.Length; i++)
                {if (objball[i] != null)
                    {
                        spriteBatch.Draw(ball, objball[i].Position, Color.White);
                    }
                }
                DrawLives(redLives, true);
                DrawLives(blueLives, false);
                spriteBatch.Draw(bluePlayerSprite, new Vector2(objBluePlayer.X, objBluePlayer.Y), Color.White);
                spriteBatch.Draw(redPlayer, new Vector2(objRedPlayer.X, objRedPlayer.Y), Color.White);
                if (pwp != null)
                {
                   // Debug.WriteLine("test1");
                    //Debug.WriteLine("X: " + pwp.X.ToString() + " Y: " + pwp.Y.ToString());
                    spriteBatch.Draw(pwp.Sprite, new Vector2(pwp.X, pwp.Y), Color.White);
                }

            }
            spriteBatch.End();

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
