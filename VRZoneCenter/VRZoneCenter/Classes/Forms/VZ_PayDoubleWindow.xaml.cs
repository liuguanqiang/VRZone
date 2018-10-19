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
    /// VZ_PayDoubleWindow.xaml 的交互逻辑
    /// </summary>
    public partial class VZ_PayDoubleWindow : Window
    {
        System.Timers.Timer timer;
        int index = 15;
        public VZ_PayDoubleWindow()
        {
            InitializeComponent();
            this.Topmost = true;
            

            timer = new System.Timers.Timer(1000);
            timer.AutoReset = true;
            timer.Elapsed += Timer_Elapsed;
            timer.Start();

            getQRCodeImageAsync(imgCode1, VZ_AppHelper.getQRCodeURL(2), 0);
            getQRCodeImageAsync(imgCode2, VZ_AppHelper.getQRCodeURL(3), 0);
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            index--;
            Dispatcher.BeginInvoke((Action)(() =>
            {
                int show = index + 1;
                tbTime.Text = "续时享优惠 请扫码支付(" + show + ")";
                tbPrice1.Text = VZ_AppHelper.GetInstance().payInfo.priceArr[2] + "元续时" + VZ_AppHelper.GetInstance().payInfo.watchtimeArr[2] + "分钟";
                if(VZ_AppHelper.GetInstance().payInfo.priceArr != null && VZ_AppHelper.GetInstance().payInfo.priceArr.Length >= 2)
                {
                    tbPrice2.Text = VZ_AppHelper.GetInstance().payInfo.priceArr[3] + "元续时" + VZ_AppHelper.GetInstance().payInfo.watchtimeArr[3] + "分钟";
                }
                if (index < 0)
                {
                    timer.Close();
                    this.Close();
                }
            }));
        }

        void getQRCodeImageAsync(Image img, String url, int index)
        {
            if (index > 3) return;
            new Thread(new ThreadStart(() => {

                ImageSource imageSource = ImageExtensions.get_Bitmap(url);
                if (imageSource == null)
                {
                    Thread.Sleep(2000);
                    getQRCodeImageAsync(img, url, index + 1);
                    return;
                }
                imageSource.Freeze();
                Dispatcher.BeginInvoke((Action)(() => {
                    img.Source = imageSource;
                    img.Stretch = Stretch.UniformToFill;
                    imageSource = null;
                }));
            })).Start();
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

        private void btClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
