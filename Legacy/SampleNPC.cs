using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CirclePrefect.Native;

namespace GenMapViewer
{
    public class SampleNPC : NPC
    {
        public SampleNPC()
        {
            Initialize();
        }
        private NPC npc;
        private float angle;
        public override void AI()
        {
            if ((angle += 0.017f) >= Math.PI * 2f)
                angle = 0f;
            float sin = Main.LocalPlayer.Center.X + 100 * (float)Math.Sin(angle);
            float cos = Main.LocalPlayer.Center.Y + 100 * (float)Math.Cos(angle);
            position = new Vector2(sin, cos);
        }

        public override void Draw(Bitmap bmp, Graphics gfx, int frameCount)
        {
            gfx.FillRectangle(Brushes.Blue, hitbox.GetRectangleF());
        }

        public override void Initialize()
        {
            for (int i = 0; i < Main.npc.Length; i++)
            {
                if (Main.npc[i] != null && !Main.npc[i].active)
                {
                    whoAmI = i;
                    break;
                }
            }
            Main.npc[whoAmI] = this;
            Main.npc[whoAmI].active = true;
            Main.npc[whoAmI].width = 16;
            Main.npc[whoAmI].height = 16;
            Main.npc[whoAmI].color = Color.Blue;
            npc = Main.npc[whoAmI];
        }

        public override void Update()
        {
            hitbox = new Rectangle((int)position.X, (int)position.Y, width, height);

            for (int i = 0; i < Main.fg.Length; i++)
            {
                if (Main.fg[i] != null && Main.fg[i].active)
                {
                    if (hitbox.Intersect(Main.fg[i].hitbox))
                    {
                        Main.fg[i].Kill();
                        break;
                    }
                }
            }
        }
    }
}
