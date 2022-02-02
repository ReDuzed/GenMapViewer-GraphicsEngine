using RUDD;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GenMapViewer
{
    public class SquareBrush : Main
    {
        public int X;
        public int Y;
        public new int Width;
        public new int Height;
        public bool Active = true;
        public bool Square = true;
        public int whoAmI;
        public Rectangle Hitbox
        {
            get { return new Rectangle(X, Y, Width, Height); }
        }
        public bool door(bool door)
        {
            return false;
        }
        public bool active(bool active)
        {
            Active = active;
            return active;
        }
        public SquareBrush(int x, int y, int width, int height)
        {
            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Height = height;
        }
        public void Collision(Player player, int buffer = 4)
        {
            if (Hitbox.Contains(player.position.X, player.position.Y, Player.plrWidth, Player.plrHeight))
                player.collide = true;
            //  Directions
            if (Hitbox.Contains(player.position.X, player.position.Y - buffer, Player.plrWidth, 2))
                player.colUp = true;
            if (Hitbox.Contains(player.position.X, player.position.Y + Player.plrHeight + buffer, Player.plrWidth, 2))
                player.colDown = true;
            if (Hitbox.Contains(player.position.X + Player.plrWidth + buffer, player.position.Y, 2, Player.plrHeight))
                player.colRight = true;
            if (Hitbox.Contains(player.position.X - buffer, player.position.Y, 2, Player.plrHeight))
                player.colLeft = true;
        }
        protected override void PreDraw(Bitmap bmp, Graphics gfx)
        {
            if (!Active)
                return;
            gfx.FillRectangle(Brushes.Red, new System.Drawing.Rectangle(Hitbox.x, Hitbox.y, Width, Height));

            if (Square)
            {
                gfx.DrawPolygon(Pens.White, new Point[]
                {
                    new Point(X, Y),
                    new Point(X + Width, Y),
                    new Point(X, Y + Height)
                });
                gfx.DrawPolygon(Pens.White, new Point[]
                {
                    new Point(X + Width, Y),
                    new Point(X + Width, Y + Height),
                    new Point(X, Y + Height)
                });
            }
        }
    }
    public class TriangleBrush : Main
    {
        public int X, Y, width, height;
        public bool active = true;
        public int whoAmI;
        public bool flag;
        public TriangleBrush(int x, int y, int width, int height, bool flag)
        {
            this.X = x;
            this.Y = y;
            this.width = width;
            this.height = height;
            this.flag = flag;
        }
        public Rectangle Hitbox
        {
            get { return new Rectangle(X, Y, width, height); }
        }
        protected override void PreDraw(Bitmap bmp, Graphics gfx)
        {
            if (!active)
                return;
            if (flag)
            {
                gfx.DrawPolygon(Pens.White, new Point[]
                {
                    new Point(X, Y),
                    new Point(X + width, Y),
                    new Point(X, Y + height)
                });
            }
            else
            {
                gfx.DrawPolygon(Pens.White, new Point[]
                {
                    new Point(X + width, Y + height),
                    new Point(X + width, Y),
                    new Point(X, Y + height)
                });
            }
        }
        public void Collision(Player player, int buffer = 4)
        {
            if (Hitbox.Contains(player.position.X, player.position.Y, Player.plrWidth, Player.plrHeight))
                player.collide = true;
            //  Directions
            if (Hitbox.Contains(player.position.X, player.position.Y - buffer, Player.plrWidth, 2))
                player.colUp = true;
            if (Hitbox.Contains(player.position.X, player.position.Y + Player.plrHeight + buffer, Player.plrWidth, 2))
                player.colDown = true;
            if (Hitbox.Contains(player.position.X + Player.plrWidth + buffer, player.position.Y, 2, Player.plrHeight))
                player.colRight = true;
            if (Hitbox.Contains(player.position.X - buffer, player.position.Y, 2, Player.plrHeight))
                player.colLeft = true;
        }
    }
    

    public class Darkness : Entity
    {
        public static Darkness[,] overlay = new Darkness[100, 45];
        public bool statusFx = true;
        public static WaitHandle wait;
        public static Darkness[,] NewOverlay(int width = 800, int height = 450)
        {
            for (int m = 0; m < overlay.GetLength(0); m++)
            {
                for (int n = 0; n < overlay.GetLength(1); n++)
                {
                    overlay[m, n] = new Darkness();
                    overlay[m, n].position = new Vector2(m * 8, n * 8);
                }
            }
            return overlay;
        }
        internal void Initialize()
        {
            overlay = Darkness.NewOverlay();
        }
        public void Draw(Bitmap bmp, Graphics gfx)
        {
            if (statusFx)
            {
                if (!Proximity(Main.LocalPlayer.Center, 48f))
                {
                    gfx.FillRectangle(Brushes.Black, position.X, position.Y, 8, 8);
                }
            }
        }
    }
}
