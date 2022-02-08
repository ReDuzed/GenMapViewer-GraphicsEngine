﻿using System;
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
        private NPC npc;
        private Player player;
        private float angle;
        public override void AI()
        {
            if ((angle += 0.017f) >= Math.PI * 2f)
                angle = 0f;
            float sin = Main.ScreenWidth / 2 + 100 * (float)Math.Sin(angle);
            float cos = Main.ScreenHeight / 2 + 100 * (float)Math.Cos(angle);
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
            Main.npc[whoAmI].active = true;
            Main.npc[whoAmI].width = 16;
            Main.npc[whoAmI].height = 16;
            Main.npc[whoAmI].color = Color.Blue;
            npc = Main.npc[whoAmI];
        }

        public override void Update()
        {
            hitbox = new Rectangle((int)position.X, (int)position.Y, width, height);
        }
    }
}
