using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

using RUDD;

namespace GenMapViewer
{
    public class Player : Entity
    {
        #region variables
        private bool clamp;
        public const int plrWidth = 32, plrHeight = 48;
        const int buffer = 4;
        public bool colUp, colDown, colRight, colLeft, collide;
        public bool controlLeft, controlUp, controlRight, controlDown;
        public bool isAttacking;
        //  Initial values
        bool canLeft = true, canRight = true;
        float moveSpeed = 0.15f;
        float maxSpeed = 3f;
        float stopSpeed;
        public int health = 100, mana = 20;
        public const int MaxHealth = 100;
        int flag, flag2;
        #endregion
        internal void Initialize()
        {
            active = true;
        }

        internal void PreDraw(Bitmap bmp, Graphics gfx)
        {
        }
        internal void Update()
        {
            hitbox = new Rectangle((int)position.X, (int)position.Y, plrWidth, plrHeight);

            //  Initializing
            stopSpeed = moveSpeed * 2f;
            
            //  Collision
            if (colLeft)
            {
                if (controlRight)
                    position.X += moveSpeed * 2;
                if (controlUp)
                    velocity.Y -= moveSpeed;
                else if (controlDown)
                    velocity.Y += moveSpeed;
            }
            if (colRight)
            {
                if (controlLeft)
                    position.X -= moveSpeed * 2;
                if (controlUp)
                    velocity.Y -= moveSpeed;
                else if (controlDown)
                    velocity.Y += moveSpeed;
            }
            if (colDown)
            {
                if (controlUp)
                    position.Y -= moveSpeed * 2;
                if (controlLeft)
                    velocity.X -= moveSpeed;
                else if (controlRight)
                    velocity.X += moveSpeed;
            }
            if (colUp)
            {
                if (controlDown)
                    position.Y += moveSpeed * 2;
                if (controlLeft)
                    velocity.X -= moveSpeed;
                else if (controlRight)
                    velocity.X += moveSpeed;
            }

            //  Movement mechanic
            if (!IsMoving())
            {
                //  Stopping movement
                if (velocity.X > 0f && !controlRight)
                    velocity.X -= stopSpeed;
                if (velocity.X < 0f && !controlLeft)
                    velocity.X += stopSpeed;
                if (velocity.Y > 0f && !controlDown)
                    velocity.Y -= stopSpeed;
                if (velocity.Y < 0f && !controlUp)
                    velocity.Y += stopSpeed;
            }

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

            //  Movement speed set
            if (velocity.X < moveSpeed && velocity.X > -moveSpeed)
                velocity.X = 0f;
            if (velocity.Y < moveSpeed && velocity.Y > -moveSpeed)
                velocity.Y = 0f;

            //  Positioning
            if (IsMoving())
            {
                if (!colLeft && !colRight)
                    position.X += velocity.X;
                if (!colUp && !colDown)
                    position.Y += velocity.Y;
            }

            //  Controls
            if (controlRight = !KeyDown(Key.A) && KeyDown(Key.D))
            {
                // move right
                velocity.X += moveSpeed;
            }
            if (controlLeft = KeyDown(Key.A) && !KeyDown(Key.D))
            {
                // move left
                velocity.X -= moveSpeed;
            }
            if (controlDown = !KeyDown(Key.W) && KeyDown(Key.S))
            {
                // move down
                velocity.Y += moveSpeed;
            }
            if (controlUp = KeyDown(Key.W) && !KeyDown(Key.S))
            {
                // move up
                velocity.Y -= moveSpeed;
            }

            //  GUI Hotbar
            if (KeyDown(Key.D1) || KeyDown(Key.D2) || KeyDown(Key.D3) || KeyDown(Key.D4) || KeyDown(Key.D5))
            {
                foreach (GUI g in Main.gui.Where(t => t != null))
                    g.selected = false;
            }
            if (KeyDown(Key.D1))
                Main.gui[0].selected = true;
            else if (KeyDown(Key.D2))
                Main.gui[1].selected = true;
            else if (KeyDown(Key.D3))
                Main.gui[2].selected = true;
            else if (KeyDown(Key.D4))
                Main.gui[3].selected = true;
            else if (KeyDown(Key.D5))
                Main.gui[4].selected = true;
            
            //  Menu
            //OpenMenu();
        }

        public void OpenMenu()
        {
            if (Main.MouseDevice.LeftButton == MouseButtonState.Pressed && flag2 % 2 == 0)
            {
                flag2++;
                if (Main.gui[GUI.SkillMenu].hitbox.Contains(Main.MousePosition.X, Main.MousePosition.Y))
                {
                    foreach (GUI menu in Main.skill)
                    {
                        menu.active = !menu.active;
                    }
                }
            }
            if (Main.MouseDevice.LeftButton == MouseButtonState.Released && flag2 % 2 == 1)
                flag2 = 0;
            //  GUI
            if (isAttacking)
                return;
            if (flag % 2 == 0)
            {
                foreach (GUI gui in Main.gui.Where(t => t != null))
                {
                    if (gui.hitbox.Contains(Main.MousePosition.X, Main.MousePosition.Y))
                    {
                        return;
                    }
                }
                if (LeftMouse())
                { 
                    if (GUI.GetElement(GUI.ID.SwordSwing).selected)
                    {
                        Projectile.NewProjectile(0, 0, Projectile.SwordSwing, Color.White, 10);
                    }
                    if (GUI.GetElement(GUI.ID.SwordSwipe).selected)
                    {
                        Projectile.NewProjectile(0, 0, Projectile.SwordSwipe, Color.White, 60);
                    }
                }
                flag++;
            }
            if (Main.MouseDevice.LeftButton == MouseButtonState.Released && flag % 2 == 1)
                flag = 0;
        }
        private int fuel = 3;
        private int maxFuel = 3;
        private void AuxMovement()
        {
            //clamp = fuel <= 0;
            Vector2 mouse = Main.WorldMouse;
            if (Main.MouseDevice.LeftButton == MouseButtonState.Pressed)
            {
                foreach (Dust dust in Main.dust.Where(t => t != null && t.active))
                {
                    if (dust.type == Dust.Waypoint.Green)
                    {
                        if (dust.hitbox.Contains(mouse.X, mouse.Y))
                        {
                            var speed = AngleToSpeed(AngleTo(position, mouse), 5f);
                            velocity.X += speed.X;
                            velocity.Y += speed.Y;
                            break;
                        }
                    }
                }
            }
            else if (fuel < maxFuel)
                fuel++;
        }
        public bool KeyUp(Key key)
        {
            return Keyboard.IsKeyUp(key);
        }
        public bool KeyDown(Key key)
        {
            return Keyboard.IsKeyDown(key);
        }
        public bool IsMoving()
        {
            return (KeyDown(Key.W) || KeyDown(Key.A) || KeyDown(Key.S) || KeyDown(Key.D)) && (velocity.X > 0f || velocity.X < 0f || velocity.Y > 0f || velocity.Y < 0f) && (!colDown || !colLeft || !colRight || !colUp);
        }
        public bool Movement()
        {
            return (KeyDown(Key.W) || KeyDown(Key.A) || KeyDown(Key.S) || KeyDown(Key.D));
        }
        public bool LeftMouse()
        {
            return Main.MouseDevice.LeftButton == MouseButtonState.Pressed;
        }
        public bool RightMouse()
        {
            return Main.MouseDevice.RightButton == MouseButtonState.Pressed;
        }
    }
}
