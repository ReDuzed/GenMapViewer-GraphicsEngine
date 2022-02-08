using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing.Text;
using System.Windows.Forms.VisualStyles;

namespace GenMapViewer
{
    public abstract class Dust : Entity
    {
        public abstract void Initialize();
        public abstract void Update();
        public abstract void Draw(Bitmap bmp, Graphics gfx, int frameCount);
        public void Kill()
        {
            Main.dust[whoAmI].active = false;
            Main.dust[whoAmI] = null;
        }
    }
}
