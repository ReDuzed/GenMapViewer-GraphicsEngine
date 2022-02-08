using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

using CirclePrefect.Native;

namespace GenMapViewer
{
    public class Menu : Entity, IDisposable
    {
        public static Menu Select
        {
            get; private set;
        }
        internal string[] options;
        private const int textHeight = 12;
        internal static Item Current;
        internal static string Selected;
        private Player player
        {
            get { return Main.LocalPlayer; }
        }
        public static Menu NewMenu(float x, float y, string[] options)
        {
            Select?.Dispose();
            Select = new Menu();
            Select.active = true;
            Select.options = options;
            Select.Initialize();
            return Select;
        }
        private void Initialize()
        {
            X = Current.hitbox.x;
            Y = Current.hitbox.y;

            int width = 0;
            const int textWidth = 10;
            for (int i = 1; i < options.Length; i++)
            {
                width = options[i - 1].Length * textWidth;
                if (options[i - 1].Length > options[i].Length)
                    width = options[i].Length * textWidth;
                hitbox = new Rectangle(X, Y, width, (i + 1) * textHeight);
            }
        }
        public void Update()
        {
            if (Main.LocalPlayer.LeftMouse())
            {
                switch (Selected)
                {
                    case "Drop":
                        if (Inventory.itemList.Contains(Current))
                        {
                            Inventory.itemList.Remove(Current);
                            Inventory.itemProximate.Add(Current);
                            Inventory.listUpdate = true;
                            //Item.NewItem((int)player.Center.X, (int)player.Center.Y, Current.width, Current.height, Current.itemType, Current.itemStyle, Item.Owner_World);
                        }
                        goto default;
                    case "Close":
                        goto default;
                    default:
                        active = false;
                        break;
                }
                if (!hitbox.Contains(Main.WorldMouse))
                {
                    foreach (Item i in Inventory.itemList)
                    {
                        if (active = i.hitbox.Contains(Main.WorldMouse)) return;
                    }
                }
            }
        }
        public void Draw(Bitmap bmp, Graphics gfx)
        {
            if (!active) return;

            gfx.FillRectangle(System.Drawing.Brushes.Black, hitbox.GetRectangleF());
            for (int i = 0; i < options.Length; i++)
            {
                Rectangle select = new Rectangle(X, Y + i * textHeight, hitbox.Width, textHeight);
                if (select.Contains(Main.WorldMouse))
                {
                    gfx.FillRectangle(System.Drawing.Brushes.Blue, select.GetRectangleF());
                    Selected = options[i];
                    break;
                }
            }
            for (int j = 0; j < options.Length; j++)
            {
                gfx.DrawString(options[j], SystemFonts.DefaultFont, System.Drawing.Brushes.White, X, Y + j * 12);
            }
        }
        public void Dispose()
        {
        }
    }
}
