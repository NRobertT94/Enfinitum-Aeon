
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace EnfinitumAeon2
{
    /// <summary>
    /// This class displays the main menu with the buttons
    /// </summary>
    public class MainMenu : GameSettings
    {
       
        private Texture2D background,helpBackground;
        private List<string> menuItems = new List<string>();
        private List<Vector2> position = new List<Vector2>();
        private List<Color> itemsColor = new List<Color>();
       
        private bool help;
        /// <summary>
        /// Constructor where the position of the buttons are set
        /// playOnce is a bool that lets a sound play once when hovering 
        /// over a menu button
        /// </summary>
        /// <param name="graphics"></param>
        public MainMenu(GraphicsDeviceManager graphics)
            : base(graphics)
        {         
            help = false;
            InitMainMenu();
        }
        /// <summary>
        /// Loads the fonts from GameOptions,
        /// background texture, and sound
        /// </summary>
        /// <param name="content"></param>
        public void LoadContent(ContentManager content)
        {
            LoadOptionsContent(content);
            helpBackground = content.Load<Texture2D>("Backgrounds/image2");
            background = content.Load<Texture2D>("Backgrounds/MenuBackground");
           
        }
        /// <summary>
        /// This detects hover, and changes colors of the buttons
        /// also it plays the hover sound 
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="game"></param>
        public void Update(GameTime gameTime, Main game)
        {
            for (int i = 0; i < menuItems.Count; i++)
            {
                if (HoveringOverText(menuItems[i], position[i], biggerFont))
                {
                    itemsColor[i] = customColor["highlighted"]; // set to deep sky blue if hovering over text
                    /* if (!playOnce)            //!FIX! sound plays only once if hovered over a text
                         hoverSound.Play();    //no solution found till now to prevent sound playing forever
                     playOnce = true; */
                    // if commented out, hover song will play forever when you hover over button

                }
                else
                {
                    itemsColor[i] = customColor["white"];// set to white                    
                }
            }
        }
        /// <summary>
        /// Draws background and the text/buttons
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch)
        {
           
                spriteBatch.Draw(background, new Rectangle(0, 0, screenWidth, screenHeight), customColor["white"]);
                for (int i = 0; i < menuItems.Count; i++)
                {
                    spriteBatch.DrawString(biggerFont, menuItems[i], position[i], itemsColor[i]);
                }
            
        }
        /// <summary>
        /// Initializes menu items and positions
        /// </summary>
        public void InitMainMenu()
        {
            int spaceBetween = 0;
            menuItems.AddRange(new string[] { "New Game", "Load Game", "Help", "Exit" });
            for (int i = 0; i < menuItems.Count; i++)
            {
                position.Add(new Vector2(screenWidth / 2 - 60, 200 + spaceBetween));
                itemsColor.Add(new Color(255, 255, 255));
                spaceBetween += 55;
            }
        }
        /// <summary>
        /// Detects if NewGame is clicked 
        /// </summary>
        /// <returns></returns>
        public bool ButtonClicked(string name)
        {
            for (int i = 0; i < menuItems.Count; i++)
                if (menuItems[i].Equals(name) && TextClicked(menuItems[i], position[i], biggerFont))
                    return true;
            return false;
        }

        public bool Help 
        {
            get { return help; }
            set { help = value; }
        }
        
    }
}
