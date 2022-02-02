using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Input;
using RUDD;
using System.Windows.Media.Effects;
using System.Windows;

namespace GenMapViewer
{
    public class Inventory : Main
    {
        public new int Width
        {
            get { return Main.ScreenWidth; }
        }
        public new int Height
        {
            get { return Main.ScreenHeight; }
        }
        public RectangleF Armor
        {
            get { return new RectangleF(Width / 10 - Main.ScreenX, 30 - Main.ScreenY, Width / 3, Height - 60); }
        }
        public RectangleF Equipment
        {
            get { return new RectangleF(Width / 2 + 30 - Main.ScreenX, 30 - Main.ScreenY, Width / 3, Height / 3 + 30); }
        }
        public RectangleF Items
        {
            get { return new RectangleF(Width / 2 + 30 - Main.ScreenX, Height / 2 + 30 - Main.ScreenY, Width / 3, Height / 3 + 15); }
        }
        private Image texture
        {
            get { return GUI.Texture("temp_bg"); }
        }
        public new Player player;
        public static bool open = false;
        internal static bool listUpdate;
        int flag;
        public Inventory(Player player)
        {
            this.player = player;
            player.equip = new Item[10];
            player.armor = new Item[8];
            player.item = new Item[256];
            for (int i = 0; i < 24; i++)
            {
                var n = new Item() { itemType = Item.Type.Sword_OneHand, active = true };
                n.Initialize();
                itemList.Add(n);
                Item.NewItem(Main.rand.Next(0, Main.ScreenWidth), Main.rand.Next(0, Main.ScreenHeight), 24, 24, Item.Type.Sword_OneHand, Item.Style.Weapon_OneHand, Item.Owner_World);
            }
        }
        public static IList<Item> itemList = new List<Item>();
        public static IList<Item> itemProximate = new List<Item>();
        public new void Update()
        {
            if (KeyDown(Key.O) && flag % 2 == 0)
            {
                itemProximate.Clear();
                foreach (Item i in Main.item)
                {
                    if (i != null && i.active && i.owner == Item.Owner_World && Entity.Proximity(player.Center, i.Center, 64f))
                    {
                        itemProximate.Add(i);
                    } 
                }
                open = !open;
                flag = 1;
            }
            if (KeyUp(Key.O) && flag % 2 == 1)
                flag = 0;
            if (!open)
                return;

            if (player.RightMouse())
            {
                foreach (Item i in player.item.Where(t => t != null && t.owner == player.whoAmI))
                {
                    if (player.mouseItem != null && player.mouseItem == i)
                    {
                        player.mouseItem = null;
                    }
                    if (ContainsMouse(i.hitbox))
                    {
                        switch (i.itemStyle)
                        {
                            case Item.Style.Weapon_OneHand:
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            if (player.LeftMouse())
            {
                foreach (Item i in itemProximate)
                {
                    if (ContainsMouse(i.hitbox))
                    {
                        itemList.Add(i);
                        itemProximate.Remove(i);
                        listUpdate = true;
                        return;
                    }
                }
            }
        }
        private bool ContainsMouse(Rectangle other)
        {
            return other.Contains(Main.WorldMouse.X, Main.WorldMouse.Y);
        }
        public new void PreDraw(Bitmap bmp, Graphics gfx)
        {
            if (!open)
                return;
            gfx.DrawImage(texture, new RectangleF(0 - Main.ScreenX, 0 - Main.ScreenY, Width, Height), new RectangleF(0, 0, Width, Height), GraphicsUnit.Pixel);
            gfx.DrawRectangles(Pens.Silver, new RectangleF[] { Items, Equipment, Armor });
        }
        public void Draw(Bitmap bmp, Graphics gfx)
        {
            if (!open)
                return;
            if (!listUpdate)
            {
                DrawInventory(itemList, Items, gfx);
                DrawInventory(itemProximate, Equipment, gfx);
            }
            listUpdate = false;
        }
        private void DrawInventory(IList<Item> list, RectangleF region, Graphics gfx)
        {
            const int offset = 8;
            int c = (int)region.X + offset, r = (int)region.Y + offset;
            foreach (Item i in list)
            {
                i.DrawInventory(c, r, gfx, r < region.Height + region.Y - 24);
                c += 32;
                if (c > region.Width + region.X - 24)
                {
                    r += 32;
                    c = (int)region.X + offset;
                }
                i.hitbox = new Rectangle(c, r, 24, 24);
            }
        }
        public new bool KeyDown(Key key)
        {
            return player.KeyDown(key);
        }
        public new bool KeyUp(Key key)
        {
            return player.KeyUp(key);
        }
        public bool LeftMouse()
        {
            return player.LeftMouse();
        }
        public bool RightMouse()
        {
            return player.RightMouse();
        }
    }
}
