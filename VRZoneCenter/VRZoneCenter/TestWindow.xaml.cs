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
using VRZoneLib.Classes.Units;
using VRZoneCenter.Classes.Utils;
using VRZoneLib.Classes.Utils;

namespace VRZoneCenter
{
    /// <summary>
    /// TestWindow.xaml 的交互逻辑
    /// </summary>
    public partial class TestWindow : Window ,VZDownloadCallback
    {
        public TestWindow()
        {
            InitializeComponent();

            String str = VZMethods.readFileToString(@"C:\Users\zwtd\Desktop\test.json");
            String str2 = VZMethods.readFileToString(AppDomain.CurrentDomain.BaseDirectory + @"Update\current.json");
            VZDownloadManager.getSingleton().setUpdateList(str, 2, str2);
        }

        private void bt_startUpdate_Click(object sender, RoutedEventArgs e)
        {
            VZDownloadManager.getSingleton().start(this);
        }

        private void bt_pauseUpdate_Click(object sender, RoutedEventArgs e)
        {
            VZDownloadManager.getSingleton().pause();
        }

        public void VZDownloadAllComplete()
        {
            MessageBox.Show("finish");
        }

        public void VZDownloadProgress(VZ_FileInfos info, int progress)
        {
            //throw new NotImplementedException();
            //Dispatcher.BeginInvoke((Action)(() =>
            //{
            //    tb_log.Text += ("progress:" + progress + "  " + info.name + "  " + tb_log.Text);
            //    tb_log.Text += "\n";
            //    tb_log.ScrollToEnd();
            //}));
        }

        public void VZDownloadError(VZ_FileInfos info)
        {
            //throw new NotImplementedException();
            //Dispatcher.BeginInvoke((Action)(() =>
            //{
            //    tb_log.Text += ("error:" + info.name + "  " + tb_log.Text);
            //    tb_log.Text += "\n";
            //    tb_log.ScrollToEnd();
            //}));
        }

        public void VZDownloadComplete(VZ_FileInfos info)
        {
            //throw new NotImplementedException();
            //Dispatcher.BeginInvoke((Action)(() =>
            //{
            //    tb_log.Text += ("complete:" + info.name + "  " + tb_log.Text);
            //    tb_log.Text += "\n";
            //    tb_log.ScrollToEnd();
            //}));
        }

        public void VZDownloadMsg(string msg)
        {
            
        }
    }
}
