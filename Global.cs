using RUDD;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Terraria.Utilities;
using GenMapViewer.NPCs;

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
    public class Dust : Entity
    {
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
        public static Dust NewDust(int x, int y, int width, int height, int type, Color color, int maxLife)
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
            Main.dust[index].type = type;
            Main.dust[index].active = true;
            Main.dust[index].maxLife = maxLife;
            Main.dust[index].whoAmI = index;
            return Main.dust[index];
        }
        int ticks;
        protected override void Update()
        {
            Effect();
            if (ticks++ % Main.frameRate * 15 == 0)
                timeLeft++;
            if (timeLeft > maxLife)
            {
                Kill();
            }
        }
        public void Draw(Bitmap bmp, Graphics gfx)
        {
            if (active)
            {
                gfx.DrawRectangle(new Pen(color), new System.Drawing.Rectangle((int)x - width / 2, (int)y - width / 2, width, height));
            }
        }
        private new void Effect()
        {
            switch (type)
            {
                case 0:
                    color = Color.Green;
                    break;
                case 1:
                    color = Color.Red;
                    break;
                case 2:
                    color = Color.Yellow;
                    break;
            }
        }
        public void Kill()
        {
            Main.dust[whoAmI].active = false;
        }
    }

    public class NPC : Entity
    {
        public NPC()
        {

        }
        public static NPC NewNPC(int x, int y, int type, Color color, int maxLife)
        {
            int index = 500;
            for (int i = 0; i < Main.npc.Length; i++)
            {
                if (Main.npc[i] == null || !Main.npc[i].active)
                {
                    index = i;
                    break;
                }
                if (i == index)
                {
                    return Main.npc[index];
                }
            }
            Main.npc[index] = new NPC();
            Main.npc[index].position = new Vector2(x, y);
            Main.npc[index].type = type;
            Main.npc[index].active = true;
            Main.npc[index].maxLife = maxLife;
            Main.npc[index].color = color;
            Main.npc[index].whoAmI = index;
            Main.npc[index].Initialize();
            return Main.npc[index];
        }
        private new void Initialize()
        {
            switch (type)
            {
                case -1:
                    width = 176;
                    height = 192;
                    Name = "Necrosis";
                    texture = Bitmap.FromFile(NpcTexture(Name));
                    break;
            }
        }
        protected override void Update()
        {
            switch (type)
            {
                case -1:
                    hitbox = new Rectangle((int)position.X, (int)position.Y, width, height);
                    break;
            }
        }
        private int ticks;
        private Dust target;
        float moveSpeed = 0.15f;
        float maxSpeed = 3f;
        float stopSpeed
        {
            get { return moveSpeed; }
        }
        float jumpSpeed
        {
            get { return maxSpeed; }
        }
        public void AI()
        {
            switch (type)
            {
                case -1:
                    if (velocity.X > maxSpeed)
                        velocity.X = maxSpeed;
                    if (velocity.X < -maxSpeed)
                        velocity.X = -maxSpeed;
                    if (velocity.Y > maxSpeed)
                        velocity.Y = maxSpeed;
                    if (velocity.Y < -maxSpeed)
                        velocity.Y = -maxSpeed;
                    if (target == null || !target.active)
                        target = Main.dust.Where(t => t != null).ToArray()[0];
                    if (target.active)
                    {
                        position.X += velocity.X;
                        position.Y += velocity.Y;
                        if (!this.hitbox.Collision(target.Center.X, target.Center.Y))
                        {
                            var speed = AngleToSpeed((float)Math.Atan2(position.Y - target.position.Y, position.X - target.position.X) + 180f * Draw.radian, moveSpeed);
                            velocity.X += speed.X;
                            velocity.Y += speed.Y;
                        }
                        else target.Kill();
                    }
                    if (ticks++ >= 600)
                        ticks = 1;
                    break;
            }
        }
        public static Vector2 AngleToSpeed(float angle, float amount)
        {
            float cos = (float)(amount * Math.Cos(angle));
            float sine = (float)(amount * Math.Sin(angle));
            return new Vector2(cos, sine);
        }
        protected override void PreDraw(Bitmap bmp, Graphics gfx)
        {
            gfx.DrawImage(texture, new Point((int)position.X, (int)position.Y));
        }
        private string NpcTexture(string name)
        {
            return "Textures\\" + name + ".png";
        }
    }

    public class Projectile : Entity
    {
        private new void Initialize()
        {
            switch (type)
            {
                case -1:
                    width = 38;
                    height = 44;
                    timeLeft = maxLife;
                    Name = "Orb";
                    texture = Bitmap.FromFile(ProjTexture(Name));
                    break;
            }
        }
        private string ProjTexture(string name)
        {
            return "Textures\\" + name + ".png";
        }
        public static Projectile NewProjectile(int x, int y, int type, Color color, int maxTime)
        {
            int index = 1001;
            for (int i = 0; i < Main.projectile.Length; i++)
            {
                if (Main.projectile[i] == null || !Main.projectile[i].active)
                {
                    index = i;
                    break;
                }
                if (i == index)
                {
                    return Main.projectile[index];
                }
            }
            Main.projectile[index] = new Projectile();
            Main.projectile[index].position = new Vector2(x, y);
            Main.projectile[index].type = type;
            Main.projectile[index].active = true;
            Main.projectile[index].maxLife = maxTime;
            Main.projectile[index].color = color;
            Main.projectile[index].whoAmI = index;
            Main.projectile[index].Initialize();
            return Main.projectile[index];
        }
        public void AI()
        {
            switch (type)
            {
                case -1:
                    if (style == 0)
                    {

                    }
                    break;
            }
        }
        public static Vector2 AngleToSpeed(float angle, float amount)
        {
            float cos = (float)(amount * Math.Cos(angle));
            float sine = (float)(amount * Math.Sin(angle));
            return new Vector2(cos, sine);
        }
        protected override void Update()
        {
            if (timeLeft++ > maxLife)
            {
                Kill();
            }
        }
        public void Kill()
        {
            Main.projectile[whoAmI].active = false;
        }
        int ticks;
        int frameY;
        int totalFrames = 7;
        protected override void PreDraw(Bitmap bmp, Graphics gfx)
        {
            if (texture == null)
                return;
            int frameHeight = height;
            if (ticks++ % 5 == 0)
                frameY++;
            if (frameY == totalFrames)
                frameY = 0;
            gfx.DrawImage(texture, new RectangleF(position.X, position.Y, width, height), new RectangleF(0, frameY * frameHeight, width, height), GraphicsUnit.Pixel);
        }
    }
}
