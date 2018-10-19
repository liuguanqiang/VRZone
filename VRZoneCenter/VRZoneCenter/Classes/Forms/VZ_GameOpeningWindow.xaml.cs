using System;
using System.Timers;
using System.Windows;
using VRZoneCenter.Classes.Utils;

namespace VRZoneCenter.Classes.Forms
{
    /// <summary>
    /// VZ_GameOpeningWindow.xaml 的交互逻辑
    /// </summary>
    public partial class VZ_GameOpeningWindow : Window
    {
        public VZ_GameOpeningWindow()
        {
            InitializeComponent();
            this.Topmost = true;
            Timer timer = new Timer(5000);
            timer.AutoReset = false;
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Dispatcher.BeginInvoke((Action)(() =>
            {
                this.Close();
            }));
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
