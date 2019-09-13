using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

namespace HealthyFrenzy
{
    class Player
    {
        // properties
        Texture2D texture;
        Vector2 position;
        int speed;

        // player controls
        KeyboardState keyboardState;
        GamePadState gamePadState;
       

        public Player(Texture2D txr) // Constructor
        {
            texture = txr;
            speed = 10;
            SetStartPlayerPosition();
        }

        public void SetStartPlayerPosition()
        {
            position = new Vector2((Game1.screenBounds.Width - Width) / 2, Game1.screenBounds.Height - Height);
        }

        public int Width
        {
            get { return texture.Width; }
        }

        public int Height
        {
            get { return texture.Height; }
        }

        public void Update()
        {
            keyboardState = Keyboard.GetState();
            gamePadState = GamePad.GetState(PlayerIndex.One);

            if (gamePadState.DPad.Left == ButtonState.Pressed || keyboardState.IsKeyDown(Keys.Left))
            {
                position.X -= speed;
            }
            if (gamePadState.DPad.Right == ButtonState.Pressed || keyboardState.IsKeyDown(Keys.Right))
            {
                position.X += speed;
            }
            
            if(position.X <=0 || position.X >= Game1.screenBounds.Width - Width * 0.72f) 
            {
                Game1.HopEffect.Play();
            }

            // 0.72f because =
            position.X = MathHelper.Clamp(position.X, 1, (Game1.screenBounds.Width - Width*0.72f) -1); 
        }

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(texture, position, Color.Black);
        }

        public Rectangle BoundingBox
        {
            get
            {
                return new Rectangle((int)position.X+15, (int)position.Y+10, Width-60, 10);
            }
        }
    }
}
