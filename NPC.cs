using CirclePrefect.Native;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenMapViewer
{
    public abstract class NPC : Entity
    {
        public abstract void Initialize();
        public abstract void AI();
        public abstract void Update();
        public abstract void Draw(Bitmap bmp, Graphics gfx, int frameCount);
        public void Kill()
        {
            Main.npc[whoAmI].active = false;
        }
    }
}
