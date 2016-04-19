using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
namespace EnfinitumAeon2
{
    /// <summary>
    /// This class has all the planets
    /// </summary>
    public class SolarSystem : GameSettings
    {     
        private Player player;    
        private Texture2D background;
        private Texture2D playerStats;
        private Planet[] solarObjects;
        private Planet selected;
        private string[] solarObjectNames;
        private float[] distance;
        private float[] angle;
        private float[] diameter;
        private float[] speed;
        private float[] scale;
        private int[] enemies;
        private int level;
        private bool isSelected;
        private bool combatMode;
        private Save save;
        /// <summary>
        /// In the constructor, level will be used to load the apropiate content for each solar system
        /// </summary>
        /// <param name="level"></param>
        /// <param name="graphics"></param>
        public SolarSystem(Player player, int level, GraphicsDeviceManager graphics, Save save)
            : base(graphics)
        {
            this.player = player;
            this.level = level;
            this.save = save;
            InitializeSolarSystem(level);
            combatMode = false;
            isSelected = false;
        }
        /// <summary>
        /// Loads the fonts from GameOptions, background texture and
        /// the content for each planet
        /// </summary>
        /// <param name="content"></param>
        public void LoadContent(ContentManager content)
        {
            LoadOptionsContent(content);
            playerStats = content.Load<Texture2D>("Planets/displayStats3");
            background = content.Load<Texture2D>("Backgrounds/space" + level);
            for (int i = 0; i < solarObjects.Length; i++)           
                solarObjects[i].LoadContent(content);               
            
          //combatSong = content.Load<Song>("Sounds/combatSong");              
        }
        /// <summary>
        /// This updates the planets position
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime, ContentManager content)
        {
            UpdateInputState();
            UpdateInputState();
            for (int i = 0; i < solarObjects.Length; i++)
            {
                solarObjects[i].Update(gameTime);
                if (solarObjects[i].Clicked)
                {
                    selected = solarObjects[i];
                    selected.EnemyShips = solarObjects[i].EnemyShips;
                    selected.Level = solarObjects[i].Level;
                    combatMode = true;
                }


            }

            if (F5Pressed())
            {
                save.saveGame();
            }
        }
        /// <summary>
        /// Draws background and planets
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(background, new Rectangle(0, 0, screenWidth, screenHeight), Color.White);
            spriteBatch.DrawString(mediumFont, "Credits: " + player.Credits + "\n" + "Enfinitum: " + player.Enfinitum + "\n" + "Ship: " + player.Ship.Name + "\n" + "Attack: " + player.Ship.Attack
                + "\n" + "Defense: " + player.Ship.Defense + "\n" + "Speed: " + player.Ship.MoveSpeed, new Vector2((screenWidth - playerStats.Width) + 30, 40), Color.White);
            spriteBatch.Draw(playerStats, new Vector2((screenWidth - playerStats.Width), 10), Color.White);
            for (int i = 0; i < solarObjects.Length; i++)
            {
                solarObjects[i].Draw(spriteBatch);
                if (solarObjects[i].PlanetMouseStates("Clicked"))
                {
                    selected = solarObjects[i];
                    isSelected = true;
                }

            }
            if (isSelected)
            {
                selected.drawStats(spriteBatch);

            }

        }
        public void InitializeSolarSystem(int level)
        {
            if (level == 1)
            {
                solarObjectNames = new string[10] { "Sun", "Mercury", "Venus", "Earth", "Mars", "Jupiter", "Saturn", "Uranus", "Neptune", "Pluto" };
                distance = new float[10] { 0, 170, 190, 260, 340, 470, 600, 650, 700, 750 };
                angle = new float[10] { 0, 45, 170, 32, 220, 14, 150, 105, 15, 82 };
                diameter = new float[10] { 35, 5, 7, 8, 6, 15, 16, 10, 9, 2 };
                scale = new float[10] { 0.06f, 0.005f, 0.01f, 0.01f, 0.01f, 0.09f, 0.08f, 0.05f, 0.06f, 0.01f };
                speed = new float[10] { 0f, 0.06f, 0.08f, 0.06f, 0.08f, 0.06f, 0.04f, 0.07f, 0.09f, 0.05f };
                enemies = new int[10] { 0, 6, 12, 18, 24, 30, 36, 42, 48, 54 };
                solarObjects = new Planet[10];                   
            }
            else if (level == 2)
            {
                solarObjectNames = new string[10] { "Odin", "Thor", "Eir", "Njorun", "Snotra", "Draugr", "Forseti", "Loki", "Frigg", "Meili" };
                distance = new float[10] { 0, 170, 190, 260, 340, 470, 600, 650, 700, 750 };
                angle = new float[10] { 0, 45, 170, 32, 220, 14, 150, 105, 15, 82 };
                scale = new float[10] { 0.1f, 0.02f, 0.03f, 0.02f, 0.06f, 0.09f, 0.08f, 0.05f, 0.06f, 0.03f };
                diameter = new float[10] { 35, 5, 7, 8, 6, 15, 16, 10, 9, 2 };
                enemies = new int[10] { 0, 24, 32, 40, 48, 56, 64, 72, 80, 88 };
                solarObjects = new Planet[10];              
                speed = new float[10] { 0f, 0.06f, 0.08f, 0.06f, 0.08f, 0.06f, 0.04f, 0.07f, 0.09f, 0.05f };              
            }
            else if (level == 3)
            {
                 solarObjectNames=new string[10]{ "Ra", "Nut", "Shu", "Osiris", "Isis", "Set", "Nephtys", "Horus", "Anubis","Sobek" };
                 distance = new float[10] { 0, 170, 190, 260, 340, 470, 600, 650, 700, 750 };
                 angle = new float[10] { 0, 45, 170, 32, 220, 14, 150, 105, 15, 82 };
                 scale = new float[10] { 0.1f, 0.02f, 0.03f, 0.02f, 0.06f, 0.09f, 0.08f, 0.05f, 0.06f, 0.03f };
                 diameter = new float[10] { 35, 5, 7, 8, 6, 15, 16, 10, 9, 2 };
                 enemies = new int[10] { 0, 20, 30, 40, 50, 60, 70, 80, 90, 100 };
                 solarObjects = new Planet[10];             
                 speed = new float[10] { 0f, 0.06f, 0.08f, 0.06f, 0.08f, 0.06f, 0.04f, 0.07f, 0.09f, 0.05f };                
            }
            for (int i = 0; i < solarObjectNames.Length; i++)
            {
                solarObjects[i] = new Planet(level, solarObjectNames[i], distance[i], angle[i], diameter[i], scale[i],
                    speed[i], enemies[i], player, graphics);
             
            }
        }
        /// <summary>
        /// Resets the values of the solar system instances, readying them to be restored to from save.
        /// </summary>
        /// <param name="level">The level of the solar system being reset</param>
        public void ResetSolarSystem(int level)
        {
            if (level == 1)
                enemies = new int[10] { 0, 10, 15, 20, 25, 30, 35, 40, 45, 50 };
            else if (level == 2)
                enemies = new int[10] { 0, 20, 25, 30, 35, 40, 45, 50, 55, 60 };
            else if (level == 3)
                enemies = new int[10] { 0, 20, 25, 30, 35, 40, 45, 50, 55, 60 };

            for (int i = 1; i < Planets.Length; i++)       // starts at 1, because the Sun's value never changes
            {
                Planets[i].Conquered = false;
                Planets[i].Defences = enemies[i];
                Planets[i].calculateEnfinitum();
                Planets[i].calculateChance();
                Planets[i].calculateCredits();
            }
        }
        public Planet[] Planets
        {
            get { return solarObjects; }
        }
        public int Level
        {
            get { return level; }
        }     
        public Planet SelectedPlanet
        {
            get { return selected; }
        }
        public bool CombatMode
        {
            get { return combatMode;}
            set { combatMode = value; }
        }
    }
}
