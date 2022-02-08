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
using System.Security.Policy;
using System.Windows.Threading;

namespace GenMapViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public abstract partial class Main : Window
    {
        public Main()
        {
            Instance = this;
            InitializeComponent();
            LoadResources();
            MainMenu();
        }
        #region variables
        public static Main Instance;
        public static bool Logo = true;  
        private static Thread mainMenu;
        public static int ScreenWidth => 800;
        public static int ScreenHeight => 600;
        public float ScreenX, ScreenY;
        private bool init = false;
        Action method2;
        private int frameRate => 1000 / 60;
        public static float FrameRate = 1000 / 120;
        #endregion
        #region base functions
        private void MainMenu()
        {
            method2 = null;
            //  Todo draw thread separate from input thread
            mainMenu = new Thread(t =>
            {
                Main m = (Main)t;
                method2 = delegate ()
                {
                    if (!Logo)
                        return;
                    System.Threading.Thread.Sleep(frameRate);
                    using (Bitmap bmp = new Bitmap((int)m.logo.Width, (int)m.logo.Height))
                    {
                        using (Graphics graphic = Graphics.FromImage(bmp))
                        {
                            TitleScreen(bmp, graphic);
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
            mainMenu.IsBackground = true;
            mainMenu.Start(this);
        }
        private void Matrix_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void Main_KeyDown(object sender, KeyEventArgs e)
        {
            if (!e.Handled && !e.IsRepeat && e.Key.Equals(Key.Enter))
            {
                Logo = false;
                Button_Click(this, null);
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Logo = false;
            logo.IsEnabled = false;
            logo.Visibility = Visibility.Hidden;
            EventHandler draw = null;
            draw = (object o, EventArgs args) => 
            {
                using (Bitmap bmp = new Bitmap(ScreenWidth, ScreenHeight))
                {
                    using (Graphics graphic = Graphics.FromImage(bmp))
                        Draw(bmp, graphic);
                    int stride = ScreenWidth * ((PixelFormats.Bgr24.BitsPerPixel + 7) / 8);
                    var data = bmp.LockBits(new System.Drawing.Rectangle(0, 0, ScreenWidth, ScreenHeight), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                    graphic.Source = BitmapSource.Create(ScreenWidth, ScreenHeight, 96f, 96f, PixelFormats.Bgr24, null, data.Scan0, stride * ScreenHeight, stride);
                    bmp.UnlockBits(data);
                }
            };
            EventHandler logic = null;
            logic = (object o, EventArgs args) => 
            { 
                if (init)
                {
                    Initialize();
                    init = false;
                }
                Update();
            };
            new DispatcherTimer(TimeSpan.FromMilliseconds(FrameRate), DispatcherPriority.Render, draw, Dispatcher);
            new DispatcherTimer(TimeSpan.FromMilliseconds(1000 / 500), DispatcherPriority.Send, logic, Dispatcher);
        }
        #endregion
        #region overrides
        public abstract bool PreDraw(Bitmap bitmap, Graphics graphics, Camera CAMERA, bool CAMERA_FOLLOW = false);
        public abstract void Draw(Bitmap bitmap, Graphics graphics);
        public abstract void Camera(Graphics graphics, Camera CAMERA);
        public abstract void Update();
        public abstract void Initialize();
        public abstract void LoadResources();
        public abstract void TitleScreen(Bitmap bitmap, Graphics graphics);
        #endregion
    }
    public class Core : Main
    {
        public override void LoadResources()
        {
        }
        public override void Initialize()
        {
        }
        public override void TitleScreen(Bitmap bitmap, Graphics graphics)
        {
            graphics.DrawString("Demo", System.Drawing.SystemFonts.DefaultFont, System.Drawing.Brushes.Red, 10, 30);
        }
        public override bool PreDraw(Bitmap bitmap, Graphics graphics, Camera CAMERA, bool CAMERA_FOLLOW = false)
        {
            if (CAMERA_FOLLOW)
            { 
                Camera(graphics, CAMERA);
            }
            return false;
        }
        public override void Draw(Bitmap bitmap, Graphics graphics)
        {
            if (!PreDraw(bitmap, graphics, null))
                return;
            graphics.Clear(System.Drawing.Color.Black);
            graphics.FillRectangle(System.Drawing.Brushes.Black, 0, 0, ScreenWidth, ScreenHeight);
        }
        public override void Update()
        {
            if (Keyboard.IsKeyDown(Key.Escape))
                Application.Current.Shutdown();
        }
        public override void Camera(Graphics graphic, Camera CAMERA)
        {
            if (CAMERA.isMoving)
            {
                ScreenX -= CAMERA.velocity.X;
                ScreenY -= CAMERA.velocity.Y;
            }
            graphic.RenderingOrigin = new System.Drawing.Point((int)CAMERA.position.X, (int)CAMERA.position.Y);
            graphic.TranslateTransform(
                ScreenX + CAMERA.velocity.X,
                ScreenY + CAMERA.velocity.Y,
                System.Drawing.Drawing2D.MatrixOrder.Append);
        }
    }
    public class Camera
    {
        public Vector2 oldPosition;
        public Vector2 position;
        public Vector2 velocity;
        public bool isMoving;
    }
}
