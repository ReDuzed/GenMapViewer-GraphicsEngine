using RUDD;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Terraria.Utilities;

namespace GenMapViewer
{
    public struct PointData
    {
        public bool active;
        public int x;
        public int y;
        public System.Drawing.Color type;
        public PointData(bool active, int x, int y, System.Drawing.Color type)
        {
            this.active = active;
            this.x = x;
            this.y = y;
            this.type = type;
        }
    }
    public class Draw
    {
        public const float radian = 0.017f;
        public static float Circumference(float distance)
        {
            return radian * (45f / distance);
        }
        public float radians(float distance)
        {
            return radian * (45f / distance);
        }
    }
    public class WorldGen
    {
        public static UnifiedRandom genRand
        {
            get { return Main.rand; }
        }
    }
    public struct Tile
    {
        public static Bitmap bmp;
        public void type(int x, int y, ushort type)
        {
            bmp.SetPixel(x, y, Main.types[type]);
        }
        public byte liquid;
        public bool active()
        {
            return Main.rand.Next(0, 1) == 1;
        }
        public bool active(bool active)
        {
            return active;
        }
    }
    public class TileID
    {
        public static byte
            Empty = 0,
            Ash = 1,
            Ore = 2,
            Stone = 3,
            Plant = 4;
    }
    public struct Rectangle
    {
        public int x;
        public int y;
        public int Width;
        public int Height;
        public Rectangle(int x, int y, int width, int height)
        {
            this.x = x;
            this.y = y;
            this.Width = width;
            this.Height = height;
        }
        public static Rectangle Empty
        {
            get { return new Rectangle(); }
        }
        public bool Intersects(Rectangle other)
        {
            return other.x >= x && other.x <= x + Width && other.y >= y && other.y <= y + Height;
        }
        public bool TopCollide(float X, float Y)
        {
            return X >= this.x && X <= this.x + Width && Y >= this.y;
        }
        public bool Collision(float X, float Y)
        {
            return X >= this.x && X <= this.x + Width && Y >= this.y && Y <= this.y + Height;
        }
        public bool RightCollide(float X, float Y)
        {
            return X >= this.x && Y >= this.y && Y <= this.y + Height;
        }
        public bool LeftCollide(float X, float Y)
        {
            return X <= this.x + Width && Y >= this.y && Y <= this.y + Height;
        }
        public bool Contains(int X, int Y)
        {
            for (int m = x; m < x + Width; m++)
            {
                for (int n = y; n < y + Height; n++)
                {
                    if (X >= x && X <= x + Width)
                    {
                        if (Y >= y && Y <= y + Height)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
    public class Dust : Main
    {
        public bool active;
        public int timeLeft;
        public int maxLife;
        public Dust(float x, float y)
        {
            this.position = new Vector2(x, y);
        }
        public Dust(float x, float y, int width, int height, Color color)
        {
            this.position = new Vector2(x, y);
            this.color = color;
            this.width = width;
            this.height = height;
        }
        public float x
        {
            get { return position.X; }
        }
        public float y
        {
            get { return position.Y; }
        }
        public new int width = 16, height = 16;
        public int z = -1;
        public Vector2 position;
        public Color color;
        public static Dust NewDust(int x, int y, int width, int height, Color color, int maxSeconds)
        {
            int index = 1000;
            for (int i = 0; i < Main.dust.Length; i++)
            {
                if (Main.dust[i] == null || !Main.dust[i].active)
                {
                    index = i;
                    break;
                }
                if (i == index)
                {
                    return Main.dust[index];
                }
            }
            Main.dust[index] = new Dust(x, y, width, height, color);
            Main.dust[index].active = true;
            Main.dust[index].maxLife = maxSeconds;
            return Main.dust[index];
        }
        int ticks;
        protected override void Update()
        {
            if (ticks++ % Main.frameRate == 0)
                timeLeft++;
            if (timeLeft > maxLife)
                active = false;
        }
        public void Draw(Bitmap bmp, Graphics gfx)
        {
            if (active)
            {
                gfx.DrawRectangle(new Pen(color), new System.Drawing.Rectangle((int)x - width / 2, (int)y - width / 2, width, height));
            }
        }
    }
}
