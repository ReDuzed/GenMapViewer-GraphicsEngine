using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
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
using Terraria.Utilities;
using GenMapViewer.Biome;

namespace GenMapViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Main : Window
    {
        public static Main Instance;
        public static UnifiedRandom rand;
        public static Tile[,] tile = new Tile[ScreenWidth / 16, ScreenHeight / 16];
        public static SquareBrush[] square = new SquareBrush[256];
        public static Dust[] dust = new Dust[1001]; 
        public static NPC[] npc = new NPC[501];
        public static Projectile[] projectile = new Projectile[1001];
        public static int frameRate
        {
            get { return 1000 / 167; }
        }
        public Main()
        {
            Instance = this;
            rand = new UnifiedRandom();
            InitializeComponent();
        }
        /*
        public static int rightWorld
        {
            get { return width; }
        }
        public static int bottomWorld
        {
            get { return height; }
        }
        public static int maxTilesX
        {
            get { return width; }
        }
        public static int maxTilesY
        {
            get { return height; }
        }*/
        private bool once = true;
        public static int ScreenWidth;
        public static int ScreenHeight;
        public static Bitmap level;
        public static Player[] player = new Player[256];
        private float ScreenX, ScreenY;
        public static System.Drawing.Color[] types = new System.Drawing.Color[]
        {
            System.Drawing.Color.Black,
            System.Drawing.Color.White,
            System.Drawing.Color.Red,
            System.Drawing.Color.Brown,
            System.Drawing.Color.Green
        };
        public static Player LocalPlayer
        {
            get { return player[0]; }
        }

        protected virtual void Initialize()
        {
            ScreenWidth = (int)graphic.Width;
            ScreenHeight = (int)graphic.Height;
            //square[0] = new SquareBrush(0, height / 2, width, height / 2);
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Initialize();
            Action method = null;
            method = delegate() 
            {
                
                System.Threading.Thread.Sleep(frameRate);
                using (Bitmap bmp = new Bitmap(ScreenWidth, ScreenHeight))
                {
                    using (Graphics graphic = Graphics.FromImage(bmp))
                    {
                        if (once)
                        {
                            
                            player[0] = new Player();
                            player[0].position = new Vector2(ScreenWidth / 2, ScreenHeight / 2);
                            //level = new LevelGen().Generate(bmp);
                            once = false;
                        }
                        else
                        {
                            if (LocalPlayer.IsMoving())
                            {
                                ScreenX -= player[0].velocity.X;
                                ScreenY -= player[0].velocity.Y;
                            }
                            graphic.RenderingOrigin = new System.Drawing.Point((int)LocalPlayer.position.X, (int)LocalPlayer.position.Y);
                            graphic.TranslateTransform(
                                ScreenX + player[0].velocity.X,
                                ScreenY + player[0].velocity.Y,
                                System.Drawing.Drawing2D.MatrixOrder.Append);
                            graphic.Clear(types[TileID.Empty]);
                            graphic.FillRectangle(new SolidBrush(types[TileID.Empty]), 0, 0, ScreenWidth, ScreenHeight);
                            //graphic.DrawImage(Bitmap.FromFile("output.png"), 0, 0);
                            PreDraw(bmp, graphic);
                            foreach (SquareBrush sq in square.Where(t => t != null))
                                sq.PreDraw(bmp, graphic);
                            foreach (NPC n in npc.Where(t => t != null))
                                n.PreDraw(bmp, graphic);
                            foreach (Player p in player.Where(t => t != null))
                                p.PreDraw(bmp, graphic);
                            foreach (Projectile pr in projectile.Where(t => t != null))
                                pr.PreDraw(bmp, graphic);
                            foreach (Dust d in dust.Where(t => t != null))
                                d.Draw(bmp, graphic);
                        }
                    }
                    int stride = ScreenWidth * ((PixelFormats.Bgr24.BitsPerPixel + 7) / 8);
                    var data = bmp.LockBits(new System.Drawing.Rectangle(0, 0, ScreenWidth, ScreenHeight), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                    graphic.Source = BitmapSource.Create(ScreenWidth, ScreenHeight, 96f, 96f, PixelFormats.Bgr24, null, data.Scan0, stride * ScreenHeight, stride);
                    bmp.UnlockBits(data);
                }
                foreach (Player p in player.Where(t => t != null))
                    p.Update();
                foreach (NPC n in npc.Where(t => t != null))
                {
                    n.Update();
                    n.AI();
                }
                foreach (Projectile pr in projectile.Where(t => t != null))
                    pr.Update();
                foreach (Dust d in dust.Where(t => t != null))
                    d.Update();
                PostUpdate();
                
                graphic.Dispatcher.BeginInvoke(method, System.Windows.Threading.DispatcherPriority.Background);
            };
            graphic.Dispatcher.BeginInvoke(method, System.Windows.Threading.DispatcherPriority.Background);
        }

        private new bool KeyUp(Key key)
        {
            return Keyboard.IsKeyUp(key);
        }
        private new bool KeyDown(Key key)
        {
            return Keyboard.IsKeyDown(key);
        }
        float ticks = 0;
        protected virtual void PreDraw(Bitmap bmp, Graphics gfx)
        {
            ticks += Keyboard.IsKeyDown(Key.NumPad0) ? 0.05f : 0.017f;
            float cos = ScreenWidth / 2 + ScreenWidth / 2 * (float)Math.Cos(ticks);
            float sin = ScreenHeight / 2 + ScreenHeight / 2 * (float)Math.Sin(ticks);
            gfx.DrawLine(Pens.White, ScreenWidth / 2, ScreenHeight / 2, (int)cos, (int)sin);
        }
        protected virtual void Update()
        {
            
        }
        protected void PostUpdate()
        {
            if (KeyDown(Key.NumPad0) && npc[0] == null)
            {
                NPC.NewNPC(0, 0, -1, System.Drawing.Color.White, 500);
                Projectile.NewProjectile(300, 0, -1, System.Drawing.Color.White, 300);
            }   
            if (rand.NextDouble() > 0.90f)
            {
                Dust.NewDust(rand.Next(0, ScreenWidth), rand.Next(0, ScreenHeight), 16, 16, rand.Next(3), System.Drawing.Color.Green, 150);
            }
        }
    }
}
