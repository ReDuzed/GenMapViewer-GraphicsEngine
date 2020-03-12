using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using RUDD;

namespace GenMapViewer
{
    public class Player : Main
    {
        public static Vector2 position;
        public static Vector2 velocity;
        private bool canJump;
        private const int plrWidth = 32, plrHeight = 48;
        private Rectangle hitbox;
        protected override void PreDraw(Bitmap bmp, Graphics gfx)
        {
            hitbox = new Rectangle((int)position.X + width / 2 - plrWidth / 2, (int)position.Y, plrWidth, plrHeight);
            gfx.DrawRectangle(Pens.Blue, new System.Drawing.Rectangle(hitbox.x, hitbox.y, hitbox.Width, hitbox.Height));
            gfx.DrawString(velocity.X.ToString(), SystemFonts.DefaultFont, Brushes.Red, 10, 100);
            gfx.DrawString((position.Y + plrHeight).ToString(), SystemFonts.DefaultFont, Brushes.Red, 10, 112);
            gfx.DrawString(square[0].Y.ToString(), SystemFonts.DefaultFont, Brushes.Red, 10, 124);
            gfx.DrawString(canJump.ToString(), SystemFonts.DefaultFont, Brushes.Red, 10, 136);
            base.PreDraw(bmp, gfx);
        }
        protected override void Update()
        {
            //  Initializing
            bool canLeft = true, canRight = true;
            float moveSpeed = 0.15f;
            float maxSpeed = 3f;
            float stopSpeed = moveSpeed * 2f;
            float jumpSpeed = maxSpeed * 2f, fallSpeed = 0.917f;
            //  Clamp
            if (velocity.X > maxSpeed)
                velocity.X = maxSpeed;
            if (velocity.X < -maxSpeed)
                velocity.X = -maxSpeed;
            if (velocity.Y >= fallSpeed * maxSpeed)
                velocity.Y = fallSpeed * maxSpeed;
            //  Border
            canLeft = position.X >= 1;
            canRight = position.X < width - 1;
            //  Positioning
            position.X += velocity.X;
            position.Y += velocity.Y;
            if (!canJump)
                velocity.Y += fallSpeed;
            //  Controls
            if (!KeyDown(Key.A) && KeyDown(Key.D))
            {
                velocity.X += moveSpeed;
            }
            else if (KeyDown(Key.A) && !KeyDown(Key.D))
            {
                velocity.X -= moveSpeed;
            }
            else if (velocity.X > 0f)
            {
                velocity.X -= stopSpeed;
            }
            else if (velocity.X < 0f)
            {
                velocity.X += stopSpeed;
            }
            if (canJump && KeyDown(Key.Space))
            {
                velocity.Y = -jumpSpeed;
                canJump = false;
            }
            //  Movement speed set
            if (velocity.X < moveSpeed && velocity.X > -moveSpeed)
                velocity.X = 0f;
            //  Brush interaction
            foreach (SquareBrush sq in square.Where(t => t != null))
            {
                for (float i = 0; i < velocity.Y; i += fallSpeed)
                {
                    for (int n = (int)position.X - 8; n < (int)position.X + 8; n++)
                    {
                        if (position.Y + plrHeight >= sq.Y)
                        {
                            canJump = true;
                            position.Y = sq.Y - plrHeight;
                            velocity.Y = 0f;
                        }
                    }
                }
            }
        }
        private new bool KeyUp(Key key)
        {
            return Keyboard.IsKeyUp(key);
        }
        private new bool KeyDown(Key key)
        {
            return Keyboard.IsKeyDown(key);
        }
    }
}
