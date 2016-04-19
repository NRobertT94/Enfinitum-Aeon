using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
namespace EnfinitumAeon2
{
    /// <summary>
    /// Planet class...Simple as that..
    /// </summary>
    public class Planet : GameSettings
    {
        private float distance;
        private Vector2 newPosition;
        private Texture2D texture;
        private Texture2D displayStats;
        private Texture2D battleButton;
        private Vector2 position;
        private float angle;
        private float speed;
        private string name;
        private float offset;
        private float scale;
        private int level;
        private float time;
        private bool clicked;
        private float initialSpeed;
        private float initialScale;
        private int enemyShips;
        private Vector2 initialPosition;
        private Player player;
        private int defences, enfinitum, credits, chance;           // these are local vars only
        private bool conquered;

        // Constructor... amountOfSatellites at the moment is 0. If someone wants to make the satellites.... 
        public Planet(int level, String name, float distance, float angle, float offset, float scale, 
            float speed, int enemyShips, Player player, GraphicsDeviceManager graphics)
            : base(graphics)
        {
            this.name = name;
            this.angle = angle;
            this.distance = distance;
            this.speed = speed;
            this.offset = offset;
            this.scale = scale;
            this.level = level;
            time = 0;
            initialScale = scale + 0.001f;
            initialSpeed = speed;
            initialPosition = position;
            position = new Vector2(distance, angle);
            this.enemyShips = enemyShips;
            this.player = player;

            defences = enemyShips;
            conquered = false;
            if (defences == 0)
                conquered = true;
            enfinitum = calculateEnfinitum();
            chance = calculateChance();
            credits = calculateCredits();
        }

        /// <summary>
        /// Calculates how much Enfinitum the user gets from this planet; calculated from the current level 
        /// and the planet's defences.
        /// </summary>
        /// <returns>-1 if the user has already taken this planet's enfinitum, a value > 0 otherwise</returns>
        public int calculateEnfinitum()
        {
            if (conquered)
                return -1;

            int enf = 0, temp = 0;
            if (level == 1)
                temp = (int)(defences * 0.4);
            else if (level == 2)
                temp = (int)(defences * 1.5);
            else if (level == 3)
                temp = (int)(defences * 2.7);
            enf = temp;
            enfinitum = enf;

            return enfinitum;
        }

        /// <summary>
        /// Calculates the chance of victory in battle based on the defences level of the Planet.
        /// Checks that the Planet is controlled by the enemy before bothering to calculate.
        /// </summary>
        /// <returns>Int value for chance; -1 if the user has already beaten this planet</returns>
        public int calculateChance()
        {
            if (conquered)
                return -1;

            int cha = 0;
            int temp = defences * level;
            double avg = ((double)temp / 300);
            int rounded = 100 - (int)(avg * 100);

            if (player.Ship.Level == 1)
                cha = rounded - (15 * level);
            else if (player.Ship.Level > 1 && player.Ship.Level <= 3)
                cha = rounded - (20 * level);
            else if (player.Ship.Level > 3)
                cha = rounded + 5;
            else if (player.Ship.Level == 5)
                cha = rounded + 15;

            if (cha > 100)      // makes sure the chance isn't greater than 100
                cha = 100;
            else if (cha < 0)
                cha = 0;
            chance = cha;

            return cha;
        }

        /// <summary>
        /// Calculates how many credits the User would get for taking this Planet, based on their 
        /// current level and chance of victor.
        /// </summary>
        /// <returns>Int value for credits due; -1 returned if none</returns>
        public int calculateCredits()
        {
            if (conquered)
                return -1;
            int cred = level * (int)((100 - chance) / 1.5);
            credits = cred;

            return cred;
        }

