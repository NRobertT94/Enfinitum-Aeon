using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;


namespace EnfinitumAeon2
{
    /**
     * This class contains the game states, screen and input settings as well as some 
     * graphics stuff like colors and fonts **/
    public class GameSettings
    {
        public int screenHeight;
        public int screenWidth;
        protected MouseState currentMouseState;
        protected MouseState previousMouseState;
        protected KeyboardState currentKeyboardState;
        protected KeyboardState previousKeyboardState;
        protected GraphicsDeviceManager graphics;
        public SpriteFont biggerFont;
        protected SpriteFont smallerFont;
        protected SpriteFont mediumFont;
        private SoundEffect clickedSound;
        protected GameState CurrentState;
        /// <summary>
        /// GameState is used to switch the content displayed on the screen.
        /// For example the first state will be MainMenu, if text New Game is clicked
        /// the game state will switch to NewGame state
        ///
        /// </summary>
        protected enum GameState
        {
            MainMenu,
            LoadGame,
            Level1,
            Level2,
            Level3,
            Combat,          
            Intro,
            Help,
            Shop,            
        }
        /// <summary>
        /// The constructor for the class 
        /// </summary>
        /// <param name="graphics"></param>
        public GameSettings(GraphicsDeviceManager graphics)
        {
            this.graphics = graphics;
            screenHeight = graphics.PreferredBackBufferHeight;
            screenWidth = graphics.PreferredBackBufferWidth;         
            
            InitCustomColors();
        }
        /// <summary>
        /// Accessor/Mutator for customColors
        /// </summary>
        /// <param name="?"></param>
        /// <returns></returns>

        public Dictionary<string, Color> customColor
        {
            get;
            set;
        }
        /// <summary>
        /// It loads all the fonts
        /// </summary>
        /// <param name="content"></param>
        public void LoadOptionsContent(ContentManager content)
        {
            biggerFont = content.Load<SpriteFont>("Fonts/BiggerFont");
            mediumFont = content.Load<SpriteFont>("Fonts/MediumFont");
            smallerFont = content.Load<SpriteFont>("Fonts/SmallerFont");
            clickedSound = content.Load<SoundEffect>("Sounds/hover");

        }
        /// <summary>
        /// Makes screen fullscreen
        /// </summary>          
        public void Fullscreen()
        {
            screenWidth = graphics.PreferredBackBufferWidth = 1376;
            screenHeight = graphics.PreferredBackBufferHeight = 768;
            graphics.IsFullScreen = true;
            graphics.ApplyChanges();
        }
        /// <summary>
        /// This is where you create custom colors
        /// </summary>      
        public void InitCustomColors()// Set these to what ever you want and add more
        {
            customColor = new Dictionary<string, Color>();
            customColor["highlighted"] = new Color(30, 144, 255); // sky blue
            customColor["white"] = Color.White;
            customColor["red"] = Color.Red;

        }
        /// <summary>
        /// Detects if you hover over a string in the game
        /// </summary>
        /// <param name="text"></param>
        /// <param name="position"></param>
        /// <param name="font"></param>
        /// <returns></returns>
        public bool HoveringOverText(String text, Vector2 position, SpriteFont font)
        {
            currentMouseState = Mouse.GetState();
            if (currentMouseState.X < position.X || currentMouseState.Y < position.Y ||
                currentMouseState.X > position.X + font.MeasureString(text).X ||
                currentMouseState.Y > position.Y + font.MeasureString(text).Y)
                return false;
            return true;
        }
        /// <summary>
        /// Detects if you click text on screen
        /// </summary>
        /// <param name="text"></param>
        /// <param name="position"></param>
        /// <param name="font"></param>
        /// <returns></returns>
        public bool TextClicked(String text, Vector2 position, SpriteFont font)
        {
            currentMouseState = Mouse.GetState();
            if (HoveringOverText(text, position, font) && currentMouseState.LeftButton == ButtonState.Pressed)                        
                return true;                          
            return false;
        }
        /// <summary>
        /// This is used to update input state
        /// WARNING do not delete anything form here
        /// the game will not detect mouse/keyboard states
        /// </summary>

        public void UpdateInputState()
        {
            previousMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();
            previousKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();
        }
        /// <summary>
        /// A very simple function that detects
        /// if you clicked. Why? Because there is no
        /// built in funtion to detect if the mouse was clicked
        /// </summary>
        /// <returns></returns>
        public bool Clicked()
        {
            if (previousMouseState.LeftButton == ButtonState.Released && currentMouseState.LeftButton == ButtonState.Pressed)
            {
                clickedSound.Play();
                return true;
            }              
            return false;
        }
        public bool RightClickPressed()
        {
            if (previousMouseState.RightButton == ButtonState.Released && currentMouseState.RightButton == ButtonState.Pressed)
                return true;
            return false;
        }
        public bool EscapePressed()
        {
            if (currentKeyboardState.IsKeyDown(Keys.Escape))
                return true;
            return false;
        }
        public bool F5Pressed()
        {
            if (currentKeyboardState.IsKeyDown(Keys.F5))
                return true;
            return false;
        }
        public bool HoverOverShip(Rectangle rectangle)
        {
            var mouseState = Mouse.GetState();
            var mousePosition = new Point(mouseState.X, mouseState.Y);
            if (new Rectangle(mouseState.X, mouseState.Y, 1, 1).Intersects(rectangle))
                return true;
            return false;
        }


    }
}
