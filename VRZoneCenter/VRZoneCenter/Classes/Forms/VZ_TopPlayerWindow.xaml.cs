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
using System.Windows.Forms;
using VRZoneCenter.Classes.Utils;
using System.Threading;

namespace VRZoneCenter.Classes.Forms
{
    /// <summary>
    /// VZ_TopPlayerWindow.xaml 的交互逻辑
    /// </summary>
    public partial class VZ_TopPlayerWindow : Window
    {
        //System.Timers.Timer timer;
       // double currentTime = -1;
        public VZ_TopPlayerWindow()
        {
            InitializeComponent();
            //Uri uri = new Uri("/VRZoneCenter;component/Res/vrzone_top.mp4", UriKind.RelativeOrAbsolute);
            String path = AppDomain.CurrentDomain.BaseDirectory + @"\Res\vrzone_top.mp4";
            if (vlcPlayer == null)
            {
                return;
            }
            vlcPlayer.LoadMedia(path);
            vlcPlayer.Volume = 75;
            vlcPlayer.MinWidth = 1366;
            vlcPlayer.MinHeight = 760;
            vlcPlayer.IsMute = false;
            new Thread(new ThreadStart(() => {
                Thread.Sleep(500);
                Dispatcher.BeginInvoke((Action)(() => {
                    if (vlcPlayer == null)
                    {
                        return;
                    }
                    else
                    {
                        vlcPlayer.Play();
                    }
                }));
            })).Start();
            
        }

        public void play()
        {
            if(vlcPlayer != null)
            {
                vlcPlayer.Play();
            }
        }

        public void pause()
        {
            if(vlcPlayer != null)
            {
                vlcPlayer.Pause();
            }
        }




        void makePosition()
        {
            if (Screen.AllScreens.Length < 2)
            {
                this.Close();
                return;
            }
            Screen s = VZ_AppProcessHelper.getSingleton().getSecondScreen();

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

        protected override void OnActivated(EventArgs e)
        {
            makePosition();
        }

        public void setMuted(bool isMuted)
        {
            //mediaPlayer.IsMuted = isMuted;
            if(vlcPlayer != null)
            {
                vlcPlayer.IsMute = isMuted;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //if(timer != null)
            //{
            //    timer.Close();
            //    timer = null;
            //}
            if(vlcPlayer != null)
            {
                vlcPlayer.Stop();
                vlcPlayer.Dispose();
                
                vlcPlayer = null;
            }
        }
    }
}
