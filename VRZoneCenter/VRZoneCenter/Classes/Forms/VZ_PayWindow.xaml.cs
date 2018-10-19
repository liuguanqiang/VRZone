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
    /// VZ_PayWindow.xaml 的交互逻辑
    /// </summary>
    public partial class VZ_PayWindow : Window
    {
        System.Timers.Timer timer;
        int index = 15;
        public VZ_PayWindow()
        {
            InitializeComponent();
            this.Activate();
            timer = new System.Timers.Timer(1000);
            timer.AutoReset = true;
            timer.Elapsed += Timer_Elapsed;
            timer.Start();

            getQRCodeImageAsync();
        }
        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            index--;
            Dispatcher.BeginInvoke((Action)(() =>
            {
                int show = index + 1;
                if (index < 0)
                {
                    timer.Close();
                    this.Close();
                }
            }));
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
                    qrImage.Source = imageSource;
                    qrImage.Stretch = Stretch.UniformToFill;
                    imageSource = null;
                }));
            })).Start();
        }

        private void btClose_Click(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            timer = null;
            this.Close();
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

        private void Window_Activated(object sender, EventArgs e)
        {
            makePosition();
        }
    }
}
