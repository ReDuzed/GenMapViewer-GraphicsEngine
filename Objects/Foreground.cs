using CirclePrefect.Native;
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
    public class Foreground : Entity
    {
        public Foreground()
        { 
        }
        private float moveSpeed;
        private float moveMultiplier => 2.5f;
        public void Initialize()
        {
            for (int i = 0; i < Main.fg.Length; i++)
            {
                if (Main.fg[i] == null || !Main.fg[i].active)
                {
                    whoAmI = i;
                    break;
                }
            }
            Main.fg[whoAmI] = this;
            active = true;
            if (Main.rand.NextBool())
            { 
                width = 48;
                height = 48;
            }
            else
            {
                width = 2;
                height = 18;
            }
            color = Main.types[Main.rand.Next(1, 3)];
            moveSpeed = Main.rand.NextFloat() * moveMultiplier;
            position = new Vector2(Main.rand.Next(0, Main.ScreenWidth - width), -height);
        }
        public void Update()
        {
            if (!init)
            {
                Initialize();
                init = true;
            }
            position.Y += moveSpeed;
            hitbox = new Rectangle((int)position.X, (int)position.Y, width, height);
            if (position.Y > Main.ScreenHeight)
                Kill();
        }
        public void Draw(Bitmap bmp, Graphics gfx)
        {
            if (!active)
                return;
            var pen = new Pen(new SolidBrush(color));
            pen.Width = 2;
            gfx.DrawRectangle(pen, hitbox.GetSDRectangle());
        }
        public void Kill()
        {
            Main.fg[whoAmI].active = false;
            Main.fg[whoAmI] = null;
        }
    }
}
