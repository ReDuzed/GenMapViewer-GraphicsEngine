using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RUDD;

namespace GenMapViewer
{
    public class Dialog : SimpleEntity, IDisposable
    {
        public static Dialog Instance;
        internal Item Selected
        {
            get; private set;
        }
        internal string[] contents;
        private const int textHeight = 12;
        public static Dialog NewDialog(int x, int y, string[] contents, Item item)
        {
            Instance?.Dispose();
            Instance = new Dialog()
            {
                contents = contents
            };
            Instance.Selected = item;
            Instance.Initialize();
            return Instance;
        }
        private void Initialize()
        {
            X = Selected.hitbox.x;
            Y = Selected.hitbox.y;

            int width = 0;
            const int textWidth = 10;
            for (int i = 1; i < contents.Length; i++)
            {
                width = contents[i - 1].Length * textWidth;
                if (contents[i - 1].Length > contents[i].Length)
                    width = contents[i].Length * textWidth;
                hitbox = new Rectangle(X, Y, width, (i + 1) * textHeight);
            }
        }

        public void Update()
        {
            foreach (Item i in Inventory.itemList)
            {
                if (active = (i.hitbox.Contains(Main.WorldMouse) && Menu.Select?.active == false))
                {
                    return;
                }
            }
            foreach (Item i in Inventory.itemProximate)
            {
                if (active = (i.hitbox.Contains(Main.WorldMouse) && Menu.Select?.active == false))
                {
                    return;
                }
            }
        }
        public void Draw(Bitmap bmp, Graphics gfx)
        {
            if (!active) return;

            gfx.FillRectangle(System.Drawing.Brushes.Black, hitbox.GetRectangleF());
            for (int j = 0; j < contents.Length; j++)
            {
                gfx.DrawString(contents[j], SystemFonts.DefaultFont, System.Drawing.Brushes.White, X, Y + j * 12);
            }
        }

        public void Dispose() 
        { 
        }
    }
}
