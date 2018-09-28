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
            playerStartY = screenheight / 2, playerHeight = 95, playerWidth = 16, ballHeight = 15, ballWidth = 15, pwpHeight = 32, pwpWidth = 32;

        const short blueScoreLine = blueStartX + 2, redScoreLine = redStartX - 2,
            redFrontLine = redStartX + playerWidth, blueFrontLine = blueStartX;

        //blueStartX: De x-coordinaat waar de blauwe speler start. redStartX: De x-coordinaat waar de rode speler start.
        //playerHeight: De lengte van de rechthoek waar de spelers mee spelen. playerWidth: De breedte van de rechthoek waar de spelers mee spelen
        Ball[] objball = new Ball[5];
        Player objBluePlayer, objRedPlayer;
        SpriteFont font;
        short redLives = 5, blueLives = 5, amountOfBalls = 0;
        float powerUpTime;
        PowerUp pwp;
        bool SpaceReady = false, eindeSpel = false;
        Rectangle ballRect, ballNextRect, bluePlayerRect, redPlayerRect, pwpRect;


        public Game1() 
        {
            this.Window.Position = new Point(400, 100);
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            objball[0] = new Ball(0, 0, ballBegin);
            /*ball[0] bestaat altijd. Deze kunnen we als referentiepunt gebruiken om bijvoorbeeld
            te kijken wat de breedte is van een bal. Ze hebben immers allemaal dezelfde sprite.*/
            graphics.PreferredBackBufferHeight = screenheight;
            graphics.PreferredBackBufferWidth = screenwidth;
            objBluePlayer = new Player(10, blueStartX, playerStartY);
            objRedPlayer = new Player(10, redStartX, playerStartY);
        }
        protected override void Initialize()
        {
            base.Initialize();
        }

        //De volgende methode zet de positie van de bal terug, en verwijdert ook de extra ballen.
        //We verwijderen ze met de Array.Clear functie.
        protected void ResetGame()
        {
            Array.Clear(objball, 0, objball.Length);
            powerUpTime = pwpTime;
            Random rnd = new Random();
            objball[0] = new Ball(0, 7, ballBegin);
            amountOfBalls = 1;
            if (rnd.Next(1, 3) == 1)
                objball[0].Direction = rnd.Next(-75, 75);
            else
                objball[0].Direction = rnd.Next(105, 255);

        }

        protected override void LoadContent()
        {
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
        }

        protected override void UnloadContent()
        {}
        //De volgende methode is bedoeld om te controleren waar de positie van de bal is, op basis van de hoeken ervan.
        /*public bool IsVectorInRectangle(Vector2 vect, Vector2 topLeft, Vector2 bottomRight)
        {
            if ((vect.X >= topLeft.X && vect.X <= bottomRight.X) && (vect.Y >= topLeft.Y && vect.Y <= bottomRight.Y))
            {
                return true;
            }

            return false;
        } */

        //De volgende methode is bedoeld om te controleren of de bal de spelers heeft geraakt of niet.
        /*public bool IsRectangleInRectangle(Vector2 topLeft1, Vector2 bottomRight1, Vector2 topLeft2, Vector2 bottomRight2)
        {
            Vector2 topRight1 = new Vector2(bottomRight1.X, topLeft1.Y);
            Vector2 bottomLeft1 = new Vector2(topLeft1.X, bottomRight1.Y);
            if (IsVectorInRectangle(topLeft1, topLeft2, bottomRight2) || IsVectorInRectangle(bottomLeft1, topLeft2, bottomRight2) ||
                IsVectorInRectangle(topRight1, topLeft2, bottomRight2) || IsVectorInRectangle(bottomRight1, topLeft2, bottomRight2))
                return true;
            else
                return false;
        } */

        //De volgende methode is bedoeld om de positie van de bal te veranderen.
        public Vector2 BallPositionChange(int id, int x, int y)
        {
            Vector2 pos = objball[id].Position;
            pos.X += x;
            pos.Y += y;
            return (new Vector2(pos.X, pos.Y));
        }

        //De volgende methode is bedoeld om te controleren waar de linkerbovenhoek positie van de bal is.
        public Vector2 CalculateNewballPos(int ballId)
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
        //De volgende methode kaatst de bal terug, met een kleine verandering in hoek om een beetje willekeurigheid toe te voegen.
        /*De bool horizontalbounce wordt gebruikt om te kijken of de bal tegen de onder/bovenkant
        van het scherm aan botst of tegen een balk van een speler aan.*/
        public void Bounce(int ballId, bool horizontalBounce)
        {
            Random rnd = new Random();
            if (horizontalBounce)
                objball[ballId].Direction = 180 - objball[ballId].Direction;
            else
                objball[ballId].Direction = -objball[ballId].Direction;

            objball[ballId].Direction += rnd.Next(-10, 10);
        }

        //De volgende methode bepaalt door welke power-up wordt geroept/gespawnt.
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
        }

        public void ExecuteBallPhysics(short id, GameTime gameTime)
        {
            ballRect = new Rectangle((int) objball[id].Position.X, (int) objball[id].Position.Y, ballWidth, ballHeight);
            ballNextRect = new Rectangle((int) (CalculateNewballPos(id).X), (int) (CalculateNewballPos(id).Y), ballWidth, ballHeight);
            //ballRect is de vierhoek waar de bal zich nu in bevindt. ballNextRect is de vierhoek waar de bal zich de volgende frame in zal bevinden.

            bluePlayerRect = new Rectangle(objBluePlayer.X, objBluePlayer.Y, playerWidth, playerHeight);
            redPlayerRect = new Rectangle(objRedPlayer.X, objRedPlayer.Y, playerWidth, playerHeight);

            objball[id].Position = CalculateNewballPos(id);
            objball[id].Speed += ballAcceleration * (float)gameTime.ElapsedGameTime.TotalSeconds;
            float x = objball[id].Position.X;
            float y = objball[id].Position.Y;

            //Deze verklaring zorgt ervoor dat de bal tegen de muren stuitert door de richting volledig om te keren.
            if (y >= screenheight - ballHeight || y <= 0)
            {
                Bounce(id, false);
                wall.Play();
            }
            //De volgende verklaring maakt de bal stuiterend als het balken raakt.
            /*if (IsRectangleInRectangle(CalculateNewballPos(id), new Vector2(CalculateNewballPos(id).X + ballWidth, CalculateNewballPos(id).Y + ballHeight), new Vector2(objBluePlayer.X, objBluePlayer.Y), new Vector2(objBluePlayer.X + playerWidth, objBluePlayer.Y + playerHeight))
            || IsRectangleInRectangle(CalculateNewballPos(id), new Vector2(CalculateNewballPos(id).X + ballWidth, CalculateNewballPos(id).Y + ballHeight), new Vector2(objRedPlayer.X, objRedPlayer.Y), new Vector2(objRedPlayer.X + playerWidth, objRedPlayer.Y + playerHeight)))*/
            if (ballNextRect.Intersects(bluePlayerRect))
            {
                objball[id].Position = new Vector2(objball[id].Position.X - 10, objball[id].Position.Y);
                Bounce(id, true);
                paddle.Play();
            }
            if(ballNextRect.Intersects(redPlayerRect))
            {
                objball[id].Position = new Vector2(objball[id].Position.X + 10, objball[id].Position.Y);
                Bounce(id, true);
                paddle.Play();
            }

            //De volgende verklaring checkt regelmatig of de bal een power up raakt of niet.
            if (pwp != null)
            {
                pwpRect = new Rectangle(pwp.X, pwp.Y, pwp.Sprite.Width, pwp.Sprite.Height);
                if (ballNextRect.Intersects(pwpRect))
                    //IsRectangleInRectangle(CalculateNewballPos(id), new Vector2(CalculateNewballPos(id).X + ballWidth, CalculateNewballPos(id).Y + ballHeight), new Vector2(pwp.X, pwp.Y), new Vector2(pwp.X + pwpWidth, pwp.Y + pwpHeight)))
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
            //De volgende verklaringen verklaren wanneer een speler heeft gescoord.
            //Als de bal een bepaalde x-waarde passeert, neemt de leven van zijn tegenstander met één af.
            if (x  > blueScoreLine)
            {
                blueLives--;
                miss.Play();
                ResetGame();
            }
            if (x < redScoreLine)
            {
                redLives--;
                miss.Play();
                ResetGame();
            }
        }

        protected override void Update(GameTime gameTime)
        {
            //Loops voor alle bal objecten die we hebben, en hun bewegingen toepassen.
            for (short i = 0; i < objball.Length; i++)
            {
                if (objball[i] != null)
                { ExecuteBallPhysics(i, gameTime); }
            }

            //De escape knop beëindigt het program.
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

            //De volgende verklaringen reageren op de acties van de spelers.
            if (Keyboard.GetState().IsKeyDown(Keys.W))
                if (objRedPlayer.Y - objRedPlayer.Speed > 0)
                    objRedPlayer.Y -= objRedPlayer.Speed;
            
            if (Keyboard.GetState().IsKeyDown(Keys.S))
                if (objRedPlayer.Y + playerHeight + objRedPlayer.Speed < screenheight)
                    objRedPlayer.Y += objRedPlayer.Speed;

            if (Keyboard.GetState().IsKeyDown(Keys.Up))
                if (objBluePlayer.Y - objBluePlayer.Speed > 0)
                    objBluePlayer.Y -= objBluePlayer.Speed;

            if (Keyboard.GetState().IsKeyDown(Keys.Down))
                if (objBluePlayer.Y + playerHeight + objBluePlayer.Speed < screenheight)
                    objBluePlayer.Y += objBluePlayer.Speed;

            //Deze instructie zorgt ervoor dat het spel begint wanneer de speler(s) op de spatie-knop hebben gedrukt.
            if (Keyboard.GetState().IsKeyDown(Keys.Space) && SpaceReady == false)
            {
                SpaceReady = true;
                ResetGame();
            }

            //De volgende verklaring geeft aan hoe het spel zal eindigen.
            if (blueLives <= 0 || redLives <= 0)
            {
                eindeSpel = true;
                Exit();
            }

            //De volgende verklaringen bepalen hoe vaak de power-ups verschijnen.
            powerUpTime -=  (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (powerUpTime <= 0)
            {
                CreateNewPowerUp();
                powerUpTime = pwpTime;
            }
            base.Update(gameTime);
        }

        //De volgende methode tekent het aantal levens dat de spelers hebben.
        public void DrawLives(short lives, bool isRed)
        {
            for (int i = 0; i < lives; i++)
            {
                if (isRed) //We tekenen het leven van de rode speler in de rechterbovenhoek van het scherm.
                { spriteBatch.Draw(ball, new Vector2(i * 16, 0), Color.White); }
                else //We tekenen het leven van de blauwde speler in de linkerbovenhoek van het scherm.
                { spriteBatch.Draw(ball, new Vector2(screenwidth - 16 - i * 16, 0), Color.White); }
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);
            spriteBatch.Begin();
            if (!SpaceReady)
                spriteBatch.DrawString(font, "Press space to begin", new Vector2(screenwidth / 2, screenheight / 2), Color.Black); 
            else
            {
                for (int i = 0; i < objball.Length; i++)
                {
                    if (objball[i] != null)
                    { spriteBatch.Draw(ball, objball[i].Position, Color.White); }
                }
                DrawLives(redLives, true);
                DrawLives(blueLives, false);
                spriteBatch.Draw(bluePlayerSprite, new Vector2(objBluePlayer.X, objBluePlayer.Y), Color.White);
                spriteBatch.Draw(redPlayer, new Vector2(objRedPlayer.X, objRedPlayer.Y), Color.White);
                if (pwp != null)
                { spriteBatch.Draw(pwp.Sprite, new Vector2(pwp.X, pwp.Y), Color.White); }

            }
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
