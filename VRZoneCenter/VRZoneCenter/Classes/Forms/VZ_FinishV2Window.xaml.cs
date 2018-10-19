using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using VRZoneCenter.Classes.Utils;
using VRZoneLib.Classes.Utils;

namespace VRZoneCenter.Classes.Forms
{
    /// <summary>
    /// VZ_FinishV2Window.xaml 的交互逻辑
    /// </summary>
    public partial class VZ_FinishV2Window : Window
    {

        System.Timers.Timer timer;
        System.Timers.Timer timer2;
        int index = 30;
        int index2 = 300;
        public VZ_FinishV2Window()
        {
            InitializeComponent();

            this.Topmost = true;
            timer = new System.Timers.Timer(1000);
            timer.AutoReset = true;
            timer.Elapsed += Timer_Elapsed;
            timer.Start();

            timer2 = new System.Timers.Timer(100);
            timer2.AutoReset = true;
            timer2.Elapsed += Timer2_Elapsed;
            timer2.Start();

            getQRCodeImageAsync();
        }

        void getQRCodeImageAsync()
        {
            new Thread(new ThreadStart(() => {
                ImageSource imageSource = VZ_AppHelper.getQRCode();
                if (imageSource == null)
                {
                    return;
                }
                imageSource.Freeze();
                Dispatcher.BeginInvoke((Action)(() => {
                    img_price.Source = imageSource;
                    img_price.Stretch = Stretch.UniformToFill;
                    imageSource = null;
                }));
            })).Start();
        }

        private void Timer2_Elapsed(object sender, ElapsedEventArgs e)
        {
            Dispatcher.BeginInvoke((Action)(() =>
            {
                index2--;
                draw(360 / 300.0 * (300 - index2));
            }));
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            index--;
            Dispatcher.BeginInvoke((Action)(() =>
            {
                if ((VZ_AppHelper.GetInstance().payInfo.expire > VZ_AppHelper.GetInstance().payInfo.current_time) || index < 0)
                {
                    if (timer != null)
                    {
                        timer.Close();
                        timer = null;
                    }
                    if (timer2 != null)
                    {
                        timer2.Close();
                        timer2 = null;
                    }
                    this.Close();
                }
                tbTime.Text = "" + index;
            }));
        }

        void draw(double value)
        {
            value = value - 90;
            double r = 48;
            Path path = new Path();
            PathGeometry pathGeometry = new PathGeometry();
            double x = 50 + r * Math.Cos(value * 3.1416926 / 180.0);
            double y = 50 + r * Math.Sin(value * 3.1416926 / 180.0);
            bool isLarge = false;
            if (value > 90) isLarge = true;
            if (value == 270) isLarge = false;
            ArcSegment arc = new ArcSegment(new Point(x, y), new Size(r, r), value, isLarge, SweepDirection.Clockwise, true);
            PathFigure figure = new PathFigure();
            figure.StartPoint = new Point(50, 50 - r);
            figure.Segments.Add(arc);
            pathGeometry.Figures.Add(figure);
            path.Data = pathGeometry;
            path.Stroke = new SolidColorBrush(Color.FromArgb(255, 83, 116, 132));
            path.StrokeThickness = 3.5;
            path.SnapsToDevicePixels = false;
            canvas.SnapsToDevicePixels = false;
            canvas.Children.Add(path);
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            makePosition();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (timer != null)
            {
                timer.Close();
                timer = null;
            }
            if (timer2 != null)
            {
                timer2.Close();
                timer2 = null;
            }
        }

        void makePosition()
        {
            System.Windows.Forms.Screen s = VZ_AppProcessHelper.getSingleton().getMainScreen();

            System.Drawing.Rectangle r = s.WorkingArea;
            this.Top = r.Top;
            this.Left = r.Left;
            this.Width = r.Width;
            this.Height = r.Height;

            this.WindowStartupLocation = WindowStartupLocation.Manual;
            //this.Top = 0;
            //this.Left = System.Windows.SystemParameters.MaximizedPrimaryScreenWidth;
            this.WindowState = System.Windows.WindowState.Normal;
            this.WindowStyle = System.Windows.WindowStyle.None;
            this.WindowState = System.Windows.WindowState.Maximized;
        }
    }
}
