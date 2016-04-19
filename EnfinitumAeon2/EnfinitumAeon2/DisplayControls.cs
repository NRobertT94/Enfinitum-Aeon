using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EnfinitumAeon2
{
    public class DisplayControls:GameSettings
    {
        private Texture2D background;
        private Vector2 backButton;
        private bool backClicked;
        public DisplayControls(GraphicsDeviceManager graphics):base(graphics)
        {
            backButton = new Vector2(screenWidth / 20, screenHeight - 100);
            backClicked = false;

        }
        public void LoadContent(ContentManager content)
        {
            LoadOptionsContent(content);
            background = content.Load<Texture2D>("Backgrounds/controls");
        }
        public void Update()
        {
            if (TextClicked("Back", backButton, mediumFont))           
                backClicked = true;
            
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(background, new Rectangle(0, 0, screenWidth, screenHeight), Color.White);
            spriteBatch.DrawString(mediumFont, "Move UP - W or Up Arrow\n" +
                                               "Move DOWN - S or Down Arrow\n" +
                                               "Move LEFT - A or Left Arrow\n" +
                                               "Move RIGHT - D or Right Arrow\n\n" +
                                               "Shoot Bullets - SPACE or Left Click\n" +
                                               "Shoot Rocket - ALT or Right Click\n", new Vector2(screenWidth / 3, screenHeight / 4), Color.White);
            spriteBatch.DrawString(mediumFont, "Back", backButton, Color.White);
        }
        public bool BackClicked
        {
            get { return backClicked; }
            set { backClicked = value; }
        }
    }
}
