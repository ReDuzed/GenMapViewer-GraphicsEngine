using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Input;
using RUDD;
using System.Windows.Media.Effects;
using System.Resources;
using System.Windows.Forms;

namespace GenMapViewer
{
    public class Item : Entity
    {
        public Item()
        {
        }
        public enum Style : byte
        {
            Empty = 0,
            Weapon_OneHand = 1,
            Weapon_TwoHand = 2,
            Cloak = 3,
            Helm = 4,
            Greaves = 5,
            Bracers = 6,
            Torso = 7,
            Ring = 8,
            Necklace = 9,
            Consumable = 10,
            Storage = 11,
            Purse = 12,
            Coin = 13,
            Boot = 14
        }
        public const byte Owner_World = 255;
        public enum Type : byte
        {
            Fist = 0,
            Sword_OneHand = 1,
            Sword_TwoHand = 2,
            Shield = 3,
            Helm = 4,
            Torso = 5,
            Greaves = 6,
            Boots = 7,
            Cloak = 8,
            Pack_Chest = 9,
            Pack_Backpack = 10,
            Purse = 11,
            Necklace = 12,
            Ring = 13,
            Gauntlets = 14,
            Potion = 15,
            Scroll = 16,
            Polearm = 17,
            Mace_OneHand = 18,
            Mace_TwoHand = 19,
            Spear = 20,
            Dagger_Dirk = 21,
            Coin_Iron = 22,
            Coin_Copper = 23,
            Coin_Silver = 24,
            Coin_Gold = 25,
            Coin_Platinum = 26,
            Flail = 27,
            Axe_OneHand = 28,
            Axe_TwoHand = 29,
            Mallet = 30,
            Hammer = 31,
            Bracers = 32
        }
        public Player player
        {
            get { return Main.player[owner]; }
        }
        public bool MouseItem
        {
            get { return owner != Owner_World && player.mouseItem != null && player.mouseItem == this; }
        }
        public bool visible;
        public bool menu;
        public string[] Description
        {
            get; private set;
        }

        public static Item NewItem(int x, int y, int width, int height, Type type, Style style, int owner)
        {
            int num = 500;
            for (int i = 0; i < Main.item.Length; i++)
            {
                if (Main.item[i] == null || !Main.item[i].active)
                {
                    num = i;
                    break;
                }
                if (i == num)
                {
                    break;
                }
            }
            Main.item[num] = new Item();
            Main.item[num].active = true;
            Main.item[num].X = x;
            Main.item[num].Y = y;
            Main.item[num].width = width;
            Main.item[num].height = height;
            Main.item[num].itemType = type;
            Main.item[num].itemStyle = style;
            Main.item[num].owner = owner;
            Main.item[num].whoAmI = num;
            Main.item[num].Initialize();
            return Main.item[num];
        }
        internal void Initialize()
        {
            switch (itemType)
            {
                case Type.Sword_OneHand:
                    Name = "1h Sword";
                    damage = 10;
                    texture = Main.Texture("temp");
                    Description = new string[]
                    {
                        "Name: " + Name,
                        "Damage: " + damage
                    };
                    break;
                default:
                    break;
            }
        }
        public void Update()
        {
            if (!active)
                return;
            
            if (owner == Owner_World)
                hitbox = new Rectangle(X, Y, width, height);
            
            //  In World
            PlayerAcquire();

            //  Inventory
            if ((Inventory.itemList.Contains(this) || Inventory.itemProximate.Contains(this)) && hitbox.Contains(Main.WorldMouse) && Main.LocalPlayer.LeftMouse())
            {
                Menu.Current = this;
                Menu.NewMenu(X + width, Y, new string[] { "Equip", "Drop", "Close" });
            }
            if (visible)
            {
                var inv = Main.LocalPlayer.inventory;
                if (Inventory.itemList.Contains(this))
                {
                    if (hitbox.Contains(Main.WorldMouse))
                    {
                        if (inv.LeftMouse())
                        {
                            foreach (var n in Inventory.itemList)
                                n.menu = false;
                            menu = true;
                        }
                        if (Dialog.Instance.Selected != this)
                        {
                            Dialog.NewDialog(X, Y, Description, this);
                        }
                    }
                }
            }
        }
        public void Draw(Bitmap bmp, Graphics gfx)
        {
            if (!active)
                return;
            gfx.DrawImage(texture, new Point(X, Y));
        }
        public void DrawInventory(int x, int y, Graphics gfx, bool onScreen = true)
        {
            if (!active)
                return;
            hitbox = new Rectangle(x, y, width, height);
            if (MouseItem)
            {
                x = (int)Main.MousePosition.X - 160;
                y = (int)Main.MousePosition.Y;
            }
            if (visible = onScreen)
            {
                gfx.DrawImage(texture, new Point(x, y));
            }
        }

        private void PlayerAcquire()
        {
            foreach (Player player in Main.player.Where(t => t != null && t.active))
            {
                if (Proximity(player.Center, 48f) && hitbox.Contains(Main.WorldMouse.X, Main.WorldMouse.Y) && player.LeftMouse())
                {
                    Item.ViewItem(this, player);
                    Inventory.open = true;
                }
            }
        }
        public static Item GetItem(Item item, Player player)
        {
            int num = player.item.Length - 1;
            for (int i = 0; i < player.item.Length; i++)
            {
                if (player.item[i] == null || !player.item[i].active)
                {
                    num = i;
                    break;
                }
                if (i == num)
                {
                    return item;
                }
            }
            player.item[num] = (Item)item.MemberwiseClone();
            player.item[num].owner = player.whoAmI;
            player.item[num].active = true;
            item.active = false;
            return player.item[num];
        }
        public static Item ViewItem(Item item, Player player)
        {
            int num = player.equip.Length - 1;
            for (int i = 0; i < player.equip.Length; i++)
            {
                if (player.equip[i] == null || !player.equip[i].active)
                {
                    num = i;
                    break;
                }
                if (i == num)
                {
                    return item;
                }
            }
            player.equip[num] = (Item)item.MemberwiseClone();
            player.equip[num].owner = Owner_World;
            player.equip[num].active = true;
            item.active = false;
            item.Kill();
            return player.equip[num];
        }
        public void Kill()
        {
            Main.item[whoAmI].active = false;
        }
    }
}
