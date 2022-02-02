using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RUDD;

namespace GenMapViewer
{
    public class Projectile : Entity
    {
        public const int Necrosis = -1;
        public const byte
            SwordSwipe = 0,
            SwordSwing = 1;
        public bool animated = false;
        private float timer;
        public float startAngle = 0f;
        private float EndAngle(float start)
        {
            return Math.Abs(start) + Draw.radian * 90f;
        }
        private float endAngle;
        private float currentAngle;
        public bool IsAttacking
        {
            get { return Main.LocalPlayer.isAttacking; }
            set { Main.LocalPlayer.isAttacking = value; }
        }
        private new void Initialize()
        {
            Vector2 ang = Vector2.Zero;
            switch (type)
            {
                case Necrosis:
                    width = 38;
                    height = 44;
                    style = 0;
                    Name = "Orb";
                    texture = Bitmap.FromFile(ProjTexture(Name));
                    break;
                case SwordSwipe:
                    if (IsAttacking)
                    {
                        Kill();
                        return;  
                    }
                    width = 32;
                    height = 32;
                    style = SwordSwipe;
                    Name = "Sword_Swipe";
                    texture = GUI.Texture("MagicPixel");
                    IsAttacking = true;
                    break;
                case SwordSwing:
                    if (IsAttacking)
                    {
                        Kill();
                        return;
                    }
                    width = 24;
                    height = 24;
                    style = SwordSwing;
                    Name = "Sword_Swing";
                    texture = GUI.Texture("MagicPixel");
                    startAngle = NPC.AngleTo(Main.LocalPlayer.Center, Main.WorldMouse) - Draw.radian * 45f;
                    currentAngle = startAngle;
                    endAngle = EndAngle(startAngle);
                    ang = NPC.AngleToSpeed(startAngle, 32);
                    position = new Vector2(Main.LocalPlayer.Center.X + ang.X, Main.LocalPlayer.Center.Y + ang.Y);
                    IsAttacking = true;
                    break;
                default:
                    break;
            }
        }
        private string ProjTexture(string name)
        {
            return "Textures\\" + name + ".png";
        }
        public static Projectile NewProjectile(int x, int y, float speedX, float speedY, int type, System.Drawing.Color color, int maxTime, bool animated = false)
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
            Main.projectile[index].animated = animated;
            Main.projectile[index].Initialize();
            return Main.projectile[index];
        }
        public static Projectile NewProjectile(int x, int y, int type, System.Drawing.Color color, int maxTime, bool animated = false)
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
            Main.projectile[index].animated = animated;
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
            if (!active)
                return;
            Vector2 plr = Vector2.Zero;
            Vector2 ang = Vector2.Zero;
            switch (type)
            {
                case Necrosis:
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
                case SwordSwipe:
                    plr = Main.LocalPlayer.Center;
                    ang = NPC.AngleToSpeed(timer, 64f);
                    position = new Vector2(plr.X + ang.X, plr.Y + ang.Y);
                    hitbox = new Rectangle((int)position.X, (int)position.Y, width, height);
                    if (!LocalPlayer.IsMoving())
                        return;
                    timer += 0.25f;
                    break;
                case SwordSwing:
                    plr = Main.LocalPlayer.Center;
                    ang = NPC.AngleToSpeed(currentAngle, 32);
                    position = new Vector2(plr.X + ang.X, plr.Y + ang.Y);
                    hitbox = new Rectangle((int)position.X, (int)position.Y, 32, 32);
                    break;
            }
        }
        public static Vector2 AngleToSpeed(float angle, float amount)
        {
            float cos = (float)(amount * Math.Cos(angle));
            float sine = (float)(amount * Math.Sin(angle));
            return new Vector2(cos, sine);
        }
        public void Update()
        {
            switch (type)
            {
                case Necrosis:
                    goto default;
                case SwordSwipe:
                    if (!LocalPlayer.IsMoving())
                        break;
                    if (timeLeft++ > maxLife)
                    {
                        IsAttacking = false;
                        Kill();
                    }
                    break;
                case SwordSwing:
                    if (!LocalPlayer.IsMoving())
                        break;
                    currentAngle += 0.2f;
                    if (timeLeft++ > maxLife)
                    {
                        Kill();
                        IsAttacking = false;
                    }
                    break;
                default:
                    if (timeLeft++ > maxLife)
                        Kill();
                    break;
            }
        }
        public void Kill()
        {
            Main.projectile[whoAmI].active = false;
        }
        int ticks;
        int frameY;
        int totalFrames = 7;
        public void PreDraw(Bitmap bmp, Graphics gfx, bool animated = false)
        {
            if (texture == null || !active)
                return;
            if (animated)
            {
                int frameHeight = height;
                if (ticks++ % 5 == 0)
                    frameY++;
                if (frameY == totalFrames)
                    frameY = 0;
                gfx.DrawImage(texture, new RectangleF(position.X, position.Y, width, height), new RectangleF(0, frameY * frameHeight, width, height), GraphicsUnit.Pixel);
            }
            else gfx.DrawImage(texture, new RectangleF(position.X, position.Y, width, height));
            //DEBUG
            gfx.DrawRectangle(System.Drawing.Pens.White, new System.Drawing.Rectangle(hitbox.x, hitbox.y, hitbox.Width, hitbox.Height));
        }
    }
}
