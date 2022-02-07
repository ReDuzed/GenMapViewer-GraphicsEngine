using RUDD;
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
    public class Background : Entity
    {
        public bool light;
        public Background(int x, int y, int width, int height, Image texture)
        {
            this.width = width;
            this.height = height;
            position = new Vector2(x, y);
            hitbox = new Rectangle(x, y, width, height);
            this.texture = texture;
            active = true;
        }
        internal void Draw(Bitmap bmp, Graphics gfx)
        {
            if (!active)
                return;
            if (light)
                gfx.DrawImage(texture, new RectangleF(position.X, position.Y, width, height), new RectangleF(position.X, position.Y, width, height), GraphicsUnit.Pixel);
        }
    }
}
