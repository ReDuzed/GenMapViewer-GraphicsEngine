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

using RUDD;
using GenMapViewer.Biome;
using System.Security.Policy;
using System.Windows.Threading;

namespace GenMapViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Main : Window
    {
        public Main()
        {
            Instance = this;
            InitializeComponent();
            Initialize();
            LogoDisplay();
        }
        private void Initialize()
        {
            ScreenWidth = (int)graphic.Width;
            ScreenHeight = (int)graphic.Height;
            rand = new rand();
        }
        #region variables
        public static Main Instance;
        public static Tile[,] tile = new Tile[ScreenWidth / 16, ScreenHeight / 16];
        public static SquareBrush[] square = new SquareBrush[256];
        public static Background[] ground = new Background[501];
        public static Dust[] dust = new Dust[1001]; 
        public static NPC[] npc = new NPC[501];
        public static Projectile[] projectile = new Projectile[1001];
        public static GUI[] gui = new GUI[129];
        public static GUI[,] skill = new GUI[10, 4];
        public static Item[] item = new Item[501];
        public static Player[] player = new Player[256];
        private bool once = true;
        public static int ScreenWidth;
        public static int ScreenHeight;
        public static Bitmap level;
        public static bool Logo = true;  
        Action method2;
        public static int frameRate
        {
            get { return 1000 / 120; }
        }
        public static float ScreenX
        {
            get;
            private set;
        }
        public static float ScreenY
        {
            get;
            private set;
        }
        public static MouseDevice MouseDevice
        {
            get { return Mouse.PrimaryDevice; }
        }
        public static MouseDevice OldMouse;
        public Vector2 ScreenPosition
        {
            get { return new Vector2(ScreenX, ScreenY); }
        }
        public static System.Drawing.Color[] types = new System.Drawing.Color[]
        {
            System.Drawing.Color.Black,
            System.Drawing.Color.White,
            System.Drawing.Color.Red,
            System.Drawing.Color.Brown,
            System.Drawing.Color.Green
        };
        public static rand rand;
        public static Player LocalPlayer
        {
            get { return player[0]; }
        }
        float ticks = 0;
        public static Vector2 MousePosition;
        public static Vector2 WorldMouse;
        private bool begin = false;
        #endregion
        #region base functions
        private void LogoDisplay()
        {
            method2 = null;
            //  Todo draw thread separate from input thread
            Thread thread = new Thread(t =>
            {
                Main m = (Main)t;
                method2 = delegate ()
                {
                    if (!Main.Logo)
                        return;
                    System.Threading.Thread.Sleep(Main.frameRate);
                    using (Bitmap bmp = new Bitmap((int)m.logo.Width, (int)m.logo.Height))
                    {
                        using (Graphics graphic = Graphics.FromImage(bmp))
                        {
                            graphic.DrawString("Demo", System.Drawing.SystemFonts.DefaultFont, System.Drawing.Brushes.Red, 10, 30);
                        }
                        int stride = (int)m.logo.Width * ((PixelFormats.Bgr24.BitsPerPixel + 7) / 8);
                        var data = bmp.LockBits(new System.Drawing.Rectangle(0, 0, (int)m.logo.Width, (int)m.logo.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                        m.logo.Source = BitmapSource.Create((int)m.logo.Width, (int)m.logo.Height, 96f, 96f, PixelFormats.Bgr24, null, data.Scan0, stride * (int)m.logo.Height, stride);
                        bmp.UnlockBits(data);
                    }
                    m.logo.Dispatcher.BeginInvoke(method2, System.Windows.Threading.DispatcherPriority.Background);
                };
                m.logo.Dispatcher.BeginInvoke(method2, System.Windows.Threading.DispatcherPriority.Background);
            });
            thread.IsBackground = true;
            thread.Start(this);
        }
        private new bool KeyUp(Key key)
        {
            return Keyboard.IsKeyUp(key);
        }
        private new bool KeyDown(Key key)
        {
            return Keyboard.IsKeyDown(key);
        }
        private void PreDraw(Bitmap bmp, Graphics gfx)
        {
            ticks += Keyboard.IsKeyDown(Key.NumPad0) ? 0.05f : 0.017f;
            float cos = ScreenWidth - 50 + 40 * (float)Math.Cos(ticks);
            float sin = 10 + 40 * (float)Math.Sin(ticks);
            gfx.DrawLine(Pens.White, ScreenWidth - 50, 10, (int)cos, (int)sin);
            gfx.DrawRectangle(Pens.White, new System.Drawing.Rectangle(ScreenWidth - 50, 10, 40, 40));
        }
        private void Matrix_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }
        public static System.Drawing.Image Texture(string pathNoExt)
        {
            return System.Drawing.Image.FromFile(@"Textures\" + pathNoExt + ".png");
        }
        private void Main_KeyDown(object sender, KeyEventArgs e)
        {
            if (!begin && !e.Handled && !e.IsRepeat && e.Key.Equals(Key.Enter))
            {
                begin = true;
                Button_Click(this, null);
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Logo = false;
            logo.IsEnabled = false;
            logo.Visibility = Visibility.Hidden;
            bool open = false;
            EventHandler method = null;
            method = (object o, EventArgs args) => 
            {
                using (Bitmap bmp = new Bitmap(ScreenWidth, ScreenHeight))
                {
                    using (Graphics graphic = Graphics.FromImage(bmp))
                    {
                        OldMouse = MouseDevice;
                        if (once)
                        {
                            player[0] = new Player();
                            player[0].Initialize();
                            player[0].position = new Vector2(ScreenWidth / 2 - Player.plrWidth / 2, ScreenHeight / 2 - Player.plrHeight / 2);
                            square[0] = new SquareBrush(0, 0, 100, ScreenHeight);
                            square[1] = new SquareBrush(ScreenWidth - 100, 0, 100, ScreenHeight);
                            square[2] = new SquareBrush(100, ScreenHeight - 100, ScreenWidth - 200, 100);
                            ground[0] = new Background(0, 0, ScreenWidth, ScreenHeight, GUI.Texture("Alpha Tiles Scratches"));
                            int num = skill.Length + 6;
                            const byte buffer = 8;
                            for (int i = 0; i < 5; i++)
                            {
                                gui[i] = GUI.NewElement((i + 1) * 42 + buffer, 20, 36, 36, "Element " + i, i == 0 ? GUI.Texture("Necrosis") : GUI.Texture("MagicPixel"), LocalPlayer.whoAmI, i, i == 0 ? new Action(() => { NPC.NewNPC(0, 0, -1, System.Drawing.Color.White, 500); }) : null);
                            }
                            gui[5] = GUI.NewElement(buffer, 20, 36, 36, "Menu", GUI.Texture("MagicPixel"), LocalPlayer.whoAmI, GUI.SkillMenu, null);
                            for (int i = skill.GetLength(0) - 1; i >= 0; i--)
                            {
                                for (int j = skill.GetLength(1) - 1; j >= 0; j--)
                                {
                                    skill[i, j] = GUI.NewMenu((i + 1) * 42, (j + 2) * 42, 36, 36, string.Concat("Element ", i, "/", j), GUI.Texture("MagicPixel"), LocalPlayer.whoAmI, num, num, null);
                                    num--;
                                }
                            }
                            LocalPlayer.inventory = new Inventory(LocalPlayer);
                            MouseDevice.Capture(grid_main);
                            once = false;
                        }
                        else
                        {
                            Draw(bmp, graphic);
                        }
                    }
                    int stride = ScreenWidth * ((PixelFormats.Bgr24.BitsPerPixel + 7) / 8);
                    var data = bmp.LockBits(new System.Drawing.Rectangle(0, 0, ScreenWidth, ScreenHeight), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                    graphic.Source = BitmapSource.Create(ScreenWidth, ScreenHeight, 96f, 96f, PixelFormats.Bgr24, null, data.Scan0, stride * ScreenHeight, stride);
                    bmp.UnlockBits(data);
                }
                Update();
            };
            new DispatcherTimer(TimeSpan.FromMilliseconds(1000 / 120), DispatcherPriority.Normal, method, Dispatcher);
        }
        #endregion
        #region update and draw
        private void Draw(Bitmap bmp, Graphics graphic, bool CAMERA_Follow = false)
        {
            if (CAMERA_Follow)
            { 
                if (LocalPlayer.IsMoving())
                {
                    if (!LocalPlayer.colLeft && !LocalPlayer.colRight)
                        ScreenX -= player[0].velocity.X;
                    if (!LocalPlayer.colUp && !LocalPlayer.colDown)
                        ScreenY -= player[0].velocity.Y;
                }
                graphic.RenderingOrigin = new System.Drawing.Point((int)LocalPlayer.position.X, (int)LocalPlayer.position.Y);
                graphic.TranslateTransform(
                    ScreenX + player[0].velocity.X,
                    ScreenY + player[0].velocity.Y,
                    System.Drawing.Drawing2D.MatrixOrder.Append);
            }
            graphic.Clear(types[TileID.Empty]);
            graphic.FillRectangle(new SolidBrush(types[TileID.Empty]), 0, 0, ScreenWidth, ScreenHeight);
            foreach (Background g in ground.Where(t => t != null))
                g.PreDraw(bmp, graphic);
            foreach (SquareBrush sq in square.Where(t => t != null))
                sq.PreDraw(bmp, graphic);
            foreach (NPC n in npc.Where(t => t != null))
                n.PreDraw(bmp, graphic);
            foreach (Player p in player.Where(t => t != null))
                p.PreDraw(bmp, graphic);
            foreach (Projectile pr in projectile.Where(t => t != null))
                pr.PreDraw(bmp, graphic, pr.animated);
            foreach (Dust d in dust.Where(t => t != null))
                d.Draw(bmp, graphic);
            foreach (Item i in item.Where(t => t != null))
                i.Draw(bmp, graphic);
            foreach (GUI g in gui.Where(t => t != null))
                g.Draw(bmp, graphic);
        }
        private void Update()
        {
            var m = MouseDevice.GetPosition(grid_main);
            MousePosition = new Vector2((float)m.X, (float)m.Y);
            WorldMouse = new Vector2(MousePosition.X - ScreenX, MousePosition.Y - ScreenY);
            foreach (Player p in player.Where(t => t != null))
            {
                p.Update();
                p.collide = false;
                p.colUp = false;
                p.colDown = false;
                p.colRight = false;
                p.colLeft = false;
            }
            foreach (Item i in item.Where(t => t != null))
                i.Update();
            foreach (SquareBrush sq in square.Where(t => t != null))
                sq.Collision(LocalPlayer);
            foreach (NPC n in npc.Where(t => t != null))
            {
                n.Update();
                n.AI();
            }
            foreach (Projectile pr in projectile.Where(t => t != null))
            {
                pr.Update();
                pr.AI();
            }
            foreach (Dust d in dust.Where(t => t != null))
                d.Update();
            foreach (GUI g in gui.Where(t => t != null))
                g.Update();
        }
        #endregion
    }
}
