using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace EnfinitumAeon2
{
    /// <summary>
    /// This class represents a bullet...
    /// </summary>
    public class Munition
    {
        public Texture2D texture;
        public Vector2 position;
        public bool isVisible;
        public Rectangle rectangle;
        public float velocity;
        public Vector2 size;
        public string name;
        /// <summary>
        /// The constructor has a parameter that will automatically 
        /// load the texture of the bullet
        /// </summary>
        /// <param name="texture"></param>
        public Munition(Texture2D texture)
        {
            this.texture = texture;
            this.isVisible = false;
        }
        public void UpdateBullet()
        {
            rectangle = new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y);
        }
        /// <summary>
        /// Draws the bullet if it is visible
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch)
        {
            if (isVisible)
                spriteBatch.Draw(texture, rectangle, Color.White);
        }


    }
}
