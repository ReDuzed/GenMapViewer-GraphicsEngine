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
    public partial class Main
    {
        ImageSource imgSource;
        Dispatcher dispatcher;
        public Main(ImageSource image, Dispatcher dispatcher)
        {
            this.imgSource = image;
            this.dispatcher = dispatcher;
        }
        public void Start()
        {
            Core.Start(imgSource, dispatcher);
        }
    }
    class Core : Function
    {
        internal static void Start(ImageSource imgSrc, Dispatcher dispatcher)
        {
            Instance = new Core();
            Instance.LoadResources();
            Instance.MainMenu();
        }
        #region variables
        public static Core Instance;
        public static bool Logo = true;  
        public static int ScreenWidth => 800;
        public static int ScreenHeight => 600;
        public float ScreenX, ScreenY;
        static int frameRate => 1000 / 60;
        static Thread mainMenu;
        static bool init = false;
        static Action method2;
        static ImageSource imgSrc;
        static Dispatcher dispatcher;
        public Camera camera1 = new Camera();

        public const float defaultFrameRate = 1000 / 120;
        public static float FrameRate = defaultFrameRate;
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
                    using (Bitmap bmp = new Bitmap((int)imgSrc.Width, (int)imgSrc.Height))
                    {
                        using (Graphics graphic = Graphics.FromImage(bmp))
                        {
                            if (Keyboard.IsKeyDown(Key.Enter))
                            {
                                Logo = false;
                                PrimaryRoot();
                                return;
                            }
                            TitleScreen(bmp, graphic);
                        }
                        int stride = (int)imgSrc.Width * ((PixelFormats.Bgr24.BitsPerPixel + 7) / 8);
                        var data = bmp.LockBits(new System.Drawing.Rectangle(0, 0, (int)imgSrc.Width, (int)imgSrc.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                        imgSrc = BitmapSource.Create((int)imgSrc.Width, (int)imgSrc.Height, 96f, 96f, PixelFormats.Bgr24, null, data.Scan0, stride * (int)imgSrc.Height, stride);
                        bmp.UnlockBits(data);
                    }
                    dispatcher.BeginInvoke(method2, System.Windows.Threading.DispatcherPriority.Background);
                };
                dispatcher.BeginInvoke(method2, System.Windows.Threading.DispatcherPriority.Background);
            });
            mainMenu.IsBackground = true;
            mainMenu.Start(this);
        }
        private void PrimaryRoot()
        {
            Logo = false;
            EventHandler draw = null;
            draw = (object o, EventArgs args) => 
            {
                using (Bitmap bmp = new Bitmap(ScreenWidth, ScreenHeight))
                {
                    using (Graphics graphic = Graphics.FromImage(bmp))
                    {
                        if (PreDraw(bmp, graphic, camera1))
                            Draw(bmp, graphic);
                    }
                    int stride = ScreenWidth * ((PixelFormats.Bgr24.BitsPerPixel + 7) / 8);
                    var data = bmp.LockBits(new System.Drawing.Rectangle(0, 0, ScreenWidth, ScreenHeight), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                    imgSrc = BitmapSource.Create(ScreenWidth, ScreenHeight, 96f, 96f, PixelFormats.Bgr24, null, data.Scan0, stride * ScreenHeight, stride);
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
            new DispatcherTimer(TimeSpan.FromMilliseconds(FrameRate), DispatcherPriority.Render, draw, dispatcher);
            new DispatcherTimer(TimeSpan.FromMilliseconds(1000 / 500), DispatcherPriority.Send, logic, dispatcher);
        }
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
            return true;
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
            if (CAMERA == null)
                return;
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
        #endregion
    }
    abstract class Function
    {
        public abstract bool PreDraw(Bitmap bitmap, Graphics graphics, Camera CAMERA, bool CAMERA_FOLLOW = false);
        public abstract void Draw(Bitmap bitmap, Graphics graphics);
        public abstract void Camera(Graphics graphics, Camera CAMERA);
        public abstract void Update();
        public abstract void Initialize();
        public abstract void LoadResources();
        public abstract void TitleScreen(Bitmap bitmap, Graphics graphics);
    }
    public class Camera
    {
        public Vector2 oldPosition;
        public Vector2 position;
        public Vector2 velocity;
        public bool isMoving => velocity != Vector2.Zero;
    }
}
