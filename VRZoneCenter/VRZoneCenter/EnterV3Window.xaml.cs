using System;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using VRZoneCenter.Classes.Forms;
using VRZoneCenter.Classes.Utils;
using VRZoneLib.Classes.Units;
using VRZoneLib.Classes.Utils;

namespace VRZoneCenter
{
    /// <summary>
    /// EnterV3Window.xaml 的交互逻辑
    /// </summary>
    public partial class EnterV3Window : Window, VZDownloadCallback
    {
        int maxRefresh;
        int frequencyOfRequest;
        System.Timers.Timer priceTimer;
        VZ_ZoneHelpWindow helpWin = null;
        bool isEnterGame = false;
        int counter = 0;

        public EnterV3Window()
        {
            InitializeComponent();
            //VZDownloadManager.getSingleton().setListener(this);

            //VZ_BGUpdater.getSingleton();
            frequencyOfRequest = VZ_AppHelper.GetInstance().systemInfo.frequencyOfRequest * 1000;
            maxRefresh = 1000 * 60 * 10 / frequencyOfRequest;
            priceTimer = new System.Timers.Timer(frequencyOfRequest);
            priceTimer.AutoReset = true;
            priceTimer.Elapsed += PriceTimer_Elapsed;
            priceTimer.Start();
            //VZ_BGUpdater.getSingleton().request4VersionInfo();
            tb_version.Text = VZ_AppHelper.GetInstance().systemInfo.version + " build(" + VZ_AppHelper.GetInstance().systemInfo.build + ")";
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
                    isEnterGame = true;
                    //VZDownloadManager.getSingleton().pause();
                    MainWindow win = new MainWindow();
                    win.Show();
                    if (helpWin != null && helpWin.IsVisible)
                    {
                        helpWin.Close();
                        helpWin = null;
                    }
                    this.Close();
                }));
            }
        }

        void updatePrice()
        {
            Dispatcher.BeginInvoke((Action)(() =>
            {
                VZ_AppProcessHelper.getSingleton().runTopVideo();
                this.Activate();
            }));
            counter--;
            if (counter <= 0)
            {
                ImageSource source = VZ_AppHelper.getQRCode();
                if (source != null)
                {
                    Dispatcher.BeginInvoke((Action)(() =>
                    {
                        imgCode.Source = source;
                        imgCode.Stretch = Stretch.UniformToFill;
                        source = null;
                    }));
                }
                counter = 1200;
            }
        }
        private void Window_Activated(object sender, EventArgs e)
        {
            makePosition();
            isEnterGame = false;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //VZDownloadManager.getSingleton().setListener(null);
            //VZDownloadManager.getSingleton().pause();
            if (priceTimer != null)
            {
                priceTimer.Stop();
                priceTimer = null;
            }
            if (!isEnterGame)
            {
                VZ_AppProcessHelper.getSingleton().closeTopVideo();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            VZ_AppProcessHelper.getSingleton().runTopVideo();
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

        private void bt_help_Click(object sender, RoutedEventArgs e)
        {
            helpWin = new VZ_ZoneHelpWindow();
            helpWin.Show();
            //VZ_FinishV2Window win = new VZ_FinishV2Window();
            //win.Show();
        }

        public void VZDownloadProgress(VZ_FileInfos info, int progress)
        {
            Dispatcher.BeginInvoke((Action)(() =>
            {
                tb_log.Text = "下载中 " + info.url + "  "+ progress + "%";
            }));
        }

        public void VZDownloadError(VZ_FileInfos info)
        {
            Dispatcher.BeginInvoke((Action)(() =>
            {
                tb_log.Text = "ERROR" + info.url;
            }));
        }

        public void VZDownloadComplete(VZ_FileInfos info)
        { 
            Dispatcher.BeginInvoke((Action)(() =>
            {
                tb_log.Text = "下载完毕 " + info.url;
            }));
        }

        public void VZDownloadAllComplete()
        {
            Dispatcher.BeginInvoke((Action)(() =>
            {
                tb_log.Text = "更新文件下载完毕";
                //VZ_BGUpdater.getSingleton().runUpdateApp();
            }));
        }

        public void VZDownloadMsg(String msg)
        {
            Dispatcher.BeginInvoke((Action)(() =>
            {
                tb_log.Text = msg;
            }));
        }

    }
}
