using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows;
using VRZoneLib.Classes.Units;
using VRZoneLib.Classes.Utils;
using VRZoneUpdater.Units;

namespace VRZoneUpdater
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        List<VZ_FileInfos> infos;
        String httpPath;
        List<String> noIndexDirList = new List<string>();

        public MainWindow()
        {
            InitializeComponent();

            noIndexDirList.Add(AppDomain.CurrentDomain.BaseDirectory + @"Update");
        }

        private void bt_select_dir_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();
            fbd.ShowDialog();
            if (fbd.SelectedPath != string.Empty)
            {
                tb_dir.Text = fbd.SelectedPath;
                bt_run.IsEnabled = true;
            }
            else
            {
                bt_run.IsEnabled = false;
            }
        }

        private void bt_run_Click(object sender, RoutedEventArgs e)
        {
            String path = tb_dir.Text;
            noIndexDirList.Add(path + @"\Update");
            noIndexDirList.Add(path + @"\Datas\ConfigInfo.json");
            httpPath = tb_oss_root.Text;
            if (infos != null)
            {
                infos.Clear();
                infos = null;
            }
            new Thread(new ThreadStart(() => {
                infos = getFull(path);
                Dispatcher.BeginInvoke((Action)(() => {
                    MessageBox.Show("生成完毕");
                    bt_save.IsEnabled = true;
                }));
            })).Start();
           
        }

        void addLog(VZ_FileInfos info)
        {
            tb_info.Text += info.name;
            tb_info.Text += "    ";
            tb_info.Text += info.md5;
            tb_info.Text += "    ";
            tb_info.Text += info.path;
            tb_info.Text += "\n";
            tb_info.ScrollToEnd();
        }

        public List<VZ_FileInfos> getFull(String basePath)
        {
            List<VZ_FileInfos> fileList = new List<VZ_FileInfos>();
            DirectoryInfo dir = new DirectoryInfo(basePath);
            FileInfo[] files = dir.GetFiles();
            DirectoryInfo[] dirs = dir.GetDirectories();

            foreach (FileInfo info in files)
            {
                if (info.FullName.EndsWith("VRZoneUpdater.exe"))
                {
                    continue;
                }
                VZ_FileInfos fs = new VZ_FileInfos();
                fs.name = info.Name;
                fs.path = info.FullName.Substring(basePath.Length + 1);
                fs.md5 = VZ_UpdateMethods.GetMD5HashFromFile(info.FullName);
                fs.url = httpPath + info.FullName.Substring(basePath.Length + 1).Replace("\\", "/");
                fileList.Add(fs);
                Dispatcher.BeginInvoke((Action)(() => {
                    addLog(fs);
                }));
            }

            foreach (DirectoryInfo info in dirs)
            {
                fileList.AddRange(getFiles(info, basePath));
            }
            return fileList;

        }

        private List<VZ_FileInfos> getFiles(DirectoryInfo dir, String basePath)
        {
            List<VZ_FileInfos> fileList = new List<VZ_FileInfos>();
            if (noIndexDirList.Contains(dir.FullName)) return fileList;
            FileInfo[] files = dir.GetFiles();
            DirectoryInfo[] dirs = dir.GetDirectories();

            foreach (FileInfo info in files)
            {
                VZ_FileInfos fs = new VZ_FileInfos();
                if (info.FullName.EndsWith("ConfigInfo.json"))
                {
                    continue;
                }
                if (noIndexDirList.Contains(info.FullName))
                {
                    continue;
                }
                fs.name = info.Name;
                fs.path = info.FullName.Substring(basePath.Length + 1);
                fs.md5 = VZ_UpdateMethods.GetMD5HashFromFile(info.FullName);
                fs.url = httpPath + info.FullName.Substring(basePath.Length + 1).Replace("\\", "/");
                fileList.Add(fs);
                Dispatcher.BeginInvoke((Action)(() => {
                    addLog(fs);
                }));
            }

            foreach (DirectoryInfo info in dirs)
            {
                fileList.AddRange(getFiles(info, basePath));
            }
            return fileList;
        }

        private void bt_save_Click(object sender, RoutedEventArgs e)
        {
            String json = JsonHelper.SerializeObject(infos);
            System.Windows.Forms.SaveFileDialog sf = new System.Windows.Forms.SaveFileDialog();
            sf.FileName = "vrzone_update";
            sf.Filter = "json files (*.json)|*.json";
            sf.RestoreDirectory = true;
            if (sf.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                StreamWriter myStream = new StreamWriter(sf.FileName);
                myStream.Write(json); //写入
                myStream.Close();//关闭流
            }
        }
    }
}
