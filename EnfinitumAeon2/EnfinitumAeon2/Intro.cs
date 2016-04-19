using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EnfinitumAeon2
{
    public class Intro:GameSettings
    {
      
        private Texture2D texture;
        private float time;       
        private bool textShown;
        private bool exit;
        private bool textFaded;
        private bool logoShown;
        private bool logoFaded;
        private Color textColor;
        private Color logoColor;
        public Intro(GraphicsDeviceManager graphics):base(graphics)
        {
            time = 0;          
            exit = false;
            textShown = false;
            textFaded = false;
            logoFaded = false;
            logoShown = false;
            textColor = logoColor = new Color(0, 0, 0);
            
        }
        public void LoadContent(ContentManager content)
        {
            texture = content.Load<Texture2D>("Fonts/logo");
            LoadOptionsContent(content);
        }
        public void Update(GameTime gameTime)
        {
            UpdateInputState();
            time += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (time > 30f || EscapePressed())
                exit = true;  
            if (!exit)
            {
                if (!textFaded)
                {
                    if (textColor.R != 255 && !textShown)
                    {
                        textColor.R++;
                        textColor.G++;
                        textColor.B++;
                    }
                    else
                        textShown = true;
                    if (textShown && time > 10f)
                    {
                        textColor.R--;
                        textColor.G--;
                        textColor.B--;
                        if (textColor.R == 0)
                        {
                            textFaded = true;
                        }
                    }                  
                }
                if (textFaded)
                {
                    if (logoColor.R != 255 && !logoShown)
                    {
                        logoColor.R++;
                        logoColor.G++;
                        logoColor.B++;
                    }
                    else
                        logoShown = true;
                    if (logoShown)
                    {
                        logoColor.R--;
                        logoColor.G--;
                        logoColor.B--;
                        if (logoColor.R == 0)
                            logoFaded = true;
                    }                  
                }
                if (logoFaded)
                    exit = true;
               // if(textFaded && logoColor.R==255)
               
            }
              
           
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            graphics.GraphicsDevice.Clear(Color.Black);
            if (!exit)
            {
                
                spriteBatch.DrawString(mediumFont, "Civilisation...What is civilisation?\nA stage of technical and cultural prosperity?\n"+
                                                   "Or an illusion of comfort that lasts maybe a long period of time\n"+
                                                   "and when its pillars start to corrode a shroud of darkness awaits \n"+
                                                   "to take over, leaving billions of lives in despair?\n",                                               
                                                    new Vector2(screenWidth / 6, screenHeight / 3), textColor);
                if(textFaded)
                   spriteBatch.Draw(texture, new Rectangle(screenWidth / 4, screenHeight / 3, 800, 100), logoColor);                                                                                               
            }                 
        }
        public bool Exit
        {
            get { return exit; }
            set { exit = value; }
        }
    }
}
