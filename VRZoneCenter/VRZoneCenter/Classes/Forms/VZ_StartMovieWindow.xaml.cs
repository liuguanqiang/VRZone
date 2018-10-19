using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace VRZoneCenter.Classes.Forms
{
    /// <summary>
    /// VZ_StartMovieWindow.xaml 的交互逻辑
    /// </summary>
    public partial class VZ_StartMovieWindow : Window
    {
        public VZ_StartMovieWindow()
        {
            InitializeComponent();

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
