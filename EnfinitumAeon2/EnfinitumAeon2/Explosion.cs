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
    
    public class Explosion
    {
        private Texture2D texture;
        private Rectangle rectangle;
        private Ship ship; 
        private int currentFrame;
        private Vector2 animationFrame;
        private Vector2 origin;
        private float time, interval;
        private bool isVisible;       
        private int XPos;
        private int YPos;
        private string type;
        private Random rand;
        private int dmgXPos;
        private int dmgYPos;


        public Explosion(ContentManager content,Ship ship,string type)
        {
            texture = content.Load<Texture2D>("Ships/explosion3");                
            this.ship = ship;
            currentFrame = 1;
            interval = 60f;
            time = 0;
            isVisible = true;
            animationFrame = new Vector2(222,222);        
            XPos = YPos = 0;
            this.type = type;
            rand = new Random();
            dmgXPos = ship.Rectangle.X + rand.Next(0, ship.Rectangle.Width);
            dmgYPos = ship.Rectangle.Y + rand.Next(0, ship.Rectangle.Height);
            
        }       
        public void Update(GameTime gameTime)
        {
           
                 time += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                 if (time > interval)
                 {                   
                     currentFrame++;
                     time = 0f;
                 }
                    
                 if (currentFrame == 48)
                 {
                     isVisible = false;
                     currentFrame = 0;
                 }

                 if (currentFrame % 8 == 0)
                 {
                     XPos = 0;
                     YPos+=(int)animationFrame.Y;
                     rectangle = new Rectangle((int)animationFrame.X * currentFrame, YPos, (int)animationFrame.X, (int)animationFrame.Y);
                 }
                 else
                     rectangle = new Rectangle((int)animationFrame.X * currentFrame, YPos, (int)animationFrame.X, (int)animationFrame.Y);
                     
                 origin = new Vector2(rectangle.Width / 2, rectangle.Height / 2);
                                    
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            if (isVisible && type.Equals("Destroyed"))
                spriteBatch.Draw(texture,new Rectangle(ship.Rectangle.X,ship.Rectangle.Y,ship.Rectangle.Width*3,ship.Rectangle.Height*4),rectangle,Color.White,0f,origin,SpriteEffects.None,0);
            if (isVisible && type.Equals("Damage"))
                spriteBatch.Draw(texture, new Rectangle(dmgXPos, dmgYPos, ship.Rectangle.Width, ship.Rectangle.Height),                                                                                                              
                                                        rectangle, Color.White, 0f, origin, SpriteEffects.None, 0);
        }
        public bool IsVisible
        {
            get { return isVisible; }
            set { isVisible = value; }
        }
    }
}