        /// <summary>
        /// Loads planet and satellite texture as well as fonts
        /// </summary>
        /// <param name="content"></param>
        public void LoadContent(ContentManager content)
        {
           
            texture = content.Load<Texture2D>("Planets/Level" + level + "/" + name);
            battleButton = content.Load<Texture2D>("Planets/battleButton");
            displayStats = content.Load<Texture2D>("Planets/displayStats3");
            LoadOptionsContent(content);
        }
        /// <summary>
        /// Moves the planets and satellites
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            UpdateInputState();
            position = CalculateRotationCoordinates();
            
        }
        /// <summary>
        /// Draws planets with names and writes "works" if you click on a planet
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, null, Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
        }
        public void drawStats(SpriteBatch spriteBatch)
        {
            string display = name + '\n' + "Defences: " + defences + '\n';
            if (conquered)
            {
                display += "You have" + "\n" + "already" + "\n" + "conquered" + "\n" + "this planet!";
                // NO BATTLE BUTTON HERE
            }
            else
            {
                display += "Enfinitum: " + enfinitum + '\n' + "Chance: " + chance + '\n' +
                    "Credits: " + credits;
                // BATTLE BUTTON HERE
                spriteBatch.Draw(battleButton, new Vector2((screenWidth - displayStats.Width), 10 + displayStats.Height + 100), Color.White);
                spriteBatch.DrawString(biggerFont, "BATTLE", new Vector2(screenWidth - battleButton.Width + 30, displayStats.Height + 140), Color.Red);
                if (clickedBattle())
                {
                    clicked = true;
                }
            }
            spriteBatch.Draw(displayStats, new Vector2((screenWidth - displayStats.Width), (displayStats.Height + battleButton.Height + 200)), Color.White);
            spriteBatch.DrawString(mediumFont,display, new Vector2((screenWidth - displayStats.Width + 30), displayStats.Height + battleButton.Height + 240), Color.White);
        }

        /// <summary>
        /// Return 1 if Battle is clicked
        /// </summary>
        public bool clickedBattle()
        {
            if (HoveringOverText("BATTLE", new Vector2(screenWidth - battleButton.Width + 30, displayStats.Height + 140), biggerFont) && Clicked())
                return true;
            return false;
        }
        /// <summary>
        /// Rotation physics
        /// </summary>
        /// <returns></returns>
        public Vector2 CalculateRotationCoordinates()
        {
            angle += speed;
            Vector2 centerOfRotation = new Vector2(screenWidth - offset / 2, screenHeight - offset / 2);
            float rads = ToRadians(angle);
            float x = (centerOfRotation.X + distance * (float)Math.Sin(rads)) / 2 - offset;
            float y = (centerOfRotation.Y + distance * (float)Math.Cos(rads)) / 2 - offset;
            newPosition = new Vector2(x, y);
            return newPosition;
        }
        public void ZoomIn(GameTime gameTime)
        {
            if (clicked)
            {
                speed = 0f;
                time += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (time > 0.001f && !OverSized())
                {
                    scale += 0.004f;
                    time = 0;
                }
            }
        }
        public void ZoomOut(GameTime gameTime)
        {
            speed = initialSpeed;
            position = initialPosition;
            time += (float)gameTime.ElapsedGameTime.Milliseconds;
            if (time >= 0.1f && initialScale <= scale)
            {

                scale -= 0.005f;
                time = 0;
            }

        }
        public bool PlanetMouseStates(string action)
        {
            currentMouseState = Mouse.GetState();
            float width = texture.Width * scale;
            float height = texture.Height * scale;
            var spriteLocation = new Rectangle((int)position.X, (int)position.Y, (int)width, (int)height);
            bool mouseOver = spriteLocation.Contains(currentMouseState.X, currentMouseState.Y);
            if (action.Equals("Clicked") && mouseOver && Clicked())
                return true;
            if (action.Equals("Hovering") && mouseOver)
                return true;
            return false;
        }
        public bool OverSized()
        {
            float locationScale = texture.Height * scale;
            var spriteLocation = new Rectangle((int)position.X, (int)position.Y, (int)locationScale, (int)locationScale);
            if (spriteLocation.Width >= screenWidth || spriteLocation.Height >= screenHeight)
                return true;
            return false;
        }
        public Rectangle Size()
        {
            float width = texture.Width * scale;
            float height = texture.Height * scale;
            Rectangle size = new Rectangle((int)position.X, (int)position.Y, (int)width, (int)height);
            return size;
        }      
        // to radians formula
        public float ToRadians(float angle)
        {
            return (float)(Math.PI / 180) * angle;
        }
        public float Distance
        {
            get { return distance; }
            set { distance = value; }
        }
        public float Angle
        {
            get { return angle; }
            set { angle = value; }
        }
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }
        public string Name
        {
            get { return name; }
        }
        public float Scale
        {
            get { return scale; }
        }
        public Texture2D Texture
        {
            get { return texture; }
        }
        public int Level
        {
            get { return level; }
            set { level = value; }
        }
        public Vector2 InitialPosition
        {
            get { return initialPosition; }
        }
        public float InitialScale
        {
            get { return initialScale; }
        }
        public int EnemyShips
        {
            get { return enemyShips; }
            set { enemyShips = value; }
        }

        public int Enfinitum
        {
            get { return enfinitum; }
            set { enfinitum = value; }
        }

        public int Chance
        {
            get { return chance; }
        }

        public int Defences
        {
            get { return defences; }
            set { defences = value; }
        }

        public int Credits
        {
            get { return credits; }
            set { credits = value; }
        }

        public bool Conquered
        {
            get { return conquered; }
            set { conquered = value; }
        }
        public Player Player 
        {
           get{ return player;}
           set {player = value;}
        }
        public bool Clicked
        {
            get { return clicked; }
            set { clicked = value; }
        }
    }
}