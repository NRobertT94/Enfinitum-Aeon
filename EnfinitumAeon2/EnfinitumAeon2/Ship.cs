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


    public class Ship : GameSettings
    {
        private Texture2D texture;
        private Texture2D healthTexture;
        private Rectangle rectangle;
        public Rectangle healthBar;
        public int newHealthBar;
        private List<Munition> bullets;
        private Vector2 speed;
        private Color nameColor;    
        private SoundEffect shot;
        private SoundEffect boom;
        private SoundEffect damage;
        private int attack;
        private int defense;
        private int level;
        private int moveSpeed;
        private int attackMode;
        private int price;
        private int rockets;
        private int initialDefense;
        private int credits;
        private int experience;
        private float time;
        private float fireRate;
        private float interval;
        private float rocketReload;
        private float attackModeTime;
        private string name;
        private string type;
        private bool destroyed;
        private bool hit;
        /// <summary>
        /// Constructor creates a ship with features that corespond with its name
        /// </summary>
        /// <param name="name"></param>
        /// <param name="graphics"></param>
        public Ship(string name, string type, GraphicsDeviceManager graphics)
            : base(graphics)
        {
            this.name = name;
            this.type = type;
            if (name.Equals("Fighter"))
            {
                rectangle = new Rectangle(0, screenHeight / 2, 50, 30);
                healthBar = new Rectangle(0, rectangle.Y + rectangle.Height + 5, rectangle.Width, 5);
                newHealthBar = healthBar.Width;
                speed = new Vector2(3, 3);
                price = 200;
                rockets = 1;

            }
            if (name.Equals("Destroyer"))
            {
                rectangle = new Rectangle(screenWidth - 40, screenHeight / 2, 100, 80);
                healthBar = new Rectangle(rectangle.X, rectangle.Y + rectangle.Height + 5, rectangle.Width, 5);
                newHealthBar = healthBar.Width;
                speed = new Vector2(2, 2);
                price = 600;
                rockets = 5;
            }
            if (name.Equals("Corvette"))
            {
                rectangle = new Rectangle(screenWidth - 40, screenHeight / 2, 80, 70);
                healthBar = new Rectangle(rectangle.X, rectangle.Y + rectangle.Height + 5, rectangle.Width-20, 5);
                newHealthBar = healthBar.Width;
                speed = new Vector2(3, 3);
                price = 400;
                rockets = 3;
            }
            if (name.Equals("Scout"))
            {
                rectangle = new Rectangle(screenWidth - 40, screenHeight / 2, 40, 18);
                healthBar = new Rectangle(rectangle.X, rectangle.Y + rectangle.Height + 5, rectangle.Width, 5);
                speed = new Vector2(2, 2);
                credits = 5;
                experience = 10;
            }
            if (name.Equals("Interceptor"))
            {
                rectangle = new Rectangle(screenWidth - 40, screenHeight / 2, 80, 60);
                healthBar = new Rectangle(rectangle.X, rectangle.Y + rectangle.Height + 5, rectangle.Width, 5);
                speed = new Vector2(1, 1);
                credits = 10;
                experience = 20;
            }
            if (name.Equals("Marauder"))
            {
                rectangle = new Rectangle(screenWidth - 40, screenHeight / 2, 110, 60);
                healthBar = new Rectangle(rectangle.X, rectangle.Y + rectangle.Height + 5, rectangle.Width, 5);
                speed = new Vector2(1, 1);
                credits = 20;
                experience = 30;
            }
            defense = healthBar.Width;
            attack = rectangle.Width / 5;
            level = 1;
            nameColor = Color.White;
            bullets = new List<Munition>();
            time = 0;
            destroyed = false;
            hit = false;
            attackModeTime = 0;
            interval = 0;
            rocketReload = 0;
            initialDefense = defense;
        }
        /// <summary>
        /// Calculates the level of this Ship based on the attack and defence stats.
        /// Level calculated by the average of _attack and _defence and adjusted by a small amount 
        /// if the difference between the two values is sufficiently large.
        /// </summary>

        public void LoadContent(ContentManager content)
        {
            LoadOptionsContent(content);
            texture = content.Load<Texture2D>("Ships/" + name);
            if (type.Equals("Player"))
            {
                shot = content.Load<SoundEffect>("Sounds/PlayerShot");
                healthTexture = content.Load<Texture2D>("Ships/playerHealth");
                boom = content.Load<SoundEffect>("Sounds/InterceptorDestroyed");
            }                      
            if (name.Equals("Scout"))
            {
                shot = content.Load<SoundEffect>("Sounds/ScoutShot");
                healthTexture = content.Load<Texture2D>("Ships/enemyHealth");
                boom = content.Load<SoundEffect>("Sounds/ScoutDestroyed");
            }
            if (name.Equals("Interceptor"))
            {
                shot = content.Load<SoundEffect>("Sounds/InterceptorShot");
                healthTexture = content.Load<Texture2D>("Ships/enemyHealth");
                boom = content.Load<SoundEffect>("Sounds/InterceptorDestroyed");
            }
            if (name.Equals("Marauder"))
            {
                shot = content.Load<SoundEffect>("Sounds/MarauderShot");
                healthTexture = content.Load<Texture2D>("Ships/enemyHealth");
                boom = content.Load<SoundEffect>("Sounds/MarauderDestroyed");
            }
            damage = content.Load<SoundEffect>("Sounds/damage");
                
           
        }
        //updates coresponding ship
        public void Update(GameTime gameTime, ContentManager content)
        {
            UpdateInputState();
            time += (float)gameTime.ElapsedGameTime.TotalSeconds;
            rocketReload += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (type.Equals("Player") && !destroyed)
            {
                MovePlayer();
                if (time > 0.2f && (currentKeyboardState.IsKeyDown(Keys.Space) || currentMouseState.LeftButton == ButtonState.Pressed))
                {
                    Shoot(content, "Bullets");                    
                    time = 0;
                }
                if (rocketReload > 4f && rockets > 0 && (currentKeyboardState.IsKeyDown(Keys.LeftAlt) || currentMouseState.RightButton == ButtonState.Pressed))
                {
                    Shoot(content, "Retaliator");                  
                    rockets--;
                    rocketReload = 0;
                }
            }
            if (type.Equals("Enemy") && !destroyed)
            {
                attackModeTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (attackModeTime <= interval)
                {
                    attackMode = 1;
                }
                if (attackModeTime > interval)
                {
                    attackMode = 2;
                }
                if (attackModeTime > interval*2)
                {
                    attackMode = 3;
                }
                if (attackModeTime > interval * 3)
                    attackModeTime = 0;
                             
                if (time > fireRate)
                {
                    Shoot(content,"Bullets");                              
                    time = 0;
                }
                MoveEnemy(attackMode);
            }                     
            UpdateBullet();

            //if(type.Equals("Enemy"))         

        }
        public void Draw(SpriteBatch spriteBatch)
        {
            if (!destroyed)
            {
                spriteBatch.Draw(texture, rectangle, Color.White);
                spriteBatch.Draw(healthTexture, healthBar, Color.White);            
            }
            foreach (Munition bullet in bullets)
                bullet.Draw(spriteBatch);                 
        }
        public void MovePlayer()
        {

            if ((currentKeyboardState.IsKeyDown(Keys.Down) || currentKeyboardState.IsKeyDown(Keys.S)) && healthBar.Y <= screenHeight - healthBar.Height)
            {
                rectangle.Y += (int)speed.Y + moveSpeed;
                healthBar.Y += (int)speed.Y + moveSpeed;
            }

            if ((currentKeyboardState.IsKeyDown(Keys.Up) || currentKeyboardState.IsKeyDown(Keys.W)) && rectangle.Y >= 0)
            {
                rectangle.Y -= (int)speed.Y + moveSpeed;
                healthBar.Y -= (int)speed.Y + moveSpeed;
            }
            if ((currentKeyboardState.IsKeyDown(Keys.Right) || currentKeyboardState.IsKeyDown(Keys.D)) && rectangle.X + rectangle.Width <= screenWidth)
            {
                rectangle.X += (int)speed.X + moveSpeed;
                healthBar.X += (int)speed.X + moveSpeed;
            }
            if ((currentKeyboardState.IsKeyDown(Keys.Left) || currentKeyboardState.IsKeyDown(Keys.A)) && rectangle.X >= 0)
            {
                rectangle.X -= (int)speed.X + moveSpeed;
                healthBar.X -= (int)speed.X + moveSpeed;
            }
        }
        // Enemy movement AI
        public void MoveEnemy(int mode)
        {
            if (mode == 1)
            {
                rectangle.X -= (int)speed.X;
                rectangle.Y += (int)speed.Y;
                healthBar.X -= (int)speed.X;
                healthBar.Y += (int)speed.Y;             
            }
            if (mode == 2)
            {
                rectangle.X -= (int)speed.X;
                rectangle.Y -= (int)speed.Y;
                healthBar.X -= (int)speed.X;
                healthBar.Y -= (int)speed.Y;              
            }
            if (mode == 3)
            {
                rectangle.X += (int)speed.X;
                rectangle.Y += (int)speed.Y;
                healthBar.X += (int)speed.X;
                healthBar.Y += (int)speed.Y; 
            }
            if (rectangle.Y + healthBar.Height >= screenHeight - rectangle.Height)
                speed = -speed;
            if (rectangle.Y + rectangle.Height <= rectangle.Height)
                speed = -speed;
            if (rectangle.X  >= screenWidth)
                speed.X = -speed.X;
            if (rectangle.X - rectangle.Width <= 0)
                speed.X = -speed.X;

        }
        //Bullet characteristics for each ships
        public void Shoot(ContentManager content,string ammo)
        {         
            if (ammo.Equals("Bullets"))
            {
                string color = "";
                if (type.Equals("Player"))
                    color = "greenBullet";
                if (type.Equals("Enemy"))
                    color = "redBullet";
                Munition bullet = new Munition(content.Load<Texture2D>("Ships/" + color));
               
                bullet.name = "Bullet";
                bullet.isVisible = true;
                if (name.Equals("Fighter") || name.Equals("Scout"))
                {
                   
                    bullet.size = new Vector2(20, 5);
                }
                else if (name.Equals("Corvette") || name.Equals("Interceptor"))
                {
                   
                    bullet.size = new Vector2(35, 7);
                }
                else if (name.Equals("Destroyer") || name.Equals("Marauder"))
                {
                   
                    bullet.size = new Vector2(45, 8);
                }
                bullet.velocity = 7f;
                if (type.Equals("Player") || type.Equals("Ally"))
                    bullet.position = new Vector2(rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height / 2);
                else
                    bullet.position = new Vector2(rectangle.X, rectangle.Y + rectangle.Height / 2);
                if (bullets.Count < 10)
                {
                    shot.Play();
                    bullets.Add(bullet);
                }
                   
            }
            if (ammo.Equals("Retaliator"))
            {
                Munition retaliator = new Munition(content.Load<Texture2D>("Ships/retaliator"));
                shot.Play();
                retaliator.name = "Retaliator";
                retaliator.isVisible = true;
                retaliator.velocity = 13f;
                retaliator.size = new Vector2(100, 10);
                retaliator.position = new Vector2(rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height / 2);
                if (bullets.Count <10)
                    bullets.Add(retaliator);
            }
           
           
        }     
        //this moves the bullet and removes it from the list if it's out of screen range
        public void UpdateBullet()
        {
            foreach (Munition bullet in bullets)
            {
                if (name.Equals("Fighter") || name.Equals("Corvette") || name.Equals("Destroyer"))
                {
                    bullet.position.X += bullet.velocity;
                    if (bullet.position.X >= screenWidth)
                        bullet.isVisible = false;
                }

                if (name.Equals("Scout") || name.Equals("Interceptor") || name.Equals("Marauder"))
                {
                    bullet.position.X -= bullet.velocity;
                    if (bullet.position.X <= 0)
                        bullet.isVisible = false;
                }
                bullet.UpdateBullet();
            }
            for (int i = 0; i < bullets.Count; i++)
            {
                if (!bullets[i].isVisible)
                {
                    bullets.RemoveAt(i);
                    i--;
                }
            }
        }
        //here it decreases the health by the size of the bullets Width/2
        public void TakesDamageFrom(Ship ship, ContentManager content)
        {
            if (defense <= 0 || healthBar.Width <= 0)
            {                       
                destroyed = true;
                if (defense < 0)
                    defense = 0;
            }               
            if (rectangle.Intersects(ship.rectangle))
            {
                defense -= ship.defense;
                ship.defense -= defense;
                healthBar.Width = defense;
                ship.healthBar.Width = defense;
            }
            foreach (Munition ammo in ship.bullets)
            {
                if (ammo.rectangle.Intersects(rectangle))
                {
                    if (ammo.name.Equals("Bullet"))
                    {
                        defense -= ship.attack;
                        healthBar.Width = defense;
                        hit = true;
                        ammo.isVisible = false;
                    }
                    if (ammo.name.Equals("Retaliator"))
                    {
                        defense -= 50;
                        healthBar.Width = defense;
                        hit = true;
                        ammo.isVisible = false;
                    }
                   
                }
            }

        }
        //detect if mouse is over ship
        public bool HoveringOverShip()
        {
            currentMouseState = Mouse.GetState();
            var spriteLocation = new Rectangle((int)rectangle.X, (int)rectangle.Y, (int)rectangle.Width, (int)rectangle.Height);
            bool mouseOver = spriteLocation.Contains(currentMouseState.X, currentMouseState.Y);
            if (mouseOver)
                return true;
            return false;
        }
        public int Attack
        {
            get { return attack; }
            set { attack = value; }
        }
        public int Defense
        {
            get { return defense; }
            set { defense = value; }
        }
        public int Level
        {
            get
            {
                return level;
            }
            set { level = value; }
        }
        public Rectangle HealthBar
        {
            get { return healthBar; }
            set { healthBar = value; }
        }
        public Rectangle Rectangle
        {
            get { return rectangle; }
            set { rectangle = value; }
        }
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public Color NameColor
        {
            get { return nameColor; }
            set { nameColor = value; }
        }
        public bool Destroyed
        {
            get { return destroyed; }
            set { destroyed = value; }
        }
        public int MoveSpeed
        {
            get { return moveSpeed; }
            set { moveSpeed = value; }
        }
        public float FireRate
        {
            get { return fireRate; }
            set { fireRate = value; }
        }
        public SoundEffect Boom
        {
            get { return boom; }
        }
        public SoundEffect Damage
        {
            get { return damage; }
        }
        public float Interval
        {
            get { return interval; }
            set { interval = value; }
        }
        public float AttackModeTime
        {
            get { return attackModeTime; }
            set { attackModeTime = value; }
        }
        public bool Hit
        {
            get { return hit; }
            set { hit = value; }
        }
        public int Type
        {
            get
            {
                if (Name.Equals("Fighter"))
                    return 1;
                else if (Name.Equals("Corvette"))
                    return 2;
                else
                    return 3;
            }
        }
        public void deleteFromScreen()
        {
            foreach (Munition bullet in bullets)
            {
                bullet.isVisible = false;
            }

        }
        public int Price
        {
            get { return price; }
            set { price = value; }
        }
        public int Credits
        {
            get { return credits; }
            set { credits = value; }
        }
        public int Experience
        {
            get { return experience; }
            set { experience = value; }
        }
        public int Rockets
        {
            get { return rockets; }
            set { rockets = value; }
        }
        public int InitialDefense
        {
            get { return initialDefense; }
            set { initialDefense = value; }
        }

        public int NewHealthBar 
        {
            get { return newHealthBar; }
            set { newHealthBar = value; }
        }
    }
}
