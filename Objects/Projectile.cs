using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenMapViewer
{
    public abstract class Projectile : Entity
    {
        public abstract void Initialize();
        public abstract void AI();
        public abstract void Update();
        public abstract void Draw(Bitmap bmp, Graphics gfx, int frameCount);
        public void Kill()
        {
            Main.projectile[whoAmI].active = false;
        }
    }
    public class O : Projectile
    {
        public override void Initialize()
        {
            Main.projectile[0] = new O();
        }
        public override void AI()
        {
        }
        public override void Update()
        {
        }
        public override void Draw(Bitmap bmp, Graphics gfx, int frameCount)
        {
            gfx.DrawRectangle(Pens.White, new System.Drawing.Rectangle(0, 0, 640, 480));
        }
    }
}
