using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Xml.Serialization;
using System.Diagnostics;

namespace EnfinitumAeon2
{
    /// <summary>
    /// Handles saving to and reading (loading) from a text file to save game state. 
    /// We store user statistics and the conquered/not-conquered state of each planet in each solar system.
    /// </summary>
    public class Save
    {
        private Player p;
        private SolarSystem s1, s2, s3;
        string saveData;
        private GraphicsDeviceManager graphics;

        /// <summary>
        /// Constructor. Creates a new instance of Save and creates a blank string for the save data.
        /// <param name="graphics">The graphics manager must be passed in to establish the solar systems</param>
        /// </summary>
        public Save(GraphicsDeviceManager graphics)
        {
            this.graphics = graphics;
            p = new Player(graphics);
            s1 = new SolarSystem(p, 1, graphics, this);
            s2 = new SolarSystem(p, 2, graphics, this);
            s3 = new SolarSystem(p, 3, graphics, this);
            s1.ResetSolarSystem(1);
            s2.ResetSolarSystem(2);
            s3.ResetSolarSystem(3);
            saveData = "";
        }

        /// <summary>
        /// Fills this Save instance with the data to be saved.
        /// </summary>
        /// <param name="p">The player</param>
        /// <param name="s1">Solar system 1</param>
        /// <param name="s2">Solar system 2</param>
        /// <param name="s3">Solar system 3</param>
        public void updateSaveParams(Player p, SolarSystem s1, SolarSystem s2, SolarSystem s3)
        {
            this.p = p;
            this.s1 = s1;
            this.s2 = s2;
            this.s3 = s3;
        }

        /// <summary>
        /// Creates the save data string in the format described in EnfinitumAeon2\EnfinitumAeon2\"save-load schema".txt
        /// </summary>
        private void formatSaveData()
        {
            saveData = "u:";

            saveData += p.Enfinitum + "," + p.Credits + "," + p.Ship.Type + "," + p.Ship.Attack + 
                "," + p.Ship.Defense + "," + p.Ship.MoveSpeed + ".";     // adds save data for the user
            SolarSystem[] solarArray = new SolarSystem[] { s1, s2, s3 };
            for (int i = 0; i < solarArray.Length; i++)
            {
                saveData += "s" + (i + 1) + ":";

                int limit = solarArray[i].Planets.Length;
                Planet[] temp = solarArray[i].Planets;

                for (int j = 1; j < limit; j++)
                {
                    if (temp[j].Conquered)
                        saveData += "0";        // writes 0 for a planet if conquered...
                    else
                        saveData += "1";        // ... or 1 if it's not

                    if (j == limit - 1)
                        saveData += ".";
                    else
                        saveData += ",";

                }
            }
        }

        /// <summary>
        /// Writes to a local file in the root directory. Not easy to find, but not supposed to be; 
        /// the user never has to access this file directly.
        /// </summary>
        public void saveGame()
        {
            formatSaveData();
            System.IO.StreamWriter file = new System.IO.StreamWriter(AppDomain.CurrentDomain.BaseDirectory + @"\savefile.txt");
            file.WriteLine(saveData);
            file.Close();       // closes the stream down again
        }

        public bool loadGame()
        {
            const int USER_OFFSET = 3, SOLARSYSTEM_OFFSET = 4;          // constant offsets used for reading; stored for ease of reading
            
            try
            {
                string loadData = System.IO.File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + @"\savefile.txt");

                int uStart, s1Start, s2Start, s3Start;  // starting indexes of each of the 4 save sections
                uStart = loadData.IndexOf("u") + 1;     // +1 to account for colon
                s1Start = loadData.IndexOf("s1") + 2;
                s2Start = loadData.IndexOf("s2") + 2;
                s3Start = loadData.IndexOf("s3") + 2;

                string temp = loadData.Substring(uStart + 1, ((s1Start - USER_OFFSET) - uStart));
                string[] userData = temp.Split(new Char[] { ',', '.' });     // delimits for commas and terminating full stops
                p.Enfinitum = Convert.ToInt32(userData[0]);
                p.Credits = Convert.ToInt32(userData[1]);
                int type = Convert.ToInt32(userData[2]);
                if (type == 1)
                    p.Ship.Name = "Fighter";
                else if (type == 2)
                    p.Ship.Name = "Corvette";
                else
                    p.Ship.Name = "Destroyer";
                p.Ship.Attack = Convert.ToInt32(userData[3]);
                p.Ship.Defense = Convert.ToInt32(userData[4]);
                p.PlanetsConquered = Convert.ToInt32(userData[5]);

                SolarSystem[] solarArray = new SolarSystem[] { s1, s2, s3 };
                int[] solarStartPoints = new int[] { s1Start, s2Start, s3Start, loadData.Length };
                string[] currentSolarData = new string[] { };
                for (int i = 0; i < solarArray.Length; i++)     // iterates through every planet in all 3 solar systems
                {
                    temp = loadData.Substring(solarStartPoints[i] + 1,
                        ((solarStartPoints[i + 1] - SOLARSYSTEM_OFFSET) - solarStartPoints[i]));
                    currentSolarData = temp.Split(new Char[] { ',', '.' });
                    for (int j = 1; j < currentSolarData.Length; j++)
                    {
                        if (Convert.ToInt32(currentSolarData[j-1]) == 0)
                        {
                            solarArray[i].Planets[j].Conquered = true;
                            solarArray[i].Planets[j].Defences = 0;
                            solarArray[i].Planets[j].Enfinitum = 0;
                            solarArray[i].Planets[j].Credits = 0;
                            solarArray[i].Planets[j].calculateChance();
                        }
                        else
                            solarArray[i].Planets[j].Conquered = false;
                    }
                }   
            }
            catch (FileNotFoundException fnf)
            {
                // ERROR
                return false;
            }
            return true;
        }
        /// <summary>
        /// Returns the Player instance for external access
        /// </summary>
        public Player Player
        {
            get { return p; }
        }
        /// <summary>
        /// Returns the array of 3 SolarSystem instances for external access
        /// </summary>
        public SolarSystem[] SolarSystems
        {
            get { return (new SolarSystem[] { s1, s2, s3 }); }
        }
    }
}
