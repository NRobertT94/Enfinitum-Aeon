using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EnfinitumAeon2
{
    public class Player:GameSettings
    {
        private Ship ship;
        private int enfinitum, credits,experience, planetsConquered;
        private static int[] enfinitumNeeded = new int[] { 60, 140, 280 };     // enfinitum needed to complete each solar system

        public Player(GraphicsDeviceManager graphics)
            : base(graphics)
        {
            ship = new Ship("Fighter","Player",graphics);
            enfinitum = experience = 0;
            credits = 100;
            planetsConquered = 1;
           
            
        }
        public void LoadContent(ContentManager content)
        {
            ship.LoadContent(content);
        }
        public void Update(GameTime gameTime, ContentManager content, int attackMode)
        {
            ship.Update(gameTime, content);
          
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            ship.Draw(spriteBatch);
        }
        public Ship Ship
        {
            get { return ship; }
            set { ship = value; }
        }
        public int Credits
        {
            get {return credits;}
            set { credits = value; }
        }
        public int Experience
        {
            get { return experience; }
            set { experience = value; }
        }
        public int Enfinitum
        {
            get { return enfinitum; }
            set { enfinitum = value; }
        }
        public int PlanetsConquered
        {
            get { return planetsConquered; }
            set { planetsConquered = value; }
        }
        public int[] EnfinitumNeede
        {
            get { return enfinitumNeeded; }
        }     
        public void RestoreHealth()
        {
            ship.Destroyed = false;
            ship.Rectangle = new Rectangle(0, screenHeight / 2, ship.Rectangle.Width, ship.Rectangle.Height);
            ship.healthBar.X = ship.Rectangle.X;
            ship.healthBar.Y = ship.Rectangle.Y + ship.Rectangle.Height;
            ship.Defense = ship.Rectangle.Width;
            ship.healthBar.Width = ship.NewHealthBar;
            ship.InitialDefense = ship.Defense;
        }
        public void ResetPlayer() 
        {
            enfinitum = 0;
            credits = 400;
            Ship.Attack = 12;
            Ship.Defense = 60;
            Ship.Level = 1;
            planetsConquered = 1;
        }
    }
}
