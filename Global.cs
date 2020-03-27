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
        public void Kill(bool explode = false)
        {
            Main.dust[whoAmI].active = false;
            if (active && explode && Main.rand.NextDouble() >= 0.50f)
            {
                Projectile.NewProjectile((int)position.X, (int)position.Y, Main.rand.Next(-2, 2), Main.rand.Next(-2, 2), -1, Color.Purple, 600);
            }
        }
        public class Waypoint
        {
            public const int
                Green = 0,
                Red = 1,
                Yellow = 2;
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
                    texture = FromFile(Name);
                    break;
            }
        }
        private Image FromFile(string Name)
        {
            return Bitmap.FromFile(NpcTexture(Name));
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
        private int proj;
        private Dust target;
        float moveSpeed = 0.15f;
        float maxSpeed = 3f;
        private Projectile[] orb = new Projectile[5];
        float stopSpeed
        {
            get { return moveSpeed; }
        }
        float jumpSpeed
        {
            get { return maxSpeed; }
        }
        public Dust pickRand()
        {
            var list = Main.dust.Where(t => t != null).ToArray();
            return list[Main.rand.Next(list.Length)];
        }
        private bool launch;
        private bool redPhase;
        private int timer, timer2;
        private int redOrbsCollected;
        private Dust[] targets = new Dust[1001];
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
                    {
                        target = pickRand();
                        ticks = 0;
                        proj = 0;
                        launch = false;
                    }
                    if (target.active)
                    {
                        if (target.type != Dust.Waypoint.Yellow && scale < 1f)
                            scale += 0.1f;

                        position.X += velocity.X;
                        position.Y += velocity.Y;
                        switch (target.type)
                        {
                            case Dust.Waypoint.Green:
                                if (!this.hitbox.Collision(target.Center.X, target.Center.Y))
                                {
                                    var speed = AngleToSpeed((float)Math.Atan2(position.Y - target.position.Y, position.X - target.position.X) + 180f * Draw.radian, moveSpeed);
                                    velocity.X += speed.X;
                                    velocity.Y += speed.Y;
                                }
                                else
                                {
                                    target.Kill();
                                }
                                break;
                            case Dust.Waypoint.Yellow:
                                if (!this.hitbox.Collision(target.Center.X, target.Center.Y))
                                {
                                    var speed = AngleToSpeed((float)Math.Atan2(position.Y - target.position.Y, position.X - target.position.X) + 180f * Draw.radian, moveSpeed);
                                    velocity.X += speed.X;
                                    velocity.Y += speed.Y;
                                }
                                else
                                {
                                    if (scale > 0.1f)
                                        scale -= 0.1f;
                                    else
                                    {
                                        target.Kill();
                                        while ((target = pickRand()).type != Dust.Waypoint.Yellow)
                                        { 
                                            position = target.position;
                                            break;
                                        }
                                    }
                                }
                                break;
                            case Dust.Waypoint.Red:
                                //  player kill red orbs before collected

                                if (velocity.X > stopSpeed)
                                    velocity.X = stopSpeed;
                                if (velocity.X < -stopSpeed)
                                    velocity.X = -stopSpeed;
                                if (velocity.Y > stopSpeed)
                                    velocity.Y = stopSpeed;
                                if (velocity.Y < -stopSpeed)
                                    velocity.Y = -stopSpeed;

                                if (!launch)
                                {
                                    redOrbsCollected++;
                                    Vector2 speed = AngleToSpeed(AngleTo(position, Main.LocalPlayer.position), 4f);
                                    orb[proj] = Projectile.NewProjectile((int)position.X - 16, (int)position.Y - 16, speed.X, speed.Y, -1, Color.White, 300);
                                    launch = true;
                                }
                                if (Distance(position, orb[proj].position).X > 300)
                                {
                                    orb[proj].Kill();
                                }
                                break;
                        }
                    }
                    if (ticks++ >= 600)
                        ticks = 1;
                    if (redOrbsCollected >= 4)
                    {
                        if (!redPhase && (scale -= 0.1f) <= 0.3f)
                        {
                            targets = Main.dust.Where(t => t != null && t.active && Distance(t.position, position).X < 300f && Distance(t.position, position).Y < 300f && Main.rand.NextDouble() >= 0.34f).ToArray();
                            redPhase = true;
                        }
                        maxSpeed = 4f;
                    }
                    if (redPhase)
                    {
                        if (timer++ > 600)
                        {
                            timer = 0;
                            redPhase = false;
                        }
                    }
                    break;
            }
        }
        public static float AngleTo(Vector2 from, Vector2 to)
        {
            return (float)Math.Atan2(to.Y - from.Y, to.X - from.X);
        }
        public static Vector2 AngleToSpeed(float angle, float amount)
        {
            float cos = (float)(amount * Math.Cos(angle));
            float sine = (float)(amount * Math.Sin(angle));
            return new Vector2(cos, sine);
        }
        protected override void PreDraw(Bitmap bmp, Graphics gfx)
        {
            gfx.DrawImage(texture, new RectangleF(position.X, position.Y, width * scale, height * scale), new RectangleF(0, 0, width, height), GraphicsUnit.Pixel);
            if (redPhase)
            {
                Point[] point = new Point[targets.Length];
                for (int i = 0; i < targets.Length; i++)
                {
                    point[i] = new Point((int)targets[i].position.X, (int)targets[i].position.Y);
                }
                if (point.Length > 0)
                {
                    point[0] = new Point((int)position.X, (int)position.Y + 20);
                    if (timer % 2 == 0)
                        timer2++;
                    if (timer2 < point.Length)
                    {
                        gfx.DrawLine(Pens.White, point[0], point[timer2]);
                        targets[timer2].Kill(true);
                    }
                    else timer2 = 0;
                }
            }
        }
        private string NpcTexture(string name)
        {
            return "Textures\\" + name + ".png";
        }
        public static Vector2 Distance(Vector2 one, Vector2 two) 
        {
            float width = one.X - two.X;
            float height = one.Y - two.Y;
            return new Vector2(Math.Abs(width), Math.Abs(height));
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
                    style = 0;
                    Name = "Orb";
                    texture = Bitmap.FromFile(ProjTexture(Name));
                    break;
            }
        }
        private string ProjTexture(string name)
        {
            return "Textures\\" + name + ".png";
        }
        public static Projectile NewProjectile(int x, int y, float speedX, float speedY, int type, Color color, int maxTime)
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
            Main.projectile[index].velocity = new Vector2(speedX, speedY);
            Main.projectile[index].type = type;
            Main.projectile[index].active = true;
            Main.projectile[index].maxLife = maxTime;
            Main.projectile[index].color = color;
            Main.projectile[index].whoAmI = index;
            Main.projectile[index].Initialize();
            return Main.projectile[index];
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
        float moveSpeed = 0.15f;
        float maxSpeed = 4f;
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

                    position.X += velocity.X;
                    position.Y += velocity.Y;
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
            if (texture == null || !active)
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
