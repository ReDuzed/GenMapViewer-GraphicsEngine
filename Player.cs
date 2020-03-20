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
        public Vector2 position;
        public Vector2 velocity;
        public Vector2 Center
        {
            get { return new Vector2(position.X + plrWidth / 2, position.Y + plrHeight / 2); }
        }
        private bool canJump;
        private const int plrWidth = 32, plrHeight = 48;
        private Rectangle hitbox;
        private bool colUp, colDown, colRight, colLeft;
        protected override void PreDraw(Bitmap bmp, Graphics gfx)
        {
            hitbox = new Rectangle((int)position.X + width / 2 - plrWidth / 2, (int)position.Y, plrWidth, plrHeight);
            gfx.DrawRectangle(Pens.Blue, new System.Drawing.Rectangle(hitbox.x, hitbox.y, hitbox.Width, hitbox.Height));
            gfx.DrawString(velocity.X.ToString(), SystemFonts.DefaultFont, Brushes.Red, 10, 100);
            gfx.DrawString((position.Y + plrHeight).ToString(), SystemFonts.DefaultFont, Brushes.Red, 10, 112);
            gfx.DrawString(square[1].Y.ToString(), SystemFonts.DefaultFont, Brushes.Red, 10, 124);
            gfx.DrawString(canJump.ToString(), SystemFonts.DefaultFont, Brushes.Red, 10, 136);
            gfx.DrawString(Distance(Angle(0, 0), position).X.ToString(), SystemFonts.DefaultFont, Brushes.Red, 10, 148);
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
            if (velocity.Y > maxSpeed)
                velocity.Y = maxSpeed;
            if (velocity.Y < -maxSpeed)
                velocity.Y = -maxSpeed;
            /*
            if (velocity.Y >= fallSpeed * maxSpeed)
                velocity.Y = fallSpeed * maxSpeed;
               */
            //  Border
            canLeft = position.X >= 1;
            canRight = position.X < width - 1;
            //  Positioning
            position.X += velocity.X;
            position.Y += velocity.Y;
            /*
            if (!canJump)
                velocity.Y += fallSpeed;
                */
            //  Controls
            if (!KeyDown(Key.A) && KeyDown(Key.D))
            {
                // move right
                velocity.X += moveSpeed;
            }
            else if (KeyDown(Key.A) && !KeyDown(Key.D))
            {
                // move left
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
            if (!KeyDown(Key.W) && KeyDown(Key.S))
            {
                // move down
                velocity.Y += moveSpeed;
            }
            else if (KeyDown(Key.W) && !KeyDown(Key.S))
            {
                // move up
                velocity.Y -= moveSpeed;
            }
            else if (velocity.Y > 0f)
            {
                velocity.Y -= stopSpeed;
            }
            else if (velocity.Y < 0f)
            {
                velocity.Y += stopSpeed;
            }
            //  Collision
            if (colLeft && velocity.X < 0f)
                velocity.X = 0f;
            if (colRight && velocity.X > 0f)
                velocity.X = 0f;
            if (colUp || colDown)
                velocity.Y = 0f;
            /*
            if (canJump && KeyDown(Key.Space))
            {
                velocity.Y = -jumpSpeed;
                canJump = false;
            }
                */
            //  Movement speed set
            if (velocity.X < moveSpeed && velocity.X > -moveSpeed)
                velocity.X = 0f;
            if (velocity.Y < moveSpeed && velocity.Y > -moveSpeed)
                velocity.Y = 0f;
            //  Brush interaction
            foreach (SquareBrush sq in square.Where(t => t != null))
            {
                if (colRight = sq.Hitbox.Collision(position.X + plrWidth, position.Y))
                {
                    position.X = sq.X - plrWidth;
                }
                
                if (colDown = sq.Hitbox.Collision(position.X + plrWidth / 2, position.Y + plrHeight))
                {
                    position.Y = sq.Y - plrHeight;
                }
                if (colUp = sq.Hitbox.Collision(position.X + plrWidth / 2, position.Y))
                {
                    position.Y = sq.Y + sq.Height;
                    break;
                }
                if (colLeft = sq.Hitbox.Collision(position.X, position.Y))
                {
                    position.X = sq.X + sq.Width;
                    break;
                }
            }
                /*
                foreach (SquareBrush sq in square.Where(t => t != null))
                {
                    var dist = Distance(Angle(sq.X, sq.Y), position);
                    if (dist.X < 30f && dist.Y < 30f)
                    {
                        for (float i = 0; i < velocity.Y; i += fallSpeed)
                        {
                            for (int n = (int)position.X + plrWidth / 2 - 8; n < (int)position.X + plrWidth / 2 + 8; n++)
                            {
                                if (sq.Hitbox.Intersects(hitbox))
                                {
                                    canJump = true;
                                    position.Y = sq.Y - plrHeight;
                                    velocity.Y = 0f;
                                }
                            }
                        }
                    }
                }
                    */
            }
        public float Angle(float x, float y)
        {
            return (float)Math.Atan2(position.Y - y, position.X - x);
        }
        public Vector2 Distance(float angle, Vector2 position)
        {
            float VelocityX = (float)(position.X + Math.Cos(angle));
            float VelocityY = (float)(position.Y + Math.Sin(angle));
            return new Vector2(VelocityX, VelocityY);
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
