using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RUDD;
using Terraria.Utilities;

namespace GenMapViewer.Biome
{
    /*
    public class MagnoDen
    {
        public static bool active = true;
        public bool complete;
        public static bool miniBiome;
        private int width
        {
            get { return (int)800; }
        }
        private int leftBounds
        {
            get { return 50; }
        }
        private int upperBounds
        {
            get { return 50; }
        }
        public int maxY
        {
            get { return 450; }
        }
        private int centerX
        {
            get { return width / 2; }
        }
        private int centerY
        {
            get { return 450 / 2; }
        }
        public int lookFurther;
        public static int whoAmI = 0;
        public static readonly int max = 200;
        private readonly int border = 3;
        private readonly int cave = 1;
        private int x
        {
            get { return (int)center.X; }
        }
        private int y
        {
            get { return (int)center.Y; }
        }
        public int points;
        private int cycle;
        private int id;
        private int X = 800;
        private int Y = 450;
        private int Width;
        private int Height;
        private int iterate;
        public Vector2 center;
        public static Vector2 origin;
        public static Rectangle[] bounds;
        public static Dictionary<Vector2, int> plots = new Dictionary<Vector2, int>();
        public static MagnoDen[] mDen;
        public void Start(MagnoDen den, bool miniBiome = true, int iterate = 8)
        {
            active = true;
            this.iterate = iterate;
            mDen = new MagnoDen[max];
            center = new Vector2(centerX / 2, centerY);
            //  bounds = new Rectangle(leftBounds, centerY, leftBounds + width / 3, centerY + width / 4);
            origin = center;
            bounds = new Rectangle[max / iterate];
            for (int i = 0; i < bounds.Length; i++)
                bounds[i] = Rectangle.Empty;
            mDen[whoAmI] = den;
            mDen[whoAmI].id = whoAmI;
            mDen[whoAmI].center = center;
        }
        public void Update()
        {
            GenerateNewMiner();
        }
        public bool StandardMove()
        {
            int size = WorldGen.genRand.Next(1, 4);
            int rand = WorldGen.genRand.Next(1, 5);
            if (WorldGen.genRand.Next(1, 4) == 1 && Main.tile[x + 1 + lookFurther, y].active())
            {
                center.X += 1f;
                lookFurther = 0;
                points++;
                DigPlot(size);
            }
            if (WorldGen.genRand.Next(1, 4) == 1 && Main.tile[x - 1 - lookFurther, y].active())
            {
                center.X -= 1f;
                lookFurther = 0;
                points++;
                DigPlot(size);
            }
            if (WorldGen.genRand.Next(1, 4) == 1 && Main.tile[x, y + 1 + lookFurther].active() && center.Y < maxY)
            {
                center.Y += 1f;
                lookFurther = 0;
                points++;
                DigPlot(size);
            }
            if (WorldGen.genRand.Next(1, 4) == 1 && Main.tile[x, y - 1 - lookFurther].active() && center.Y > upperBounds)
            {
                center.Y -= 1f;
                lookFurther = 0;
                points++;
                DigPlot(size);
            }
            if (!Main.tile[x + 1 + lookFurther, y].active() &&
                !Main.tile[x - 1 - lookFurther, y].active() &&
                !Main.tile[x, y + 1 + lookFurther].active() &&
                !Main.tile[x, y - 1 - lookFurther].active())
                lookFurther++;
            if (!plots.ContainsKey(center))
                plots.Add(center, size);
            if (points > 10)
                return true;
            return false;
        }
        public void VerticalMove()
        {
            Vector2 old = center;
            while (center.X == old.X && center.Y == old.Y)
            {
                int rand = WorldGen.genRand.Next(1, 5);
                int x = (int)center.X;
                int y = (int)center.Y;
                switch (rand)
                {
                    case 1:
                        do
                        {
                            center.X += 1f;
                            lookFurther++;
                        } while (!Main.tile[x + 1 + lookFurther, y].active()
                                && x < Main.rightWorld / 16);
                        break;
                    case 2:
                        do
                        {
                            center.X -= 1f;
                            lookFurther++;
                        } while (!Main.tile[x - 1 - lookFurther, y].active()
                                && x > 50);
                        break;
                    case 3:
                        do
                        {
                            center.Y += 1f;
                            lookFurther++;
                        } while (!Main.tile[x, y + 1 + lookFurther].active()
                                && y < Main.bottomWorld / 16);
                        break;
                    case 4:
                        do
                        {
                            center.Y -= 1f;
                            lookFurther++;
                        } while (!Main.tile[x, y - 1 - lookFurther].active()
                                && y > maxY);
                        break;
                    default:
                        break;
                }
                if (lookFurther % 2 == 0)
                    PlaceWater(center);
                lookFurther = 0;
            }
        }
        public bool AverageMove()
        {
            Vector2 old = center;
            if (WorldGen.genRand.Next(1, 4) == 1) center.X += 1f;
            if (WorldGen.genRand.Next(1, 4) == 1) center.X -= 1f;
            if (WorldGen.genRand.Next(1, 4) == 1) center.Y += 1f;
            if (WorldGen.genRand.Next(1, 4) == 1) center.Y -= 1f;
            return center.X != old.X && center.Y != old.Y;
        }
        public MagnoDen GenerateNewMiner()
        {
            if (this == mDen[0])
            {
                whoAmI++;
                if (whoAmI == max)
                    FinalDig();
                if (whoAmI < max)
                {
                    mDen[whoAmI] = new MagnoDen();
                    mDen[whoAmI].center = NewPosition(mDen[whoAmI - 1].center);
                }
                else
                    Terminate();
            }
            return mDen[Math.Min(whoAmI, max - 1)];
        }
        public Vector2 NewPosition(Vector2 previous)
        {
            return new Vector2(previous.X, origin.Y);
        }
        public static bool Inbounds(int x, int y)
        {
            return x < Main.maxTilesX - 50 && x > 50 && y < Main.maxTilesY - 200 && y > 50;
        }
        public void DigPlot(int size)
        {
            for (int i = (int)center.X - size; i < (int)center.X + size; i++)
                for (int j = (int)center.Y - size; j < (int)center.Y + size; j++)
                {
                    if (Inbounds(i, j))
                    {
                        if (WorldGen.genRand.Next(60) == 0)
                            PlaceWater(new Vector2(i, j));
                        Main.tile[i, j].type(i, j, TileID.Stone);
                        Main.tile[i, j].active(true);
                        //  WorldGen.PlaceTile(i, j, TileID.PearlstoneBrick, false, true);
                    }
                }
        }
        public void FinalDig()
        {
            var v2 = plots.Keys.ToArray();
            var s = plots.Values.ToArray();
            for (int k = 1; k < v2.Length; k++)
            {
                int x = (int)v2[k].X;
                int y = (int)v2[k].Y;
                for (int i = x - s[k] * border; i < x + s[k] * border; i++)
                {
                    for (int j = y - s[k] * border; j < y + s[k] * border; j++)
                    {
                        Main.tile[i, j].type(i, j, TileID.Stone);
                        Main.tile[i, j].active(true);
                        //  WorldGen.PlaceTile(i, j, TileID.PearlstoneBrick, true, true);
                        //  WorldGen.KillWall(i, j);
                    }
                }
            }
            for (int l = 1; l < v2.Length; l++)
            {
                int x = (int)v2[l].X;
                int y = (int)v2[l].Y;
                for (int i = (int)x - s[l]; i < (int)x + s[l]; i++)
                    for (int j = (int)y - s[l]; j < (int)y + s[l]; j++)
                    {
                        if (WorldGen.genRand.Next(60) == 0)
                            PlaceWater(new Vector2(i, j));
                        Main.tile[i, j].type(i, j, TileID.Stone);
                        Main.tile[i, j].active(false);
                        //  WorldGen.KillTile(i, j, false, false, true);
                    }
            }
        }
        public void PlaceWater(Vector2 position)
        {
            int x = (int)position.X;
            int y = (int)position.Y;
            if (Inbounds(x, y))
                Main.tile[x, y].liquid = 60;
        }
        public void CheckComplete(int divisor = 2)
        {
            cycle++;
            if (cycle == max / divisor)
            {
                whoAmI++;
                if (whoAmI < mDen.Length)
                {
                    mDen[whoAmI] = new MagnoDen();
                    mDen[whoAmI].id = whoAmI;
                    if (miniBiome)
                    {
                        if (whoAmI % iterate == 0)
                            mDen[whoAmI].center = NewPosition(mDen[whoAmI - 1].center);
                        else mDen[whoAmI].center = mDen[whoAmI - 1].center;
                    }
                    else mDen[whoAmI].center = mDen[whoAmI - 1].center;
                }
            }
        }
        public void GetBounds()
        {
            int count = 0;
            foreach (MagnoDen m in mDen)
                if (m != null && m.center.X != Vector2.Zero.X)
                {
                    if (m.center.X < X)
                        X = (int)m.center.X;
                    if (m.center.Y < Y)
                        Y = (int)m.center.Y;
                    if (m.center.X - X > Width)
                        Width = (int)m.center.X - X;
                    if (m.center.Y - Y > Height)
                        Height = (int)m.center.Y - Y;
                    count++;
                    if (miniBiome)
                    {
                        if (count % iterate == 0)
                        {
                            bounds[count] = (new Rectangle(X, Y, Width, Height));
                            X = 0;
                            Y = 0;
                            Width = 0;
                            Height = 0;
                        }
                    }
                    else bounds[0] = new Rectangle(X, Y, Width, Height);
                }
        }
        public void Terminate()
        {
            Unload();
            active = false;
            for (int i = 0; i < mDen.Length; i++)
                mDen[i] = null;
        }
        protected void Unload()
        {
            origin = Vector2.Zero;
            plots.Clear();
        }
    }
    /*
    private void Spiral()
    {
        float radius = 1f;
        for (float r = 0; r < Math.PI * 2f; r += Draw.radian)
        {
            radius += Draw.radian * 10f;
            float cos = center.X + radius * (float)Math.Cos(r);
            float sin = center.Y + radius * (float)Math.Sin(r);
            bmp.SetPixel((int)cos, (int)sin, tile[TileID.Stone]);
        }
    }
    public void Generate(Bitmap bmp)
    {
        Vector2 center = new Vector2(width / 2, height / 2);
        float radius = 200f;
        for (float r = 0; r < Math.PI * 2f; r += Draw.radian)
        {
            for (float n = 0; n < radius; n += Draw.Circumference(radius))
            {
                float cos = center.X + n * (float)Math.Cos(r);
                float sin = center.Y + n * (float)Math.Sin(r);
                if (n > radius * 0.75f)
                {
                    if (rand.NextDouble() > 0.99875d)
                    {
                        bmp.SetPixel((int)cos, (int)sin, tile[TileID.Stone]);
                    }
                }
                else bmp.SetPixel((int)cos, (int)sin, tile[TileID.Stone]);
            }
        }

        int length = 14;
        List<PointData> points = new List<PointData>();
        Vector2[] corner = new Vector2[length];
        Rectangle bounds = new Rectangle(width / 4, height / 4, width - width / 2, height - height / 2);
        for (int i = 0; i < corner.Length; i++)
        {
            while (!bounds.Contains((int)corner[i].X, (int)corner[i].Y))
            {
                corner[i] = new Vector2(rand.Next(50, width - 100), rand.Next(50, height - 120));
            }
        }
        Rectangle[] rec = new Rectangle[length];
        for (int i = 0; i < rec.Length; i++)
        {
            int width = rand.Next(40, 90);
            int height = rand.Next(30, 80);
            rec[i] = new Rectangle((int)corner[i].X - width / 2, (int)corner[i].Y - height / 2, width, height);
            for (int m = rec[i].x; m < rec[i].x + rec[i].Width; m++)
            {
                for (int n = rec[i].y; n < rec[i].y + rec[i].Height; n++)
                {
                    points.Add(new PointData(rand.NextDouble() > 0.90f, m, n, tile[TileID.Ash]));
                }
            }
        }
    }

class BoxGen
{
    private void Generate(Bitmap bmp)
    {
        Vector2[] point = new Vector2[8];
        int[] length = new int[point.Length];
        for (int i = 0; i < point.Length; i++)
        {
            int x = rand.Next(50, width - 100);
            int y = rand.Next(50, height - 60);
            point[i] = new Vector2(x, y);
            length[i] = rand.Next(30, 100);
        }
        bool flag;
        Vector2[] stalagmite = new Vector2[point.Length];
        for (int m = 0; m < point.Length; m++)
        {
            flag = true;
            Vector2 start = point[m];
            int height = rand.Next(20, 40);
            int ceiling = (int)start.X + length[m] / 2 + rand.Next(-5, 5);
            for (int i = (int)start.X; i < (int)start.X + length[m]; i++)
            {
                if (flag && i == ceiling)
                {
                    stalagmite[m] = new Vector2(i, point[m].Y);
                    flag = false;
                }
                for (int j = (int)start.Y; j <= (int)start.Y + height; j++)
                {
                    bmp.SetPixel(i, j, types[TileID.Empty]);
                    if (j == start.Y + height)
                    {
                        int k = rand.Next(1, 4);
                        for (int n = j; n < j + k; n++)
                        {
                            bmp.SetPixel(i, n, types[TileID.Ash]);
                        }
                    }
                }
            }
        }
        for (int m = 0; m < stalagmite.Length; m++)
        {
            int k = rand.Next(4, 10);
            for (int j = (int)stalagmite[m].Y; j < (int)stalagmite[m].Y + k; j++)
            {
                for (int i = (int)stalagmite[m].X - k / 2; i < (int)stalagmite[m].X + k / 2; i++)
                {
                    bmp.SetPixel(i, j, types[TileID.Ore]);
                }
            }
        }
    }
}
//  3/9 4:50 pm caver
class SaveState
{
    public void Generate(Bitmap bmp)
    {
        List<Vector2> miner = new List<Vector2>();
        Vector2 start = new Vector2(50, height / 2);
        float radius = 20f;
        float tunnel = 10f;
        float move = radius / 4f;
        int ticks = 0;
        int max = 50;
        bool flag = false;
        while (start.X < width / 2)
        {
            for (float r = 0; r < Math.PI * 2f; r += Draw.radian)
            {
                for (float n = 0; n < radius; n += Draw.Circumference(radius))
                {
                    if (n >= tunnel)
                    {
                        float cos = start.X + n * (float)Math.Cos(r);
                        float sin = start.Y + n * (float)Math.Sin(r);
                        bmp.SetPixel((int)cos, (int)sin, types[TileID.Stone]);
                        if (flag && r <= Math.PI && r >= 0f)
                        {
                            bmp.SetPixel((int)cos, (int)sin, types[TileID.Ash]);
                        }
                    }
                }
            }
            if (!flag && rand.NextDouble() >= 0.75f)
            {
                flag = true;
            }
            if (flag)
            {
                if (ticks++ > max)
                {
                    flag = false;
                    ticks = 0;
                }
                start.Y += flag && !(start.Y + radius >= height || start.Y - radius <= 0f) ? -1 * move : move;
            }
            miner.Add(start);
            start.X += move;
        }
        foreach (var loc in miner)
        {
            tunnel = (float)rand.Next(8, 15);
            for (float r = 0; r < Math.PI * 2f; r += Draw.radian)
            {
                for (float n = 0; n < radius; n += Draw.Circumference(radius))
                {
                    if (n < tunnel)
                    {
                        float cos = loc.X + n * (float)Math.Cos(r);
                        float sin = loc.Y + n * (float)Math.Sin(r);
                        bmp.SetPixel((int)cos, (int)sin, types[TileID.Empty]);
                    }
                }
            }
        }

        // 3/9 7:00 pm 2nd caver
        public void Generate(Bitmap bmp)
        {
            List<Vector2> miner = new List<Vector2>();
            Vector2 start = new Vector2(50, height / 2);
            float radius = 20f;
            float tunnel = 10f;
            float move = radius / 4f;
            int ticks = 0, ticks2 = 0;
            int max = 20, max2 = 25;
            bool flag = false, flag2 = false;
            bool level = true;
            bool ash = false;
            while (start.X < width / 2)
            {
                for (float r = 0; r < Math.PI * 2f; r += Draw.radian)
                {
                    for (float n = 0; n < radius; n += Draw.Circumference(radius))
                    {
                        if (n >= tunnel)
                        {
                            float cos = start.X + n * (float)Math.Cos(r);
                            float sin = start.Y + n * (float)Math.Sin(r);
                            bmp.SetPixel((int)cos, (int)sin, types[TileID.Stone]);
                            if (ash && flag && r <= Math.PI && r >= 0f)
                            {
                                bmp.SetPixel((int)cos, (int)sin, types[TileID.Ash]);
                            }
                        }
                    }
                }
                if (!flag && rand.NextDouble() >= 0.99f)
                {
                    flag = true;
                }
                if (!level && rand.NextDouble() >= 0.90f)
                {
                    level = true;
                }
                if (!ash && rand.NextDouble() >= 0.95f)
                {
                    ash = true;
                }
                if (flag)
                {
                    if (ticks++ > max)
                    {
                        flag = false;
                        ticks = 0;
                    }
                }
                if (level)
                {
                    if (ticks++ > max)
                    {
                        level = false;
                        ticks = 0;
                    }
                }
                if (ash)
                {
                    if (ticks2++ > max2)
                    {
                        ash = false;
                        ticks2 = 0;
                    }
                }
                if (!level && start.Y < height - radius && start.Y > 0f + radius)
                {
                    start.Y += flag ? -1 * move : move;
                }
                miner.Add(start);
                start.X += move;
            }
            flag = false;
            ticks = 0;
            bool back = true, forward = false;
            Vector2 branch = start;
            branch.Y = height - radius;
            while (branch.Y > radius * 2f)
            {
                for (float r = 0; r < Math.PI * 2f; r += Draw.radian)
                {
                    for (float n = 0; n < radius; n += Draw.Circumference(radius))
                    {
                        if (n >= tunnel)
                        {
                            float cos = branch.X + n * (float)Math.Cos(r);
                            float sin = branch.Y + n * (float)Math.Sin(r);
                            if (cos < width && sin < height && sin > radius)
                            {
                                bmp.SetPixel((int)cos, (int)sin, types[TileID.Stone]);
                                if (ash && flag && r <= Math.PI && r >= 0f)
                                {
                                    bmp.SetPixel((int)cos, (int)sin, types[TileID.Ash]);
                                }
                            }
                        }
                    }
                }
                if (!flag && rand.NextDouble() >= 0.995f)
                {
                    flag = true;
                }
                if (!level && rand.NextDouble() >= 0.90f)
                {
                    level = true;
                }
                if (!ash && rand.NextDouble() >= 0.95f)
                {
                    ash = true;
                }
                if (flag)
                {
                    if (ticks++ > max)
                    {
                        flag = false;
                        ticks = 0;
                    }
                }
                if (level)
                {
                    if (ticks++ > max)
                    {
                        level = false;
                        ticks = 0;
                    }
                }
                if (ash)
                {
                    if (ticks2++ > max2)
                    {
                        ash = false;
                        ticks2 = 0;
                    }
                }
                if (!level && start.Y < height - radius && start.Y > 0f + radius)
                {
                    branch.X += flag ? -1 * move : move;
                }
                miner.Add(branch);
                branch.Y -= move;
            }
            while (true)
            {
                if (!flag && rand.NextDouble() >= 0.50f)
                {
                    flag = true;
                    flag2 = true;
                }
                for (float r = 0; r < Math.PI * 2f; r += Draw.radian)
                {
                    for (float n = 0; n < radius; n += Draw.Circumference(radius))
                    {
                        if (n >= tunnel)
                        {
                            float cos = start.X + n * (float)Math.Cos(r);
                            float sin = start.Y + n * (float)Math.Sin(r);
                            bmp.SetPixel((int)cos, (int)sin, types[TileID.Stone]);
                            if (r <= Math.PI && r >= 0f)
                            {
                                bmp.SetPixel((int)cos, (int)sin, types[TileID.Ash]);
                            }
                        }
                    }
                }
                if (ticks++ < max)
                {
                    if (start.Y >= height - radius)
                    {
                        start.Y -= move;
                        start.X -= move;
                    }
                    else if (start.Y <= 0f + radius)
                    {
                        start.Y += move;
                        start.X -= move;
                    }
                    else
                    {
                        if (flag2)
                        {
                            if (back)
                            {
                                start.Y -= move;
                                start.X -= move;
                            }
                            else if (!forward)
                            {
                                start.X -= move;
                            }
                            else
                            {
                                start.X += move;
                            }
                        }
                        else
                        {
                            if (back)
                            {
                                start.Y += move;
                                start.X -= move;
                            }
                            else if (!forward)
                            {
                                start.X -= move;
                            }
                            else
                            {
                                start.X += move;
                            }
                        }
                    }
                }
                else
                {
                    if (forward)
                    {
                        break;
                    }
                    if (!back)
                    {
                        forward = true;
                        max = 50;
                    }
                    back = false;
                    ticks = 0;
                }
                miner.Add(start);
            }
            max = 20;
            ticks = 0;
            while (start.X < width - radius)
            {
                for (float r = 0; r < Math.PI * 2f; r += Draw.radian)
                {
                    for (float n = 0; n < radius; n += Draw.Circumference(radius))
                    {
                        if (n >= tunnel)
                        {
                            float cos = start.X + n * (float)Math.Cos(r);
                            float sin = start.Y + n * (float)Math.Sin(r);
                            bmp.SetPixel((int)cos, (int)sin, types[TileID.Stone]);
                            if (ash && flag && r <= Math.PI && r >= 0f)
                            {
                                bmp.SetPixel((int)cos, (int)sin, types[TileID.Ash]);
                            }
                        }
                    }
                }
                if (!flag && rand.NextDouble() >= 0.995f)
                {
                    flag = true;
                }
                if (!level && rand.NextDouble() >= 0.90f)
                {
                    level = true;
                }
                if (!ash && rand.NextDouble() >= 0.95f)
                {
                    ash = true;
                }
                if (flag)
                {
                    if (ticks++ > max)
                    {
                        flag = false;
                        ticks = 0;
                    }
                }
                if (level)
                {
                    if (ticks++ > max)
                    {
                        level = false;
                        ticks = 0;
                    }
                }
                if (ash)
                {
                    if (ticks2++ > max2)
                    {
                        ash = false;
                        ticks2 = 0;
                    }
                }
                if (!level && start.Y < height - radius && start.Y > 0f + radius)
                {
                    start.Y += flag ? -1 * move : move;
                }
                miner.Add(start);
                start.X += move;
            }
            foreach (var loc in miner)
            {
                tunnel = (float)rand.Next(8, 15);
                for (float r = 0; r < Math.PI * 2f; r += Draw.radian)
                {
                    for (float n = 0; n < radius; n += Draw.Circumference(radius))
                    {
                        if (n < tunnel)
                        {
                            float cos = loc.X + n * (float)Math.Cos(r);
                            float sin = loc.Y + n * (float)Math.Sin(r);
                            bmp.SetPixel((int)cos, (int)sin, types[TileID.Empty]);
                        }
                    }
                }
            }
        }
    }*/
}
