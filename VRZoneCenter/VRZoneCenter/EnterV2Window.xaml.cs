using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using VRZoneCenter.Classes.Forms;
using VRZoneCenter.Classes.Utils;
using VRZoneLib.Classes.Utils;

namespace VRZoneCenter
{
    /// <summary>
    /// EnterV2Window.xaml 的交互逻辑
    /// </summary>
    public partial class EnterV2Window : Window
    {

        int maxRefresh;
        int counter = 0;
        int frequencyOfRequest;
        System.Timers.Timer priceTimer;
        string price = "";
        string price2 = "";

        VZ_ZoneHelpWindow helpWin = null;

        public EnterV2Window()
        {
            InitializeComponent();

            frequencyOfRequest = VZ_AppHelper.GetInstance().systemInfo.frequencyOfRequest * 1000;
            maxRefresh = 1000 * 60 * 10 / frequencyOfRequest;

            priceTimer = new System.Timers.Timer(frequencyOfRequest);
            priceTimer.AutoReset = true;
            priceTimer.Elapsed += PriceTimer_Elapsed;
            priceTimer.Start();

            tb_version.Text = VZ_AppHelper.GetInstance().systemInfo.version;
        }

        void makePosition()
        {
            Screen s = VZ_AppProcessHelper.getSingleton().getMainScreen();

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

        private void PriceTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (VZ_AppHelper.GetInstance().payInfo == null) return;
            updatePrice();

            VZ_AppProcessHelper.getSingleton().quitSteamVRHome();

            if ((VZ_AppHelper.GetInstance().payInfo.expire > VZ_AppHelper.GetInstance().payInfo.current_time) || VZ_AppHelper.GetInstance().systemInfo.isDebug)
            {
                Dispatcher.BeginInvoke((Action)(() =>
                {
                    MainWindow win = new MainWindow();
                    win.Show();
                    //win.showHelp();
                    if(helpWin != null && helpWin.IsVisible)
                    {
                        helpWin.Close();
                        helpWin = null;
                    }
                    this.Close();
                    //VZ_PayOKWindow payWin = new VZ_PayOKWindow();
                    //payWin.Show();
                }));
            }
        }

        void updatePrice()
        {
            counter++;
            Dispatcher.BeginInvoke((Action)(() =>
            {
                VZ_AppProcessHelper.getSingleton().runTopVideo();
                tb1.Text = VZ_AppHelper.GetInstance().payInfo.price + "元 / " + VZ_AppHelper.GetInstance().payInfo.watch_time + "分钟";
            }));

            //价格1
            if (price != VZ_AppHelper.GetInstance().payInfo.price)
            {
                string url = getQRCodeURL();
                getQRCodeImageAsync(imgCode1, url, 0);
            }
            else if (counter > maxRefresh)
            {
                string url = getQRCodeURL();
                getQRCodeImageAsync(imgCode1, url, 0);
            }
            price = VZ_AppHelper.GetInstance().payInfo.price;

            //价格2
            if (VZ_AppHelper.GetInstance().payInfo.priceArr != null && VZ_AppHelper.GetInstance().payInfo.watchtimeArr != null)
            {
                if (VZ_AppHelper.GetInstance().payInfo.priceArr.Length >= 2 && VZ_AppHelper.GetInstance().payInfo.watchtimeArr.Length >= 2)
                {
                    if (price2 != VZ_AppHelper.GetInstance().payInfo.priceArr[1])
                    {
                        string url = getQRCodeURL(1);
                        getQRCodeImageAsync(imgCode2, url, 0);
                    }
                    else if (counter > maxRefresh)
                    {
                        string url = getQRCodeURL(1);
                        getQRCodeImageAsync(imgCode2, url, 0);
                    }
                    Dispatcher.BeginInvoke((Action)(() =>
                    {
                        tb2.Text = VZ_AppHelper.GetInstance().payInfo.priceArr[1] + "元 / " + VZ_AppHelper.GetInstance().payInfo.watchtimeArr[1] + "分钟";
                    }));
                    price2 = VZ_AppHelper.GetInstance().payInfo.priceArr[1];
                }
                else
                {
                    Dispatcher.BeginInvoke((Action)(() =>
                    {
                        tb2.Text = "";
                    }));
                }
            }
            
            if (counter > maxRefresh)
            {
                counter = 0;
            }
        }

        void getQRCodeImageAsync(Image img,String url,int index)
        {
            if (index > 3) return;
            new Thread(new ThreadStart(() => {

                ImageSource imageSource = ImageExtensions.get_Bitmap(url);
                if (imageSource == null)
                {
                    Thread.Sleep(2000);
                    getQRCodeImageAsync(img,url,index + 1);
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

        public String getQRCodeURL()
        {
            String url = "http://api.360looker.com/V2/wxpay/qrcode?";
            String tm = "" + VZMethods.ConvertDateTimeInt(DateTime.Now);

            String sign = "from=vrzone&id=" + VZ_AppHelper.GetInstance().systemInfo.computerId + "&sid=iloveyouvrzone&tm=" + tm + "&uid=1&--iloveyouvrzone";
            sign = VZMethods.strToMD5(sign).ToUpper();
            url += "from=vrzone&id=" + VZ_AppHelper.GetInstance().systemInfo.computerId + "&sid=iloveyouvrzone&tm=" + tm + "&uid=1&sign=" + sign;
            return url;
        }

        public String getQRCodeURL(int index)
        {
            String url = "http://api.360looker.com/V2/wxpay/qrcode?";
            String tm = "" + VZMethods.ConvertDateTimeInt(DateTime.Now);

            String sign = "from=vrzone&id=" + VZ_AppHelper.GetInstance().systemInfo.computerId + "&sid=iloveyouvrzone&sonid=" + (index + 1) + "&tm=" + tm + "&uid=1&--iloveyouvrzone";
            sign = VZMethods.strToMD5(sign).ToUpper();
            url += "from=vrzone&id=" + VZ_AppHelper.GetInstance().systemInfo.computerId + "&sid=iloveyouvrzone&sonid=" + (index + 1) + "&tm=" + tm + "&uid=1&sign=" + sign;
            return url;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            VZ_AppProcessHelper.getSingleton().runTopVideo();
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            makePosition();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (priceTimer != null)
            {
                priceTimer.Stop();
                priceTimer = null;
            }
            VZ_AppProcessHelper.getSingleton().closeTopVideo();
        }

        private void bt_help_Click(object sender, RoutedEventArgs e)
        {
            helpWin = new VZ_ZoneHelpWindow();
            helpWin.Show();
        }
    }
}
