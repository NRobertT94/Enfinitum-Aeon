
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
    /// This is where everything is binded together
    /// </summary>
    public class EnfinitumAeon : GameSettings
    {
        private Intro intro;
        private MainMenu mainMenu;
        private DisplayControls controls;
        private SolarSystem solarSystem1;     //level 1 solar system
        private SolarSystem solarSystem2;     //level 2 solar system
        private SolarSystem solarSystem3;     //level 3 solar system
        private SolarSystem currentSolarSystem;
        private string[] gameStates = new string[] { "Level 1", "Level 2", "Level 3","Shop", "Main Menu" }; // this will be displayed in each solar system as text/buttons
        private Vector2[] gameStatesPosition = new Vector2[5];  // position for each button       
        private Color[] levelColor = new Color[5];   // this will be used to change the colors for buttons 
        private Player player;
        private Shop shop;
        protected Song introSong;
        protected Song combatSong;
        private int currentSong;
        private Combat combat;
        private bool quitCombat;
        private Save thisGame;
        public bool noSaveFound;
         
        /// <summary>
        /// The constructor everything is initialized here 
        /// and starts in MainMenu state
        /// </summary>
        /// <param name="graphics"></param>
        public EnfinitumAeon(GraphicsDeviceManager graphics)
            : base(graphics)
        {
            intro = new Intro(graphics);
            mainMenu = new MainMenu(graphics);
            controls = new DisplayControls(graphics);
            player = new Player( graphics);
            thisGame = new Save(graphics);
            noSaveFound = false;
            solarSystem1 = new SolarSystem(player, 1, graphics, thisGame);  // level 1 sol sys           
            solarSystem2 = new SolarSystem(player, 2, graphics, thisGame);  // level 2 sol sys
            solarSystem3 = new SolarSystem(player, 3, graphics, thisGame);  // level 3 sol sys
            thisGame.updateSaveParams(player, solarSystem1, solarSystem2, solarSystem3); 
            SolarSystemTestModeInit();
            shop = new Shop(4, player, graphics);
            currentSong = 1;
            CurrentState = GameState.Intro;
            combat = new Combat(solarSystem1.Planets[1],player,graphics);
            quitCombat = false;
      
                 
        }
        /// <summary>
        /// Loads content for the stuff initialized in the constructor as well as fonts from Options
        /// 
        /// </summary>
        /// <param name="content"></param>
        public void LoadContent(ContentManager content)
        {
            LoadOptionsContent(content);
            intro.LoadContent(content);
            controls.LoadContent(content);
            mainMenu.LoadContent(content);
            player.LoadContent(content);
            solarSystem1.LoadContent(content);
            solarSystem2.LoadContent(content);
            solarSystem3.LoadContent(content);     
            shop.LoadContent(content);
            combatSong = content.Load<Song>("Sounds/combatSong");
            introSong = content.Load<Song>("Sounds/intro");
            combat.LoadContent(content);
            if (currentSong == 1 && MediaPlayer.State == MediaState.Stopped)
            {
                MediaPlayer.Play(introSong);
            }
         
        }
        /// <summary>
        /// Updates items depending on state
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="game"></param>
        public void Update(GameTime gameTime, Main game)
        {
            switch (CurrentState)
            {
                case GameState.Intro:
                    intro.Update(gameTime);
                    if (intro.Exit)
                        CurrentState = GameState.MainMenu;
                    break;
                case GameState.MainMenu:
                    mainMenu.Update(gameTime, game);
                    currentSong = 1;
                    if (mainMenu.ButtonClicked("New Game"))
                    {
                        noSaveFound = false;
                        player.ResetPlayer();                               // resets all stats and attributes
                        solarSystem1.ResetSolarSystem(1);
                        solarSystem2.ResetSolarSystem(2);
                        solarSystem3.ResetSolarSystem(3);
                        combat.Reset(this);
                        CurrentState = GameState.Level1;
                    }
                    else if (mainMenu.ButtonClicked("Load Game"))           // calls Save functionality to load back a saved game state
                    {
                        if (thisGame.loadGame())
                        {
                            player = thisGame.Player;
                            SolarSystem[] restore = thisGame.SolarSystems;  // retrieves the array of all 3 solar systems
                            solarSystem1 = restore[0];
                            solarSystem2 = restore[1];
                            solarSystem3 = restore[2];
                            thisGame.updateSaveParams(player, solarSystem1, solarSystem2, solarSystem3);
                            CurrentState = GameState.Level1;
                        }
                        else
                            noSaveFound = true;
                    }
                    else if (mainMenu.ButtonClicked("Help")) 
                    {
                        noSaveFound = false;
                        mainMenu.Help = true;
                        if (mainMenu.Help)
                        {
                            controls.BackClicked = false;
                            CurrentState = GameState.Help;
                        }
                           
                        
                    }
                    else if (mainMenu.ButtonClicked("Exit"))
                    {
                        noSaveFound = false;
                        game.Exit();
                    }
                    break;
                case GameState.Help:
                    controls.Update();
                    if (controls.BackClicked)
                        CurrentState = GameState.MainMenu;
                    break;
                case GameState.Level1:
                    solarSystem1.Update(gameTime, game.Content);                
                    currentSolarSystem = solarSystem1;
                    currentSong = 1;     // sets this to be the current solar system                                               
                    SolarSystemTestModeUpdate(1, smallerFont);
                    if (solarSystem1.CombatMode)
                    {                      
                        combat = new Combat(solarSystem1.SelectedPlanet, player, graphics);                      
                        combat.LoadContent(game.Content);
                        solarSystem1.CombatMode = false;
                        combat.Reset(this);
                        if (solarSystem1.SelectedPlanet.Conquered)
                            combat.Victory = true;
                        CurrentState = GameState.Combat;
                    }                                                              
                    break;
                case GameState.Level2:
                    solarSystem2.Update(gameTime, game.Content);
                    currentSong = 1;
                    currentSolarSystem = solarSystem2;                
                    SolarSystemTestModeUpdate(2, smallerFont);
                    if (solarSystem2.CombatMode)
                    {
                        combat = new Combat(solarSystem2.SelectedPlanet, player, graphics);
                        combat.LoadContent(game.Content);                    
                        solarSystem2.CombatMode = false;
                        combat.Reset(this);
                        if (solarSystem2.SelectedPlanet.Conquered)
                            combat.Victory = true;
                        CurrentState = GameState.Combat;
                    }                 
                    break;
                case GameState.Level3:
                     solarSystem3.Update(gameTime,game.Content);
                     currentSong = 1;
                     currentSolarSystem = solarSystem3;                  
                     SolarSystemTestModeUpdate(3, smallerFont);
                     if (solarSystem3.CombatMode)
                     {
                         combat = new Combat(solarSystem3.SelectedPlanet, player, graphics);
                         combat.LoadContent(game.Content);                 
                         solarSystem3.CombatMode = false;
                         combat.Reset(this);
                         if (solarSystem3.SelectedPlanet.Conquered)
                             combat.Victory = true;
                         CurrentState = GameState.Combat;
                     }                      
                     break;
                case GameState.Combat:
                     currentSong = 2;
                     combat.Update(this,gameTime,game.Content);
                     if (quitCombat && combat.Planet.Level == 1)                                            
                         CurrentState = GameState.Level1;
                     if (quitCombat && combat.Planet.Level == 2)
                         CurrentState = GameState.Level2;
                     if (quitCombat && combat.Planet.Level == 3)
                         CurrentState = GameState.Level3;
                     break;
                case GameState.Shop:
                     shop.Update(gameTime, game.Content);
                     if (shop.backButtonClicked())
                         CurrentState = GameState.Level1;
                     else if (shop.EscapePressed())
                         CurrentState = GameState.Shop;
                     SolarSystemTestModeUpdate(4, smallerFont);
                     break;
            }              
        }
        public void Draw(SpriteBatch spriteBatch)
        {
           
            switch (CurrentState)
            {
                case GameState.Intro:
                    intro.Draw(spriteBatch);
                    break;
                case GameState.MainMenu:
                    mainMenu.Draw(spriteBatch);
                    break;
                case GameState.Help:
                    controls.Draw(spriteBatch);
                    break;
                case GameState.Level1:
                    solarSystem1.Draw(spriteBatch);                 
                    SolarSystemTestModeDraw(spriteBatch);
                    break;
                case GameState.Level2:
                    solarSystem2.Draw(spriteBatch);                  
                    SolarSystemTestModeDraw(spriteBatch);
                    break;
                case GameState.Level3:                 
                    solarSystem3.Draw(spriteBatch);                                  
                    SolarSystemTestModeDraw(spriteBatch);
                    break;   
                case GameState.Shop:
                    shop.Draw(spriteBatch);
                    break;   
                case GameState.Combat:
                    combat.Draw(spriteBatch);
                    break;
            }
            if(noSaveFound)
                spriteBatch.DrawString(mediumFont, "No save file found", new Vector2(30, 30), Color.White);
        }
        /// <summary>
        /// Here I create a sample of ally and enemy ships
        /// </summary>      
        /// <summary>
        ///  Test mode will draw the levels and combat system state button   
        /// </summary>
        public void SolarSystemTestModeInit()
        {
            int spaceBetween = 0;
            for (int i = 0; i < gameStates.Length; i++)
            {
                gameStatesPosition[i] = new Vector2(screenWidth / 15, screenHeight / 15 + spaceBetween);
                levelColor[i] = customColor["white"];
                spaceBetween += 45;
            }
        }
        /// <summary>
        /// Detecs which button you click and switches to that level or combat
        /// </summary>
        /// <param name="level"></param>
        /// <param name="defaultFont"></param>
        public void SolarSystemTestModeDraw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < gameStates.Length; i++)
                spriteBatch.DrawString(mediumFont, gameStates[i], gameStatesPosition[i], levelColor[i]);
        }
        /// <summary>
        /// Detecs which button you click and switches to that level or combat
        /// </summary>
        /// <param name="level"></param>
        /// <param name="defaultFont"></param>
        public void SolarSystemTestModeUpdate(int level, SpriteFont defaultFont)
        {
            if (level == 1)
            {
                if (TextClicked(gameStates[1], gameStatesPosition[1], defaultFont))
                    CurrentState = GameState.Level2;
                else if (TextClicked(gameStates[2], gameStatesPosition[2], defaultFont))
                    CurrentState = GameState.Level3;
                else if (TextClicked(gameStates[3], gameStatesPosition[3], defaultFont))
                    CurrentState = GameState.Shop;
                else if (TextClicked(gameStates[4], gameStatesPosition[4], defaultFont))
                    CurrentState = GameState.MainMenu;
              
            }
            else if (level == 2)
            {
                if (TextClicked(gameStates[0], gameStatesPosition[0], defaultFont))
                    CurrentState = GameState.Level1;
                else if (TextClicked(gameStates[2], gameStatesPosition[2], defaultFont))
                    CurrentState = GameState.Level3;            
                else if (TextClicked(gameStates[3], gameStatesPosition[3], defaultFont))
                    CurrentState = GameState.Shop;
                else if (TextClicked(gameStates[4], gameStatesPosition[4], defaultFont))
                    CurrentState = GameState.MainMenu;
            }
             else if(level == 3)
             {
                if (TextClicked(gameStates[0], gameStatesPosition[0], defaultFont))
                    CurrentState = GameState.Level1;
                else if (TextClicked(gameStates[1], gameStatesPosition[1], defaultFont))
                    CurrentState = GameState.Level2;
                else if (TextClicked(gameStates[3], gameStatesPosition[3], defaultFont))
                    CurrentState = GameState.Shop;
                else if (TextClicked(gameStates[4], gameStatesPosition[4], defaultFont))
                    CurrentState = GameState.MainMenu;
             }  
        }
        public bool QuitCombat
        {
            get { return quitCombat; }
            set { quitCombat = value; }
        }
    }
}
