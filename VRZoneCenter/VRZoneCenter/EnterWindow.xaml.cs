using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using System.Security.Cryptography;
using VRZoneCenter.Classes.Utils;
using System.Threading;
using System.IO;
using System.ComponentModel;
using MahApps.Metro.Controls;
using VRZoneCenter.Classes.Forms;
using System.Windows.Forms;
using VRZoneLib.Classes.Utils;

namespace VRZoneCenter
{
    /// <summary>
    /// EnterWindow.xaml 的交互逻辑
    /// </summary>
    public partial class EnterWindow : Window
    {
        Image qrImage;
        TextBlock tb;
        string price;
        System.Timers.Timer priceTimer;

        int maxRefresh;
        int counter = 0;
        int frequencyOfRequest;

        public EnterWindow()
        {
            InitializeComponent();
            initUI();

            frequencyOfRequest = VZ_AppHelper.GetInstance().systemInfo.frequencyOfRequest * 1000;
            maxRefresh = 1000 * 60 * 10 / frequencyOfRequest;

            priceTimer = new System.Timers.Timer(frequencyOfRequest);
            priceTimer.AutoReset = true;
            priceTimer.Elapsed += PriceTimer_Elapsed;
            priceTimer.Start();
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
            updatePrice();
            
            if ((VZ_AppHelper.GetInstance().payInfo.expire > VZ_AppHelper.GetInstance().payInfo.current_time) || VZ_AppHelper.GetInstance().systemInfo.isDebug)
            {
                Dispatcher.BeginInvoke((Action)(() =>
                {
                    MainWindow win = new MainWindow();
                    win.Show();
                    this.Close();

                    VZ_PayOKWindow payWin = new VZ_PayOKWindow();
                    payWin.Show();
                }));
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (priceTimer != null)
            {
                priceTimer.Stop();
                priceTimer = null;
            }
            VZ_AppProcessHelper.getSingleton().hideTopVideo();
        }


        void initUI()
        {
            ColumnDefinition col1 = new ColumnDefinition();
            ColumnDefinition col2 = new ColumnDefinition();
            col1.Width = new GridLength(1.5, GridUnitType.Star);
            col2.Width = new GridLength(2, GridUnitType.Star);
            mainGrid.ColumnDefinitions.Add(col1);
            mainGrid.ColumnDefinitions.Add(col2);

            qrImage = new Image();
            qrImage.Width = 400;
            qrImage.Height = 400;
            mainGrid.Children.Add(qrImage);
            Grid.SetColumn(qrImage, 0);

            tb = new TextBlock();
            tb.Width = 800;
            tb.Height = 120;
            tb.FontSize = 100;
            tb.Text = "?元 / ?分钟";
            //tb.Background = new SolidColorBrush(Colors.Red);
            tb.TextAlignment = TextAlignment.Center;
            tb.Foreground = new SolidColorBrush(Colors.White);
            tb.Margin = new Thickness(0, 0, 125, 400);
            mainGrid.Children.Add(tb);
            Grid.SetColumn(tb, 1);

            getQRCodeImageAsync();
        }

        void updatePrice()
        {
            counter++;
            Dispatcher.BeginInvoke((Action)(() =>
            {
                VZ_AppProcessHelper.getSingleton().runTopVideo();
                tb.Text = VZ_AppHelper.GetInstance().payInfo.price + "元 / " + VZ_AppHelper.GetInstance().payInfo.watch_time + "分钟";
            }));
            if(price != VZ_AppHelper.GetInstance().payInfo.price)
            {
                getQRCodeImageAsync();
            }
            else if(counter > maxRefresh)
            {
                counter = 0;
                getQRCodeImageAsync();
            }
            price = VZ_AppHelper.GetInstance().payInfo.price;
        }

        void getQRCodeImageAsync()
        {
            String url = getQRCodeURL();

            new Thread(new ThreadStart(() => {

                ImageSource imageSource = ImageExtensions.get_Bitmap(url);
                if(imageSource == null)
                {
                    Thread.Sleep(2000);
                    getQRCodeImageAsync(1);
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
        void getQRCodeImageAsync(int index)
        {
            if (index > 3) return;
            String url = getQRCodeURL();
            new Thread(new ThreadStart(() => {

                ImageSource imageSource = ImageExtensions.get_Bitmap(url);
                if (imageSource == null)
                {
                    Thread.Sleep(2000);
                    getQRCodeImageAsync(++index);
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

        public String getQRCodeURL()
        {
            String url = "http://api.360looker.com/V2/wxpay/qrcode?";
            String tm = "" + VZMethods.ConvertDateTimeInt(DateTime.Now);

            String sign = "from=vrzone&id=" + VZ_AppHelper.GetInstance().systemInfo.computerId + "&sid=iloveyouvrzone&tm=" + tm + "&uid=1&--iloveyouvrzone";
            sign = VZMethods.strToMD5(sign).ToUpper();
            url += "from=vrzone&id=" + VZ_AppHelper.GetInstance().systemInfo.computerId + "&sid=iloveyouvrzone&tm=" + tm + "&uid=1&sign=" + sign;
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

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            VZ_AppProcessHelper.getSingleton().closeTopVideo();
        }
    }
}
