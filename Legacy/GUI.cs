using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CirclePrefect.Native;

namespace GenMapViewer
{
    public class GUI : Button
    {
        public GUI()
        {
            Content = Main.Instance.grid_main;
        }
        public int x, y;
        public int width
        {
            get { return (int)Width; }
            set { Width = value; }
        }
        public int height
        {
            get { return (int)Height; }
            set { Height = value; }
        }
        public Vector2 position
        {
            get { return new Vector2((float)Margin.Left, (float)Margin.Top); }
            set { Margin = new Thickness(value.X, value.Y, 0, 0); }
        }
        public Rectangle hitbox;
        public System.Drawing.Image texture;
        public bool active;
        public int whoAmI;
        public int owner;
        public string text;
        public Action onClick;
        private int flag;
        public bool selected;
        public bool unlocked = false;
        public int type;
        private Player player
        {
            get { return Main.player[owner]; }
        }
        public static GUI NewElement(int x, int y, int width, int height, string text, System.Drawing.Image texture, int owner, int type, Action onClick)
        {
            int num = 128;
            for (int i = 0; i < num; i++)
            {
                if (Main.gui[i] == null || !Main.gui[i].active)
                {
                    num = i;
                    break;
                }
                if (i == num)
                {
                    return Main.gui[i];
                }
            }
            Main.gui[num] = new GUI();
            Main.gui[num].whoAmI = num;
            Main.gui[num].owner = owner;
            Main.gui[num].active = true;
            Main.gui[num].text = text;
            Main.gui[num].x = x;
            Main.gui[num].y = y;
            Main.gui[num].type = type;
            Main.gui[num].width = width;
            Main.gui[num].height = height;
            Main.gui[num].Name = text.Replace(' ', '_').Replace('/', '_');
            Main.gui[num].texture = texture;
            Main.gui[num].onClick = onClick;
            Main.gui[num].Initialize();
            return Main.gui[num];
        }
        public static GUI NewMenu(int x, int y, int width, int height, string text, System.Drawing.Image texture, int owner, int whoAmI, int type, Action onClick)
        {
            Main.gui[whoAmI] = new GUI();
            Main.gui[whoAmI].whoAmI = whoAmI;
            Main.gui[whoAmI].owner = owner;
            Main.gui[whoAmI].text = text;
            Main.gui[whoAmI].x = x;
            Main.gui[whoAmI].y = y;
            Main.gui[whoAmI].width = width;
            Main.gui[whoAmI].height = height;
            Main.gui[whoAmI].Name = text.Replace(' ', '_').Replace('/', '_');
            Main.gui[whoAmI].texture = texture;
            Main.gui[whoAmI].onClick = onClick;
            Main.gui[whoAmI].type = type;
            Main.gui[whoAmI].InitMenu();
            return Main.gui[whoAmI];
        }
        public void InitMenu()
        {
            HorizontalAlignment = HorizontalAlignment.Left;
            VerticalAlignment = VerticalAlignment.Top;
            IsEnabled = true;
            Focusable = true;
            switch (type)
            {
                case Light:
                    text = "Light";
                    Name = text;
                    onClick = new Action(() => 
                    {
                        var bg = Main.ground.Where(t => t != null && t.hitbox.Contains(Main.LocalPlayer.Center.X, Main.LocalPlayer.Center.Y));
                        if (bg.Count() > 0)
                        {
                            bg.First().light = true;
                        }
                    });
                    break;
                default:
                    break;
            }
        }

        public class ID
        {
            public const string
                DEBUG = "DEBUG",
                SwordSwipe = "Swipe",
                SwordSwing = "Swing",
                Light = "Light";
        }
        public const byte
            DEBUG = 0,
            SwordSwipe = 1,
            SwordSwing = 2,
            SkillMenu = 5,
            Light = 7;

        public void Initialize()
        {
            HorizontalAlignment = HorizontalAlignment.Left;
            VerticalAlignment = VerticalAlignment.Top;
            IsEnabled = true;
            Focusable = true;
            switch (type)
            {
                case DEBUG:
                    text = "DEBUG";
                    unlocked = true;
                    break;
                case SwordSwipe:
                    text = "Swipe";
                    Name = text;
                    unlocked = true;
                    break;
                case SwordSwing:
                    text = "Swing";
                    Name = text;
                    unlocked = true;
                    break;
                case SkillMenu:
                    unlocked = true;
                    break;
                default:
                    break;
            }
        }

        public void Update()
        {
            if (!active)
                return;
            position = new Vector2(x - Main.ScreenX, y - Main.ScreenY);
            hitbox = new Rectangle(x, y, width, height);
            
            if (!unlocked)
                return;
            
            if (Main.MouseDevice.LeftButton == MouseButtonState.Pressed && flag % 2 == 0 && whoAmI < SkillMenu)
            {
                flag++;
                foreach (GUI menu in Main.skill)
                {
                    if (menu.active && menu.unlocked)
                    {
                        if (menu.hitbox.Contains(Main.MousePosition.X, Main.MousePosition.Y) && selected)
                        {
                            MenuReplace(menu, this, menu.onClick, menu.Name, menu.texture, menu.text, menu.whoAmI);
                            return;
                        }
                    }
                }
                if (hitbox.Contains(Main.MousePosition.X, Main.MousePosition.Y) && Main.gui.ToList().Contains(this))
                {
                    foreach (GUI g in Main.gui.Where(t => t != null))
                        g.selected = false;
                    selected = true;
                }
                if (selected)
                    onClick?.Invoke();
            }
            if (Main.MouseDevice.LeftButton == MouseButtonState.Released && flag % 2 == 1)
                flag = 0;
        }
        private void MenuReplace(GUI menu, GUI g, Action click, string name, System.Drawing.Image tex, string text, int type)
        {
            menu.Name = g.Name;
            menu.onClick = g.onClick;
            menu.texture = g.texture;
            menu.text = g.text;
            menu.type = g.type;
            g.Name = name;
            g.onClick = click;
            g.texture = tex;
            g.text = text;
            g.type = type;
        }
        public void Draw(Bitmap bmp, Graphics gfx)
        {
            if (!active)
                return;

            gfx.DrawImage(texture, new System.Drawing.Rectangle((int)position.X, (int)position.Y, width, height));
            //DEBUG
            gfx.DrawRectangle(selected && unlocked ? System.Drawing.Pens.Gold : Pens.Gray, new System.Drawing.Rectangle((int)position.X, (int)position.Y, hitbox.Width, hitbox.Height));
            
            if (hitbox.Contains(Main.MousePosition.X, Main.MousePosition.Y))
                gfx.DrawString(text, System.Drawing.SystemFonts.DefaultFont, System.Drawing.Brushes.White, position.X, position.Y + height);
        }
        public static System.Drawing.Image Texture(string pathNoExt)
        {
            return System.Drawing.Image.FromFile(@"Textures\" + pathNoExt + ".png");
        }
        public static GUI GetElement(string name)
        {
            var gui = Main.gui.Where(t => t != null && t.text.Contains(name));
            if (gui.Count() > 0)
                return gui.First();
            else return null;
        }
    }
}
