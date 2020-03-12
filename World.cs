using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenMapViewer
{
    public class SquareBrush : Main
    {
        public int X;
        public int Y;
        public new int Width;
        public new int Height;
        private bool Active = true;
        public bool door(bool door)
        {
            return false;
        }
        public bool active(bool active)
        {
            Active = active;
            return active;
        }
        public SquareBrush(int x, int y, int width, int height)
        {
            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Height = height;
        }
        protected override void PreDraw(Bitmap bmp, Graphics gfx)
        {
            if (!Active)
                return;
            gfx.DrawPolygon(Pens.White, new Point[]
            {
                new Point(X, Y),
                new Point(X + Width, Y),
                new Point(X, Y + Height)
            });
            gfx.DrawPolygon(Pens.White, new Point[]
            {
                new Point(X + Width - 1, Y),
                new Point(X + Width - 1, Y + Height),
                new Point(X - 1, Y + Height - 1)
            });
        }
    }
}
