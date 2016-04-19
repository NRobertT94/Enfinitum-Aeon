using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace EnfinitumAeon2
{
    public class Combat : GameSettings
    {
        private Planet planet;
        private Texture2D background;
        private Texture2D HUD;
        private List<Ship> enemies;
        private Player player;
        private int initialEnemies;
        private float loadTime;    
        private Random rand;
        private bool gameOver;
        private bool victory;
        private bool exitCombat;
        private bool backToSystem;      
        private int limit;
        private int waves;
        private int currentWave;
          
        private List<Explosion> explosions;
        
        public Combat(Planet planet, Player player, GraphicsDeviceManager graphics)
            : base(graphics)
        {
            this.planet = planet;
            this.player = player;
            rand = new Random();
            initialEnemies = planet.EnemyShips;
            backToSystem = false;
            exitCombat = false;
            gameOver = false;
            victory = false;
            currentWave = 1;
            InitializeDifficulty();
            enemies = new List<Ship>(planet.EnemyShips);
            explosions = new List<Explosion>();
            waves = planet.EnemyShips / limit;
            InitializeEnemyShips();          
            loadTime = 0;
            player.RestoreHealth();
        }           
        //loads textures for ships
        public void LoadContent(ContentManager content)
        {
           
            LoadOptionsContent(content);
            HUD = content.Load<Texture2D>("Backgrounds/displayStats3");
            background = content.Load<Texture2D>("Backgrounds/space"+planet.Level);
            for (int i = 0; i < enemies.Count; i++)
                enemies[i].LoadContent(content);              
        }
        public void Update(EnfinitumAeon EA,GameTime gameTime, ContentManager content)
        {
            UpdateInputState();
            UpdateScreen(EA,gameTime); // zoom in and out stuff          
            if (planet.OverSized()) // when the planet is fixed
            {
               
                bool isASun = (Planet.Name == "Sun" || Planet.Name == "Odin" || Planet.Name == "Ra");
                if (!gameOver && !victory || isASun) 
                {

                    if (planet.EnemyShips == 0)
                        victory = true;
                    if (planet.Player.Ship.Destroyed)                                                                                                                                                    
                        gameOver = true;                                                                                                  
                    planet.Player.Update(gameTime, content, 0);
                    if (planet.Player.Ship.Hit)
                    {
                        explosions.Add(new Explosion(content, planet.Player.Ship, "Damage"));
                        player.Ship.Damage.Play();
                        player.Ship.Hit = false;
                    }   
           
                    for (int i = 0; i < enemies.Count && !planet.Player.Ship.Destroyed; i++)
                    {
                        planet.Player.Ship.TakesDamageFrom(enemies[i],content);
                        enemies[i].Update(gameTime, content);
                        enemies[i].TakesDamageFrom(player.Ship,content);
                        if (enemies[i].Hit)
                        {
                            explosions.Add(new Explosion(content, enemies[i], "Damage"));
                            enemies[i].Damage.Play();
                            enemies[i].Hit = false;

                        }                          
                        if (enemies[i].Destroyed)
                        {
                            player.Credits += enemies[i].Credits;
                            player.Experience += enemies[i].Experience;
                            enemies[i].Boom.Play();
                            explosions.Add(new Explosion(content, enemies[i],"Destroyed"));
                            enemies.RemoveAt(i);
                            planet.EnemyShips--;
                            planet.Clicked = false;
                        }
                    }
                    foreach (Explosion explosion in explosions)
                        explosion.Update(gameTime);
                    //initializes enemy ships when the wave is destroyed
                    if (currentWave < waves && enemies.Count == 0)
                    {
                        loadTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                      //  if(loadTime <= 2f)
                           currentWave++;
                       // if(loadTime > 4f)
                           InitializeEnemyShips();
                       // if (loadTime > 6f)
                           LoadContent(content);
                       // if (loadTime > 8f)
                            loadTime = 0;
                    }
                    ManageExplosions();
                }
                if (victory && planet.Enfinitum > 0)
                {
                    player.Credits += planet.Credits;
                    player.Enfinitum += planet.Enfinitum;
                    planet.Credits = 0;
                    planet.Enfinitum = 0;
                    planet.Defences = 0;
                }
                if(gameOver)
                    planet.EnemyShips = initialEnemies;
               
            }
        }
        //randomizes ship names... it's more likely that lesser ships will be spawned 
        public string Randomize()
        {
            if (rand.Next(0, 7) == 1)
                return "Marauder";
            else if (rand.Next(0, 7) == 2 || rand.Next(0, 7) == 3)
                return "Interceptor";
            else
                return "Scout";
        }
        //effects stuff for zoom in and out
        public void UpdateScreen(EnfinitumAeon EA, GameTime gameTime)
        {
            if (!backToSystem)                           
                planet.ZoomIn(gameTime);         
                                                            
            if (EscapePressed())           
                exitCombat = true;
                         
            if (exitCombat)
            {
                backToSystem = true;
                planet.ZoomOut(gameTime);
                if (planet.Scale <= planet.InitialScale)
                {
                    exitCombat = false;
                    backToSystem = false;
                    EA.QuitCombat = true;
                    planet.Clicked = false;
                    player.Ship.deleteFromScreen();
                  
                }                                                 
            }          
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(background, new Rectangle(0, 0, screenWidth, screenHeight), Color.White);
            if (!backToSystem) // if in combat state
            {
                spriteBatch.Draw(planet.Texture, new Vector2(0, 0), null, Color.White, 0, Vector2.Zero, planet.Scale, SpriteEffects.None, 0);
              
                if (planet.OverSized())
                {
                    bool isASun = (Planet.Name == "Sun" || Planet.Name == "Odin" || Planet.Name == "Ra");
                    if (!gameOver && !victory || isASun)
                    {
                        player.Draw(spriteBatch);
                        for (int i = 0; i < enemies.Count; i++)
                        {
                            enemies[i].Draw(spriteBatch);
                        }
                        foreach (Explosion explosion in explosions)
                            explosion.Draw(spriteBatch);
                        spriteBatch.Draw(HUD, new Rectangle(screenWidth/4, 9, 650, 50), Color.White);
                        spriteBatch.DrawString(smallerFont, "  Enemies " + planet.EnemyShips.ToString() +
                                                "   Current Wave: " + currentWave.ToString() +  "    Waves  " + waves.ToString()
                                                + "\n  Rockets:" + player.Ship.Rockets + "      Credits: " + player.Credits.ToString()
                                                + "    Experience: " + player.Experience.ToString(), 
                                                new Vector2(screenWidth / 3, 10), Color.GreenYellow);  
                                                                                
                    }
                    else if (victory)
                    {
                        planet.Conquered = true;            // says this planet has been conquered by the player
                        spriteBatch.DrawString(biggerFont, "Victory!", new Vector2(screenWidth / 2 - 60, screenHeight / 2), Color.Green);
                        spriteBatch.DrawString(biggerFont, "Press Escape to go back!", new Vector2(screenWidth / 3 - 60, screenHeight / 3), Color.White);
                    }
                    else if (gameOver)
                    {
                        spriteBatch.DrawString(biggerFont, "Game Over!", new Vector2(screenWidth / 2 - 60, screenHeight / 2), Color.Red);
                        spriteBatch.DrawString(biggerFont, "Press Escape to go back!", new Vector2(screenWidth / 3 - 60, screenHeight / 3), Color.White);
                    } 
                                        
                }
            }
            if (backToSystem)
                spriteBatch.Draw(planet.Texture, planet.InitialPosition, null, Color.White, 0, Vector2.Zero, planet.Scale, SpriteEffects.None, 0);
        }
        public void ManageExplosions()
        {
            for (int i = 0; i < explosions.Count; i++)
            {
                if (!explosions[i].IsVisible)
                {
                    explosions.RemoveAt(i);
                    i--;
                }
            }
        }
        public void Reset(EnfinitumAeon EA)
        {
            
            EA.QuitCombat = false;          
            victory = false;
            gameOver = false;
            
        }
        public void InitializeDifficulty()
        {
            if (planet.Level == 1)            
                limit = 6;                                     
            if (planet.Level == 2)            
                limit = 8;                      
            if (planet.Level == 3)            
                limit = 10;                        
        }
        public void InitializeEnemyShips()
        {

            int distance = 0;
            for (int j = 0; j < limit && j < planet.EnemyShips; j++)
            {
                Ship enemy = new Ship(Randomize(), "Enemy", graphics);
                distance += enemy.Rectangle.Height * 2;
                enemy.Rectangle = new Rectangle(enemy.Rectangle.X, distance, enemy.Rectangle.Width, enemy.Rectangle.Height + 5);
                enemy.healthBar.X = enemy.Rectangle.X;
                enemy.healthBar.Y = enemy.Rectangle.Y + enemy.Rectangle.Height;
                if (enemy.Name.Equals("Scout"))
                {

                    if (planet.Level == 1)
                    {
                        enemy.Interval = (float)rand.NextDouble() + rand.Next(3, 6);
                        enemy.FireRate = (float)rand.NextDouble() + rand.Next(2, 6);

                    }
                    if (planet.Level == 2)
                    {
                        enemy.Interval = (float)rand.NextDouble() + rand.Next(2, 5);
                        enemy.FireRate = (float)rand.NextDouble() + rand.Next(1, 5);
                    }

                    if (planet.Level == 3)
                    {
                        enemy.Interval = (float)rand.NextDouble() + rand.Next(2, 4);
                        enemy.FireRate = (float)rand.NextDouble() + rand.Next(1, 4);
                    }
                }
                if (enemy.Name.Equals("Interceptor"))
                {
                    if (planet.Level == 1)
                    {
                        enemy.Interval = (float)rand.NextDouble() + rand.Next(3, 6);
                        enemy.FireRate = (float)rand.NextDouble() + rand.Next(3, 6);

                    }
                    if (planet.Level == 2)
                    {
                        enemy.Interval = (float)rand.NextDouble() + rand.Next(3, 5);
                        enemy.FireRate = (float)rand.NextDouble() + rand.Next(2, 5);
                    }

                    if (planet.Level == 3)
                    {
                        enemy.Interval = (float)rand.NextDouble() + rand.Next(2, 5);
                        enemy.FireRate = (float)rand.NextDouble() + rand.Next(2, 4);
                    }
                }
                if (enemy.Name.Equals("Marauder"))
                {
                    if (planet.Level == 1)
                    {
                        enemy.Interval = (float)rand.NextDouble() + rand.Next(3, 6);
                        enemy.FireRate = (float)rand.NextDouble() + rand.Next(3, 6);

                    }
                    if (planet.Level == 2)
                    {
                        enemy.Interval = (float)rand.NextDouble() + rand.Next(2, 5);
                        enemy.FireRate = (float)rand.NextDouble() + rand.Next(3, 5);
                    }

                    if (planet.Level == 3)
                    {
                        enemy.Interval = (float)rand.NextDouble() + rand.Next(1, 4);
                        enemy.FireRate = (float)rand.NextDouble() + rand.Next(2, 4);
                    }
                }
                if (enemy.healthBar.Y + enemy.healthBar.Height >= screenHeight)
                {
                    waves++;
                    break;
                }
                else
                    enemies.Add(enemy);
            }
        }
        public Planet Planet
        {
            get { return planet; }
            set { planet = value; }
        }           
        public bool ExitCombat
        {
            get { return exitCombat; }
            set { exitCombat = value; }
        }
        public bool Victory
        {
            set { victory = value; }
        }
    }
}
