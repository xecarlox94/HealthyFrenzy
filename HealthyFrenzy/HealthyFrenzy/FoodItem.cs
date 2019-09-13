using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace HealthyFrenzy
{
    class FoodItem
    {
        public Vector2 position;
        Texture2D texture;
        int speed;
        int value;
        public bool isActive;


        public FoodItem (Texture2D txr, int posX, int val)
        {
            texture = txr;
            speed = 3;
            position.Y = 0;
            position.X = posX;
            value = val;

            isActive = true;
        }

        public int Width
        {
            get { return texture.Width; }
        }

        public int Heigth
        {
            get { return texture.Height; }
        }

        public void Update()
        {
            position.Y += speed;
        }

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(texture, position, Color.White);
        }

        public Rectangle BoundingBox
        {
            get
            {
                return new Rectangle((int)position.X, (int)position.Y, Width, 1);
            }
        }

        public void wasCaught(Player p,FoodItem f)
        {
            if (p.BoundingBox.Intersects(BoundingBox))
            {
                isActive = false;
                Game1.HealthLevel += value;
                if (f.value > 0)
                {
                    Game1.LevelUpSoundEffect.Play();
                }
                if (f.value < 0)
                {
                    Game1.LevelDownSoundEffect.Play();
                }
            }
        }
    }
}
