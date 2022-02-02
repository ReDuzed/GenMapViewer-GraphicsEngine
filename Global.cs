using RUDD;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Terraria.Utilities;
using GenMapViewer.NPCs;
using System.Drawing.Text;

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
        private bool Intersects(Rectangle other)
        {
            return other.x >= x && other.x <= x + Width && other.y >= y && other.y <= y + Height;
        }
        public bool Intersect(Rectangle other)
        {
            int width = Width / (Width / 4);
            int height = Height / (Height / 4); 
            for (int i = x; i < x + width; i += width)
            {
                for (int j = y; j < y + height; j += height)
                {
                    if (i >= other.x && i <= other.x + other.Width && j >= other.y && j <= other.y + other.Height)
                        return true;
                }
            }
            return false;
        }
        public RectangleF GetRectangleF()
        {
            return new RectangleF(x, y, Width, Height);
        }
        public System.Drawing.Rectangle GetSDRectangle()
        {
            return new System.Drawing.Rectangle(x, y, Width, Height);
        }
        public bool TopCollide(float X, float Y)
        {
            return X >= this.x && X <= this.x + Width && Y >= this.y;
        }
        public bool Contains(float X, float Y, int width, int height)
        {
            
            return 
                X + width >= this.x && 
                X <= this.x + Width && 
                Y + height >= this.y && 
                Y <= this.y + Height;
        }
        public bool Contains(float X, float Y)
        {
            return
                X >= this.x &&
                X <= this.x + Width &&
                Y >= this.y &&
                Y <= this.y + Height;
        }
        public bool Contains(Vector2 v2)
        {
            return
                v2.X >= this.x &&
                v2.X <= this.x + Width &&
                v2.Y >= this.y &&
                v2.Y <= this.y + Height;
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
}
