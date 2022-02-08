using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Input;
using CirclePrefect.Native;
using System.Windows.Media.Effects;
using System.Resources;
using System.Windows.Forms;

namespace GenMapViewer
{
    public abstract class Item : Entity
    {
        public abstract void Initialize();
        public abstract void Update();
        public abstract void Draw(Bitmap bmp, Graphics gfx, int frameCount);
        public void Kill()
        {
            Main.item[whoAmI].active = false;
        }
    }
}
