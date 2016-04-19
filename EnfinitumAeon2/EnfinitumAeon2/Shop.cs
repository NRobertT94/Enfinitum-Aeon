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
    public class Shop : GameSettings
    {
        private Texture2D background;
        private Texture2D box;
        private Texture2D statsBox;
        private Texture2D shipBox;
        private Texture2D yesNoBox;
        private int level, healthUp;
        private Player player;
        private Rectangle local;
        private Rectangle localHealthbar;
        private string BackButton = "Back";
        private string[] upgradeButtons = new string[] { "UpgradeAttack", "UpgradeDefense", "UpgradeSpeed", "BuyNewShip" }; // this will be displayed in each solar system as text/buttons
        private Vector2[] upgradeButtonsPosition = new Vector2[4];  // position for each button
        private Color[] buttonColor = new Color[4];
        private List<Ship> ships;
        private bool selected;
        private int priceA, priceD, priceS, priceR;
        private Vector2 statsPosition;
        private Vector2 backButtonPosition;
        private bool selectionScreen;
        private Ship selectedShip;
        private int counter;
        private int speedCounter;
        /// <summary>
        /// Constructor creates a shop with features that corespond with the player
        /// </summary>
        /// <param level="level"></param>
        /// <param player="player"></param>
        /// <param name="graphics"></param>
        public Shop(int level, Player player, GraphicsDeviceManager graphics)
            : base(graphics)
        {
            this.player = player;
            selectedShip = player.Ship;
            this.level = level;
            selectionScreen = false;
            selected = false;
            backButtonPosition = new Vector2(screenWidth - 100, screenHeight - 50);
            priceA = 20;
            priceD = 20;
            priceS = 1000;
            priceR = 100;
            counter = 0;
            speedCounter = 0;
            healthUp = 0;
            statsPosition = new Vector2(screenWidth / screenWidth + 40, 40);
            CreateShip();

        }

        /// <summary>
        /// InitlilizeShop function which sets the buttons positions
        /// </summary>
        public void InitializeShop()
        {
            int spaceBetween = 200;
            for (int i = 0; i < upgradeButtons.Length; i++)
            {
                upgradeButtonsPosition[i] = new Vector2(screenWidth - 200, screenHeight - spaceBetween);
                buttonColor[i] = Color.White;
                spaceBetween += 100;
            }
        }

        /// <summary>
        /// Draws the upgrade buttons of the shop
        /// </summary>
        public void shopDraw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < upgradeButtons.Length; i++)
            {
                spriteBatch.DrawString(mediumFont, upgradeButtons[i], upgradeButtonsPosition[i], buttonColor[i]);
                spriteBatch.Draw(box, new Vector2(upgradeButtonsPosition[i].X - 20, upgradeButtonsPosition[i].Y - 20), Color.White);
            }
        }

        /// <summary>
        /// Draws the price under the upgrade buttons
        /// </summary>
        public void drawPrice(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < upgradeButtons.Length; i++)
            {

                if (upgradeButtons[i].Equals("UpgradeAttack"))
                {
                    spriteBatch.DrawString(mediumFont, "Cost:" + priceA, new Vector2(upgradeButtonsPosition[i].X, upgradeButtonsPosition[i].Y + 20), Color.White);
                }
                else if (upgradeButtons[i].Equals("UpgradeDefense"))
                {
                    spriteBatch.DrawString(mediumFont, "Cost:" + priceD, new Vector2(upgradeButtonsPosition[i].X, upgradeButtonsPosition[i].Y + 20), Color.White);
                }
                else if (upgradeButtons[i].Equals("UpgradeSpeed"))
                {
                    spriteBatch.DrawString(mediumFont, "Cost:" + priceS, new Vector2(upgradeButtonsPosition[i].X, upgradeButtonsPosition[i].Y + 20), Color.White);
                }
            }

        }

        /// <summary>
        /// Loads the content
        /// </summary>
        public void LoadContent(ContentManager content)
        {

            background = content.Load<Texture2D>("Backgrounds/shop2");
            box = content.Load<Texture2D>("Planets/battleButton");
            statsBox = content.Load<Texture2D>("Planets/displayStats2");
            shipBox = content.Load<Texture2D>("Planets/shipBox");
            yesNoBox = content.Load<Texture2D>("Planets/yesNoBox");
            player.Ship.LoadContent(content);
            LoadOptionsContent(content);
            foreach (Ship ship in ships)
            {
                ship.LoadContent(content);
            }


        }

        /// <summary>
        /// The main draw method which is invoked when the Shop button is pressed
        /// drawing all the buttons, ships and stats
        /// </summary>
        public void Draw(SpriteBatch spriteBatch)
        {
            InitializeShop();
            player.Ship.deleteFromScreen();
            spriteBatch.Draw(background, new Rectangle(0, 0, screenWidth, screenHeight), Color.White);

            if (!selectionScreen)
            {

                spriteBatch.Draw(background, new Rectangle(0, 0, screenWidth, screenHeight), Color.White);
                spriteBatch.DrawString(biggerFont, "Upgrade your ship", new Vector2((screenWidth / 2), 1), Color.White);
                spriteBatch.DrawString(mediumFont, BackButton, backButtonPosition, Color.White);
                spriteBatch.Draw(box, new Vector2(backButtonPosition.X - 20, backButtonPosition.Y - 20), Color.White);                        
                spriteBatch.DrawString(mediumFont, "Buy rockets", new Vector2(screenWidth / screenWidth + 40, screenHeight - 200), Color.White);
                spriteBatch.Draw(box, new Vector2(screenWidth / screenWidth + 20, screenHeight - 220), Color.White);
                spriteBatch.DrawString(mediumFont, "Cost: 100", new Vector2(screenWidth / screenWidth + 40, screenHeight - 180), Color.White);
                shopDraw(spriteBatch);
                drawPrice(spriteBatch);
                spriteBatch.DrawString(mediumFont, "Enfinitum:" + player.Enfinitum + "\n" + "Credits: " + player.Credits + "\n" + "Attack:" + player.Ship.Attack + "\n" + "Defense:" + player.Ship.Defense + "\n" + "Name:" + player.Ship.Name + "\n" + "Bonus Speed:" + player.Ship.MoveSpeed + "\n" + "Level:" + player.Ship.Level + "\n"+ "Experience: " +player.Experience+"\n" + "Rockets:" + player.Ship.Rockets, statsPosition, Color.White);
                spriteBatch.Draw(statsBox, new Rectangle((int)statsPosition.X-20, (int)statsPosition.Y-20,270,320), Color.White);
                local = player.Ship.Rectangle;
                localHealthbar = player.Ship.HealthBar;
                Rectangle rectangle = new Rectangle((screenWidth / 2) - 200, screenHeight / 2, 150, 150);
                player.Ship.Rectangle = rectangle;
                player.Ship.healthBar = new Rectangle(rectangle.X, rectangle.Y + rectangle.Height + 5, rectangle.Width, 5);
                spriteBatch.Draw(shipBox, new Vector2(player.Ship.Rectangle.X - 70, player.Ship.Rectangle.Y - 50), Color.White);
                player.Ship.Draw(spriteBatch);
                spriteBatch.DrawString(mediumFont, player.Ship.Name, new Vector2((screenWidth / 2) - 200, (screenHeight / 2) - 25), Color.Red);

                for (int i = 0; i < upgradeButtons.Length; i++)
                {
                    if (HoveringOverText(upgradeButtons[i], upgradeButtonsPosition[i], biggerFont) && Clicked())
                    {
                        if (i == 0)
                        {
                            addAttack(player, spriteBatch);
                        }
                        else if (i == 1)
                        {
                            addDefense(player, spriteBatch);
                        }
                        else if (i == 2)
                        {
                            addMovSpeed(player, spriteBatch);
                        }
                        else if (i == 3)
                        {
                            selectionScreen = true;
                        }
                    }

                }
            
                if (HoveringOverText("Buy rockets", new Vector2(screenWidth / screenWidth + 40, screenHeight - 200), mediumFont) && Clicked())
                {
                    if (player.Credits >= 100)
                    {
                        player.Ship.Rockets += 1;
                        player.Credits -= 100;
                    }
                    else
                    {
                        spriteBatch.DrawString(biggerFont, "Not enough credits", new Vector2(screenWidth / 2, screenHeight / 2), Color.Blue);
                    }
                }

            }
            else
            {
                DrawShips(spriteBatch);
            }

            player.Ship.Rectangle = local;
            player.Ship.healthBar = localHealthbar;

        }


        public void DrawShips(SpriteBatch spriteBatch)
        {
            int positionX = 5;
            spriteBatch.Draw(background, new Rectangle(0, 0, screenWidth, screenHeight), Color.White);
            if (!selected)
            {
                spriteBatch.DrawString(mediumFont, "Which ship do you want to purchase?", new Vector2(screenWidth / 15, screenHeight / 20), Color.White);
                spriteBatch.DrawString(mediumFont, "Back", new Vector2(screenWidth / 20, screenHeight - 50), Color.White);
                spriteBatch.Draw(box, new Vector2(screenWidth / 20 - 20, screenHeight - 70), Color.White);

                for (int j = 0; j < ships.Count; j++)
                {
                    positionX += 250;

                    if (j == 1)
                    {
                        Rectangle rectangle = new Rectangle((screenWidth / 2 - 600) + positionX, (screenHeight / 2) - 200, 100, 50);
                        ships[j].Rectangle = rectangle;
                        ships[j].healthBar = new Rectangle(rectangle.X, rectangle.Y + rectangle.Height + 5, rectangle.Width, 5);
                        spriteBatch.Draw(shipBox, new Vector2(ships[j].Rectangle.X - 70, ships[j].Rectangle.Y - 70), Color.White);
                        spriteBatch.DrawString(mediumFont, ships[j].Name, new Vector2(ships[j].Rectangle.X - ships[j].Rectangle.Width / 6, ships[j].Rectangle.Y - ships[j].Rectangle.Height / 2), Color.Red);
                        spriteBatch.DrawString(mediumFont, "Cost:" + ships[j].Price, new Vector2(ships[j].Rectangle.X, ships[j].Rectangle.Y + 120), Color.White);
                        ships[j].Draw(spriteBatch);
                    }
                    else
                    {
                        Rectangle rectangle = new Rectangle((screenWidth / 2 - 600) + positionX, (screenHeight / 2), 100, 100);
                        ships[j].Rectangle = rectangle;
                        ships[j].healthBar = new Rectangle(rectangle.X, rectangle.Y + rectangle.Height + 5, rectangle.Width, 5);
                        spriteBatch.Draw(shipBox, new Vector2(ships[j].Rectangle.X - 70, ships[j].Rectangle.Y - 70), Color.White);
                        spriteBatch.DrawString(mediumFont, ships[j].Name, new Vector2(ships[j].Rectangle.X - ships[j].Rectangle.Width / 6, ships[j].Rectangle.Y - ships[j].Rectangle.Height / 2), Color.Red);
                        ships[j].Draw(spriteBatch);
                        spriteBatch.DrawString(mediumFont, "Cost:" + ships[j].Price, new Vector2(ships[j].Rectangle.X, ships[j].Rectangle.Y + 120), Color.White);
                    }


                    if (HoverOverShip(ships[j].Rectangle) && Clicked())
                    {
                        selectedShip = ships[j];
                        selected = true;
                    }
                    else if (HoveringOverText("Back", new Vector2(screenWidth / 20, screenHeight - 50), mediumFont) && Clicked())
                    {
                        selectionScreen = false;
                    }
                }
            }
            else
            {
                spriteBatch.DrawString(biggerFont, "Do you want to buy " + selectedShip.Name + "?", new Vector2(screenWidth / 2 - (screenWidth / 4), screenHeight / screenHeight + 30), Color.White);
                //spriteBatch.DrawString(biggerFont,"Cost: "+selectedShip.Price,new Vector2)
                spriteBatch.DrawString(mediumFont, "Ship stats" + "\n\n" + "Cost: " + selectedShip.Price + "\n" + "Attack:" + selectedShip.Attack + "\n" + "Defense:" + selectedShip.Defense + "\n" + "Name:" + selectedShip.Name + "\n" + "Bonus Speed:" + selectedShip.MoveSpeed + "\n" + "Level:" + selectedShip.Level + "\n" + "Rockets: " + selectedShip.Rockets, new Vector2(statsPosition.X, statsPosition.Y + statsBox.Height + 30), Color.White);
                spriteBatch.Draw(statsBox, new Rectangle((int)statsPosition.X - 20, (int)statsPosition.Y - 20, 270, 320), Color.White);
                spriteBatch.DrawString(mediumFont, "Enfinitum:" + player.Enfinitum + "\n" + "Credits: " + player.Credits + "\n" + "Attack:" + player.Ship.Attack + "\n" + "Defense:" + player.Ship.Defense + "\n" + "Name:" + player.Ship.Name + "\n" + "Bonus Speed:" + player.Ship.MoveSpeed + "\n" + "Level:" + player.Ship.Level + "\n"+ "Experience: " +player.Experience+ "\n" + "Rockets:" + player.Ship.Rockets, statsPosition, Color.White);
                spriteBatch.Draw(statsBox, new Rectangle((int)statsPosition.X - 20, (int)statsPosition.Y + 360, 240, 270), Color.White);
                //drawTip(spriteBatch);
                spriteBatch.DrawString(biggerFont, "Yes", new Vector2(screenWidth / 2 - 150, (screenHeight - 80)), Color.White);
                spriteBatch.Draw(yesNoBox, new Vector2(screenWidth / 2 - 170, (screenHeight - 80)), Color.White);
                spriteBatch.DrawString(biggerFont, "No", new Vector2(screenWidth / 2 + 150, (screenHeight - 80)), Color.White);
                spriteBatch.Draw(yesNoBox, new Vector2(screenWidth / 2 + 120, (screenHeight - 80)), Color.White);
                spriteBatch.Draw(shipBox, new Vector2(selectedShip.Rectangle.X - 70, selectedShip.Rectangle.Y - 70), Color.White);
                selectedShip.Draw(spriteBatch);
                if (TextClicked("Yes", new Vector2(screenWidth / 2 - 150, (screenHeight - 85)), biggerFont) && Clicked())
                {
                    if (player.Credits >= selectedShip.Price)
                    {
                        buyShip(player, selectedShip, spriteBatch);
                    }
                    else
                    {
                        spriteBatch.DrawString(biggerFont, "Not enough credits", new Vector2(screenWidth / 2, screenHeight / 2), Color.Red);
                    }

                }
                else if (TextClicked("No", new Vector2(screenWidth / 2 + 150, (screenHeight - 80)), biggerFont) && Clicked())
                {
                    selected = false;
                }
                else if (EscapePressed())
                {
                    selectionScreen = false;
                }
            }
        }

        /// <summary>
        /// Draws the tip when a ship is selected
        /// </summary>
        public void drawTip(SpriteBatch spriteBatch)
        {
            if (selectedShip.Name.Equals("Fighter"))
            {
                spriteBatch.DrawString(mediumFont, "The Fighter is" + "\n" + "excellent for " + "\n" + "new players." + "\n" + "However,he is not" + "\n" + "as effective as" + "\n" + "the Destroyer or" + "\n" + "the Corvette", new Vector2(screenWidth - 300, screenHeight / 2 - 200), Color.White);
                spriteBatch.Draw(statsBox, new Vector2(screenWidth - 330, screenHeight / 2 - 220), Color.White);
            }
            else if (selectedShip.Name.Equals("Destroyer"))
            {
                spriteBatch.DrawString(mediumFont, "The Destroyer is" + "\n" + "excellent ship" + "\n" + "He is the most" + "\n" + "effective ship" + "\n" + "with high " + "\n" + "attack damage" + "\n" + "and increased" + "\n" + "health", new Vector2(screenWidth - 300, screenHeight / 2 - 200), Color.White);
                spriteBatch.Draw(statsBox, new Vector2(screenWidth - 330, screenHeight / 2 - 220), Color.White);
            }
            else if (selectedShip.Name.Equals("Corvette"))
            {
                spriteBatch.DrawString(mediumFont, "The Corvette " + "\n" + " has increased " + "\n" + "attack damage" + "\n" + "but he suffers " + "\n" + "from reduced" + "\n" + "health", new Vector2(screenWidth - 300, screenHeight / 2 - 200), Color.White);
                spriteBatch.Draw(statsBox, new Vector2(screenWidth - 330, screenHeight / 2 - 220), Color.White);
            }
        }

        /// <summary>
        /// Upgrades attack dmg of the current ship, also increasing the level of the ship
        /// </summary>
        public void addAttack(Player player, SpriteBatch spriteBatch)
        {

            if (player.Credits > priceA)
            {
                player.Ship.Attack += 1;
                player.Credits -= priceA;
                priceA = 20 + player.Ship.Level;
                counter++;
                if (counter == 5)
                {
                    player.Ship.Level += 1;
                    counter = 0;
                }

            }
            else
            {
                spriteBatch.DrawString(biggerFont, "Not enough credits", new Vector2(screenWidth / 2, screenHeight / 2), Color.Blue);
            }

        }

        /// <summary>
        /// Upgrades defense of the current ship, also increasing the level of the ship
        /// </summary>
        public void addDefense(Player player, SpriteBatch spriteBatch)
        {

            if (player.Credits > priceD)
            {
                player.Ship.Defense +=1;
                player.Ship.healthBar.Width += 1;
                player.Ship.healthBar.Width = player.Ship.NewHealthBar;
                player.Ship.InitialDefense += 1;
                player.Credits = player.Credits - priceD;
                priceD = 20 + player.Ship.Level;
                counter++;
                if (counter == 5)
                {
                    player.Ship.Level += 1;
                    counter = 0;
                }
            }
            else
            {
                spriteBatch.DrawString(biggerFont, "Not enough credits", new Vector2(screenWidth / 2, screenHeight / 2), Color.Blue);
            }

        }

        /// <summary>
        /// Upgrades speed of the current ship, also increasing the level of the ship
        /// Max 4 upgrades
        /// </summary>
        public void addMovSpeed(Player player, SpriteBatch spriteBatch)
        {

            if (player.Credits > priceS && speedCounter < 4)
            {
                player.Ship.MoveSpeed += 1;
                player.Credits = player.Credits - priceS;
                priceS = 100 + player.Ship.Level * 2;
                speedCounter++;
                player.Ship.Level += 4;

            }
            else
            {
                spriteBatch.DrawString(biggerFont, "Not enough credits", new Vector2(screenWidth / 2, screenHeight / 2), Color.Blue);
            }

        }

        /// <summary>
        /// Resets the prices of the upgrades
        /// </summary>
        public void resetPrice()
        {
            priceA = 20;
            priceD = 20;
            priceS = 1000;
        }

        /// <summary>
        /// Method which takes the currentShip and swaps it with the selected ship if the conditions are met
        /// </summary>
        public void buyShip(Player player, Ship shipToBuy, SpriteBatch spriteBatch)
        {
            player.Ship = shipToBuy;
            player.Credits = player.Credits - shipToBuy.Price;
            selectionScreen = false;
            selected = false;
            resetPrice();

        }

        /// <summary>
        /// Creates the 3 ships the user can choose from to buy
        /// </summary>
        public void CreateShip()
        {
            ships = new List<Ship>(3);
            ships.AddRange(new Ship[] { new Ship("Fighter", "Player", graphics), new Ship("Destroyer", "Player", graphics), new Ship("Corvette", "Player", graphics) });

        }


        public void Update(GameTime gameTime, ContentManager content)
        {
            UpdateInputState();
        }

        /// <summary>
        /// Checks if the backButton is clicked and returns true or false
        /// </summary>
        public bool backButtonClicked()
        {
            if (TextClicked(BackButton, backButtonPosition, biggerFont))
                return true;
            return false;
        }

    }
}