using RUDD;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenMapViewer
{
    public class NPC : Entity
    {
        public float knockBack = 1f;
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
                    knockBack = 3f;
                    break;
            }
        }
        private Image FromFile(string Name)
        {
            return Bitmap.FromFile(NpcTexture(Name));
        }
        public void Update()
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
                    if (!Main.LocalPlayer.Movement())
                        return;
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
                                if (!this.hitbox.Contains(target.Center.X, target.Center.Y, width, height))
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
                                if (!this.hitbox.Contains(target.Center.X, target.Center.Y, width, height))
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
                                    orb[proj] = Projectile.NewProjectile((int)position.X - 16, (int)position.Y - 16, speed.X, speed.Y, -1, Color.White, 300, true);
                                    launch = true;
                                }
                                if (Distance(position, orb[proj].position) > 300)
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
                            targets = Main.dust.Where(t => t != null && t.active && Distance(t.position, position) < 300f && Main.rand.NextFloat() >= 0.34f).ToArray();
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
        internal void PreDraw(Bitmap bmp, Graphics gfx)
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
    }
}
