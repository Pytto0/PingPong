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
        Texture2D ballSprite, redPlayerSprite, bluePlayerSprite, powerUpSprite, PU_Plus, PU_Speed, PU_Heart;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SoundEffect wall, miss, paddle;
        Vector2 ballBegin = new Vector2(screenwidth / 2, screenheight / 2);
        const float ballAcceleration = 0.004f, pwpTime = 15f;
        //Met ballAcceleration geven we aan hoeveel de snelheid van een bal omhoog gaat per frame.
        //pwp staat in de rest van deze class altijd voor PowerUp, dus de pwpTime is de hoeveelheid tijd totdat er 
        //een nieuwe powerup ontstaat.

        const short screenwidth = 1200, screenheight = 800, blueStartX = screenwidth - 60, redStartX = 60,
            playerStartY = screenheight / 2;
        //Zowel blueStartX als redStartX zijn de beginpunten van de linkerbovenkant van de balken.

        short playerHeight, playerWidth, ballHeight, ballWidth, pwpHeight, pwpWidth, blueScoreLine, redScoreLine,
            redFrontLine, blueFrontLine, redLives = 5, blueLives = 5, amountOfBalls = 1;
        Ball[] objBall = new Ball[4]; //We maken hier een array aan die in totaal plek heeft voor 5 ballen.
        Player objBluePlayer, objRedPlayer;
        SpriteFont font;
        float pwpCounter; //Deze variable is de teller waarmee we aftellen tot de volgende powerup.
        PowerUp pwp;
        bool spaceReady = false;
        Rectangle ballRect, ballNextRect, bluePlayerRect, redPlayerRect, pwpRect;


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            this.Window.Position = new Point(400, 100);
            objBall[0] = new Ball(0, 0, ballBegin);
            //objBalll[0] bestaat altijd. Deze kunnen we als referentiepunt gebruiken om bijvoorbeeld
            //te kijken wat de breedte is van een bal. Ze hebben immers allemaal dezelfde sprite.
            graphics.PreferredBackBufferHeight = screenheight;
            graphics.PreferredBackBufferWidth = screenwidth;
            objBluePlayer = new Player(10, blueStartX, playerStartY);
            objRedPlayer = new Player(10, redStartX, playerStartY);

            //Overigens, het spel begint niet nadat deze instructie is uitgevoerd. Dat gebeurt pas hieronder
            //in de update functie, als een speler op de spatiebalk heeft gedrukt.
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected void IntialiseVariables()
        {
            playerWidth = (short)bluePlayerSprite.Width;
            playerHeight = (short) bluePlayerSprite.Height;
            ballWidth = (short)ballSprite.Width;
            ballHeight = (short)ballSprite.Height;
            pwpWidth = (short)PU_Plus.Width; 
            pwpHeight = (short)PU_Plus.Height;
            blueFrontLine = blueStartX;
            redFrontLine = (short)(redStartX + playerWidth);
            blueScoreLine = (short)(blueFrontLine + 8);
            redScoreLine = (short)(redFrontLine - 8);

        }

        //De volgende methode zet de positie van de bal terug op de oorspronkelijke plek, en verwijdert ook de extra ballen.
        //We verwijderen ze met de Array.Clear functie. De nieuwe bal (objBall[0]) krijgt een willekeurige richting mee,
        //danwel naar links, danwel naar rechts.
        public void ResetGame()
        {
            if (!IsGameEnded())
            {
                Array.Clear(objBall, 0, objBall.Length);
                pwpCounter = pwpTime;
                Random rnd = new Random();
                objBall[0] = new Ball(0, 7, ballBegin);
                amountOfBalls = 1;
                if (rnd.Next(1, 3) == 1)
                    objBall[0].Direction = rnd.Next(-75, 75);
                else
                    objBall[0].Direction = rnd.Next(105, 255);
            }
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            ballSprite = Content.Load<Texture2D>("Graphics/bal");
            bluePlayerSprite = Content.Load<Texture2D>("Graphics/blauweSpeler");
            redPlayerSprite = Content.Load<Texture2D>("Graphics/rodeSpeler");
            PU_Plus = Content.Load<Texture2D>("Graphics/powerup_ballplus");
            PU_Speed = Content.Load<Texture2D>("Graphics/powerup_ballspeed");
            PU_Heart = Content.Load<Texture2D>("Graphics/powerup_heart");

            miss = Content.Load<SoundEffect>("Audio/PONG.SOUND_MISS");
            paddle = Content.Load<SoundEffect>("Audio/PONG.SOUND_PADDLE");
            wall = Content.Load<SoundEffect>("Audio/PONG.SOUND_WALL");

            font = Content.Load<SpriteFont>("Font");

            IntialiseVariables(); //Nu alle sprites geladen zijn, kunnen we er gegevens (zoals hoogte of breedte) 
            //over opvragen en kunnen we daarmee een aantal van onze globale variables intialiseren.
        }

        protected override void UnloadContent()
        {
        }

        //De volgende methode is bedoeld om de positie van een punt makkelijk te veranderen.
        public Vector2 PositionChange(Vector2 pos, short xPlus, short yPlus)
        {
            pos.X += xPlus;
            pos.Y += yPlus;
            return new Vector2(pos.X, pos.Y);
        }

        //De volgende methode is bedoeld om de plek te berekenen waar we de bal de volgende frame naartoe zullen moeten verplaatsen.
        public Vector2 CalculateNewballPos(int ballId)
        {
            Vector2 pos = objBall[ballId].Position;
            float x = pos.X;
            float y = pos.Y;
            double dir = objBall[ballId].Direction * Math.PI / 180;
            float speed = objBall[ballId].Speed;
            return PositionChange(objBall[ballId].Position, (short)Math.Round(Math.Cos(dir) * speed), (short)Math.Round(Math.Sin(dir) * speed));
        }
        //De volgende methode kaatst de bal terug, met een kleine verandering in hoek om een beetje willekeurigheid toe te voegen.
        /*De bool horizontalbounce wordt gebruikt om te kijken of de bal tegen de onder/bovenkant
        van het scherm aan botst of tegen een balk van een speler aan.*/
        public void Bounce(int ballId, bool horizontalBounce)
        {
            Random rnd = new Random();
            if (horizontalBounce)
                objBall[ballId].Direction = 180 - objBall[ballId].Direction;
            else
                objBall[ballId].Direction = -objBall[ballId].Direction;

            objBall[ballId].Direction += rnd.Next(-5, 5);
        }

        //De volgende methode bepaalt door welke power-up wordt geroept/gespawnt. De x waarde wordt zo gekozen dat deze
        //in het midden van het scherm zit.
        public void CreateNewPowerUp()
        {
            pwp = null;
            Random rnd = new Random();
            short choice = (short)rnd.Next(2); //Kiest een willekeurige waarde: 0, 1 of 2. Aan de hand hiervan bepalen we wat voor soort powerup we krijgen.
            short x = (short)((rnd.NextDouble() * (blueFrontLine - screenwidth / 4) + redFrontLine + screenwidth / 4) - pwpWidth);
            short y = (short)Math.Round(rnd.NextDouble() * (screenheight - pwpHeight));
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
        //Met deze functie voeren we veranderingen van plaats op een bal uit.
        public void ExecuteBallPhysics(short id, GameTime gameTime)
        {
            if (!IsGameEnded()) //We willen niet dat dit allemaal nog uitgevoerd wordt als het spel al afgelopen is.
            {
                ballRect = new Rectangle((int)objBall[id].Position.X, (int)objBall[id].Position.Y, ballWidth, ballHeight);
                ballNextRect = new Rectangle((int)(CalculateNewballPos(id).X), (int)(CalculateNewballPos(id).Y), ballWidth, ballHeight);
                //ballRect is de vierhoek waar de bal zich nu in bevindt. ballNextRect is de vierhoek waar de bal zich de volgende frame in zal bevinden.

                bluePlayerRect = new Rectangle(objBluePlayer.X, objBluePlayer.Y, playerWidth, playerHeight);
                redPlayerRect = new Rectangle(objRedPlayer.X, objRedPlayer.Y, playerWidth, playerHeight);

                objBall[id].Position = CalculateNewballPos(id);
                objBall[id].Speed += ballAcceleration * (float)gameTime.ElapsedGameTime.TotalSeconds;
                //Dit zorgt er voor dat de bal versnelt.

                float x = objBall[id].Position.X;
                float y = objBall[id].Position.Y;


                //Met de volgende paar if-statements zorgen we er voor dat de bal stuitert als deze tegen een muur
                //of tegen de balk van speler aankomt.
                if (ballNextRect.Y >= screenheight - ballHeight || y <= 0)
                {
                    Bounce(id, false);
                    wall.Play();
                }
                if (ballNextRect.Intersects(bluePlayerRect))
                {
                    objBall[id].Position = new Vector2(objBall[id].Position.X - 10, objBall[id].Position.Y);
                    Bounce(id, true);
                    paddle.Play();
                }
                if (ballNextRect.Intersects(redPlayerRect))
                {
                    objBall[id].Position = new Vector2(objBall[id].Position.X + 10, objBall[id].Position.Y);
                    Bounce(id, true);
                    paddle.Play();
                }
                //De volgende verklaring checkt regelmatig of de bal een power up raakt of niet.
                if (pwp != null)
                {
                    pwpRect = new Rectangle(pwp.X, pwp.Y, pwp.Sprite.Width, pwp.Sprite.Height);
                    if (ballNextRect.Intersects(pwpRect))
                    {
                        if (pwp.Sprite == PU_Heart) 
                        {
                            if (redLives < 5)
                                redLives++;
                            if (blueLives < 5)
                                blueLives++;
                        }
                        else if (pwp.Sprite == PU_Speed)
                            objBall[id].Speed += 2;
                        else if (pwp.Sprite == PU_Plus && amountOfBalls < 5)
                        {
                            //Met de code hieronder maken we een in onze array een nieuwe bal aan die id amountOfBalls meekrijgt
                            //(dus altijd een nog niet bezette id). De richting van de bal is gelijk aan de helft van de richting van
                            //de bal die deze powerup aanraakte, de snelheid is gelijk aan de snelheid van de bal die de powerup aanraakte
                            //maar dan 2 pixels per frame minder, en de positie is gelijk aan de positie van de bal die deze powerup aanraakte.
                            objBall[amountOfBalls] = new Ball((int)(0.5 * objBall[id].Direction), objBall[id].Speed - 2, objBall[id].Position);
                            amountOfBalls += 1;
                        }
                        pwp = null;
                    }
                }

                //De code hieronder bepaalt of het spel is afgelopen.
                if (x > blueScoreLine)
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
            
        }

        public bool IsGameEnded()
        {
            if (blueLives <= 0 || redLives <= 0)
                return true; 
            else
                return false; 
        }

        protected override void Update(GameTime gameTime)
        {
            //Loopt alle ballen bij langs en past bij iedere afzonderlijke bal een positieverandering toe (als die er is).
            for (short i = 0; i < objBall.Length; i++)
            {
                if (objBall[i] != null)
                    ExecuteBallPhysics(i, gameTime); 
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            //Met de volgende if-statements zorgen we ervoor dat de speler kan bewegen met zijn balk.
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

            //Deze instructie zorgt ervoor dat het spel begint wanneer een speler op de spatie-knop hebben gedrukt.
            if (Keyboard.GetState().IsKeyDown(Keys.Space) && spaceReady == false)
            {
                spaceReady = true;
                ResetGame();
            }

            //Met de code hieronder tellen we af totdat we een nieuwe powerup moeten maken, en we maken er een
            //als het zo ver is (en we resetten de counter natuurlijk).
            pwpCounter -=  (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (pwpCounter <= 0)
            {
                CreateNewPowerUp();
                pwpCounter = pwpTime;
            }


            base.Update(gameTime);
        }

        //De volgende methode tekent het aantal levens dat de spelers hebben.
        public void DrawLives(short lives, bool isRed)
        {
            for (int i = 0; i < lives; i++)
            {
                if (isRed) 
                    spriteBatch.Draw(ballSprite, new Vector2(i * 16, 0), Color.White); 
                else
                    spriteBatch.Draw(ballSprite, new Vector2(screenwidth - 16 - i * 16, 0), Color.White); 
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);
            spriteBatch.Begin();
                if (!spaceReady && !IsGameEnded())
                    spriteBatch.DrawString(font, "Druk op de spatiebalk om te beginnen", new Vector2(screenwidth / 4, screenheight / 2), Color.Black);
                else if(spaceReady && !IsGameEnded())
                {
                    for (int i = 0; i < objBall.Length; i++)
                    {
                        if (objBall[i] != null)
                            spriteBatch.Draw(ballSprite, objBall[i].Position, Color.White); 
                    }
                    DrawLives(redLives, true);
                    DrawLives(blueLives, false);
                    spriteBatch.Draw(bluePlayerSprite, new Vector2(objBluePlayer.X, objBluePlayer.Y), Color.White);
                    spriteBatch.Draw(redPlayerSprite, new Vector2(objRedPlayer.X, objRedPlayer.Y), Color.White);
                if (pwp != null)
                {
                    spriteBatch.Draw(pwp.Sprite, new Vector2(pwp.X, pwp.Y), Color.White);
                }
            }

            else
            {
                if (blueLives <= 0)
                    spriteBatch.DrawString(font, "Rood wint, druk op escape om het spel af te sluiten", new Vector2(screenwidth / 4, screenheight / 2), Color.Black);
                else if (redLives <= 0)
                    spriteBatch.DrawString(font, "Blauw wint, druk op escape om het spel af te sluiten", new Vector2(screenwidth / 4, screenheight / 2), Color.Black);
                
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
