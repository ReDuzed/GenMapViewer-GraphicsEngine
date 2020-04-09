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
    public class Player : Entity
    {
        
        private bool canJump;
        private bool clamp;
        public const int plrWidth = 32, plrHeight = 48;
        
        private bool colUp, colDown, colRight, colLeft;
        protected override void PreDraw(Bitmap bmp, Graphics gfx)
        {
            gfx.DrawRectangle(Pens.Blue, new System.Drawing.Rectangle(hitbox.x, hitbox.y, hitbox.Width, hitbox.Height));
           
            gfx.DrawString(velocity.X.ToString(), SystemFonts.DefaultFont, Brushes.Red, 10, 100);
            gfx.DrawString(position.Y.ToString(), SystemFonts.DefaultFont, Brushes.Red, 10, 112);
            gfx.DrawString(position.X.ToString(), SystemFonts.DefaultFont, Brushes.Red, 10, 124);
            //gfx.DrawString(square[0].Y.ToString(), SystemFonts.DefaultFont, Brushes.Red, 10, 124);
            gfx.DrawString(canJump.ToString(), SystemFonts.DefaultFont, Brushes.Red, 10, 136);
            gfx.DrawString(Main.frameRate.ToString(), SystemFonts.DefaultFont, Brushes.Red, 10, 148);
            gfx.DrawString(Instance.MousePosition().X.ToString(), SystemFonts.DefaultFont, Brushes.Red, 10, 160);
            gfx.DrawString(Instance.MousePosition().Y.ToString(), SystemFonts.DefaultFont, Brushes.Red, 10, 172);
            gfx.DrawString(fuel.ToString(), SystemFonts.DefaultFont, Brushes.Red, 10, 184);

        }
        protected override void Update()
        {
            hitbox = new Rectangle((int)position.X, (int)position.Y, plrWidth, plrHeight);

            //  Initializing
            bool canLeft = true, canRight = true;
            float moveSpeed = 0.15f;
            float maxSpeed = 3f;
            float boosted = 4f;
            float stopSpeed = moveSpeed * 2f;
            float jumpSpeed = maxSpeed * 2f * (!clamp ? boosted : 1f), fallSpeed = 0.917f;
            
            //  Clamp
            if (velocity.X > maxSpeed)
                velocity.X = maxSpeed;
            if (velocity.X < -maxSpeed)
                velocity.X = -maxSpeed;
            if (velocity.Y > maxSpeed)
                velocity.Y = maxSpeed;
            if (velocity.Y < -maxSpeed)
                velocity.Y = -maxSpeed;

            //  Border
            canLeft = position.X >= 1;
            canRight = position.X < width - 1;
            
            //  Positioning
            position.X += velocity.X;
            position.Y += velocity.Y;
            
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
                if (!colDown)
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
            if (colUp)
                velocity.Y = 0f;

            //  Movement speed set
            if (velocity.X < moveSpeed && velocity.X > -moveSpeed)
                velocity.X = 0f;
            if (velocity.Y < moveSpeed && velocity.Y > -moveSpeed)
                velocity.Y = 0f;

            //  Brush interaction
            foreach (SquareBrush sq in square.Where(t => t != null))
            {
                if (colRight = sq.Hitbox.Contains(position.X + plrWidth, position.Y))
                {
                    position.X = sq.X - plrWidth;
                }
                if (colDown = sq.Hitbox.Contains(position.X + plrWidth / 2, position.Y + plrHeight))
                {
                    position.Y = sq.Y - plrHeight;
                }
                if (colUp = sq.Hitbox.Contains(position.X + plrWidth / 2, position.Y))
                {
                    position.Y = sq.Y + sq.Height;
                    break;
                }
                if (colLeft = sq.Hitbox.Contains(position.X, position.Y))
                {
                    position.X = sq.X + sq.Width;
                    break;
                }
            }
            AuxMovement();
        }
        private int fuel = 3;
        private int maxFuel = 3;
        private void AuxMovement()
        {
            clamp = fuel <= 0;
            Vector2 mouse = Instance.MousePosition();
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                if (fuel > 0 && fuel-- <= 3)
                {
                    foreach (Dust dust in Main.dust.Where(t => t != null && t.active))
                    {
                        if (dust.type == Dust.Waypoint.Green)
                        {
                            if (dust.hitbox.Contains((int)mouse.X, (int)mouse.Y))
                            {
                                var speed = AngleToSpeed(AngleTo(position, mouse), 3f);
                                velocity.X += speed.X;
                                velocity.Y += speed.Y;
                                break;
                            }
                        }
                    }
                }
            }
            else if (fuel < maxFuel)
                fuel++;
        }
        public static Vector2 AngleToSpeed(float angle, float amount)
        {
            float cos = (float)(amount * Math.Cos(angle));
            float sine = (float)(amount * Math.Sin(angle));
            return new Vector2(cos, sine);
        }
        public static float AngleTo(Vector2 from, Vector2 to)
        {
            return (float)Math.Atan2(to.Y - from.Y, to.X - from.X);
        }
        private new bool KeyUp(Key key)
        {
            return Keyboard.IsKeyUp(key);
        }
        private new bool KeyDown(Key key)
        {
            return Keyboard.IsKeyDown(key);
        }
        
        public bool IsMoving()
        {
            return KeyDown(Key.W) || KeyDown(Key.A) || KeyDown(Key.S) || KeyDown(Key.D) || velocity.X > 0f || velocity.X < 0f || velocity.Y > 0f || velocity.Y < 0f;
        }
    }
}
