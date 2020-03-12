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
        public static bool Intersects(Rectangle rec1, Rectangle rec2)
        {
            for (int m = rec1.x; m < rec1.x + rec1.Width; m++)
            {
                for (int n = rec1.y; n < rec1.y + rec1.Height; n++)
                {
                    if (rec2.x <= m && rec2.x + rec2.Width >= m)
                    {
                        if (rec2.y <= n && rec2.y + rec2.Height >= n)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
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
