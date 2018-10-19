using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows;
using VRZoneLib.Classes.Units;
using VRZoneLib.Classes.Utils;
using VRZoneUpdater.Units;

namespace VRZoneUpdater
{
    /// <summary>
    /// UpdateWindow.xaml 的交互逻辑
    /// </summary>
    public partial class UpdateWindow : Window
    {
        VZ_VersionInfo versionInfo = null;
        List<VZ_FileInfos> md5List;
        List<String> noIndexDirList = new List<string>();

        public UpdateWindow()
        {
            InitializeComponent();

            noIndexDirList.Add(AppDomain.CurrentDomain.BaseDirectory + @"Update\");
            noIndexDirList.Add(AppDomain.CurrentDomain.BaseDirectory + @"Datas\");

            request4VersionInfo();
            if (versionInfo == null)
            {
                runVRZone();
                return;
            }
            //update();
            new Thread(new ThreadStart(() =>
            {
                update();
            })).Start();
        }

        private void addLog(String title, String des)
        {
            Dispatcher.BeginInvoke((Action)(() =>
            {
                tb_title.Text = title + " : " + des;
                tb_log.Text = tb_title.Text + "\n" + tb_log.Text;
            }));
        }

        private void updateProgress(int progress,int total)
        {
            Dispatcher.BeginInvoke((Action)(() =>
            {
                pb.Value = progress;
                pb.Maximum = total;
            }));
        }

        void update()
        {
            String updatePath = AppDomain.CurrentDomain.BaseDirectory + @"Update\" + versionInfo.build + @"\";
            if (Directory.Exists(updatePath))
            {
                //检查数据完整性
                addLog("数据完整性","检查中");
                String md5FilePath = AppDomain.CurrentDomain.BaseDirectory + @"Update\" + versionInfo.build + ".json";
                String str = VZ_UpdateMethods.readFileToString(md5FilePath);
                md5List = JsonHelper.DeserializeJsonToList<VZ_FileInfos>(str);
                String dirPath = AppDomain.CurrentDomain.BaseDirectory + @"Update\" + versionInfo.build + @"\";
                int i = 0;
                bool isMD5ok = true;
                foreach (VZ_FileInfos infos in md5List)
                {
                    String filePath = dirPath + infos.path;
                    if (!File.Exists(filePath)&&!File.Exists(AppDomain.CurrentDomain.BaseDirectory + infos.path))
                    {
                        addLog("数据完整性", filePath+" 没找到");
                        isMD5ok = false;
                        updateProgress(++i, md5List.Count);
                        continue;
                    }
                    if(File.Exists(AppDomain.CurrentDomain.BaseDirectory + infos.path) && !File.Exists(filePath))
                    {
                        updateProgress(++i, md5List.Count);
                        continue;
                    }
                    try
                    {
                        String md5 = VZ_UpdateMethods.GetMD5HashFromFile(filePath);
                        if (!md5.Equals(infos.md5))
                        {
                            //clearDirtyDatas();
                            addLog("数据完整性", filePath + " md5校验错误");
                            isMD5ok = false;
                            File.Delete(filePath);
                        }
                    }
                    catch(Exception ex)
                    {
                        
                    }
                    updateProgress(++i,md5List.Count);
                }
                if(!isMD5ok)
                {
                    //有个别文件有问题，删除
                    MessageBox.Show("校验失败");
                    runVRZone();
                    return;
                }
                addLog("数据完整性", "检查完毕");
                addLog("软件更新", "拷贝数据");
                i = 0;
                // 执行拷贝:
                String objPath = AppDomain.CurrentDomain.BaseDirectory;
                foreach (VZ_FileInfos infos in md5List)
                {
                    try
                    {
                        String path = System.IO.Path.GetDirectoryName(objPath + infos.path);
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                        if (File.Exists(dirPath + infos.path))
                        {
                            File.Copy(dirPath + infos.path, objPath + infos.path, true);
                        }
                    }
                    catch
                    {
                        addLog("软件更新", objPath + infos.path + "目标拷贝失败");
                        continue;
                    }
                    updateProgress(++i, md5List.Count);
                }
                addLog("软件更新", "更新完毕");
                // 清理旧数据
                addLog("更新缓存", "清理中");
                clearDirtyDatas();
                addLog("更新缓存", "清理完毕");
                //生成新的MD5
                addLog("生成新的文件索引", "进行中");

                String indexPath = AppDomain.CurrentDomain.BaseDirectory + @"Update\current.json";
                List<VZ_FileInfos> newList = getFull(AppDomain.CurrentDomain.BaseDirectory);
                String newListJson = JsonHelper.SerializeObject(newList);
                VZ_UpdateMethods.saveFile(indexPath, newListJson);

                addLog("生成新的文件索引", "生成完毕");
                // 更新版本信息
                addLog("更新版本信息", "更新中");
                String configPath = AppDomain.CurrentDomain.BaseDirectory + @"Datas\ConfigInfo.json";
                VZ_SystemInfo info = JsonHelper.DeserializeJsonToObject<VZ_SystemInfo>(VZ_UpdateMethods.readFileToString(configPath));
                info.build = versionInfo.build;
                info.onlineBuild = versionInfo.build;
                info.updateBuild = versionInfo.build;
                info.version = versionInfo.version;
                String newConfig = JsonHelper.SerializeObject(info);
                VZ_UpdateMethods.saveFile(configPath, newConfig);
                addLog("更新版本信息", "更新完毕");
                //finish
                addLog("升级完毕", "Version " + info.version);
            }
            else
            {
                MessageBox.Show("升级失败,未找到升级目录");
            }
            
            runVRZone();
        }

        void runVRZone()
        {
            try
            {
                Dispatcher.BeginInvoke((Action)(() =>
                {
                    try
                    {
                        String path = AppDomain.CurrentDomain.BaseDirectory + "VRZoneCenter.exe";
                        Process proc = new Process();
                        proc.StartInfo.FileName = path;
                        proc.Start();
                        this.Close();
                        Application.Current.Shutdown(-1);
                    }
                    catch
                    {
                        MessageBox.Show("启动主程序失败");
                    }
                }));
            }
            catch
            {
                MessageBox.Show("启动主程序失败");
            }
        }

        void clearDirtyDatas()
        {
            String dirPath = AppDomain.CurrentDomain.BaseDirectory + @"Update\" + versionInfo.build + @"\";
            if(Directory.Exists(dirPath))
            {
                Directory.Delete(dirPath,true);
            }
        }


        void request4VersionInfo()
        {
            try
            {
                String url = VZ_AppHelper.baseURL + "/wxpay/version";
                String tm = "" + VZ_UpdateMethods.ConvertDateTimeInt(DateTime.Now);
                Dictionary<String, String> dic = new Dictionary<String, String>();
                dic.Add("from", "vrzone");
                dic.Add("id", "" + VZ_AppHelper.getSingleton().systemInfo.computerId);
                dic.Add("sid", "iloveyouvrzone");
                dic.Add("tm", tm);
                dic.Add("uid", "1");
                String sign = "from=vrzone&id=" + VZ_AppHelper.getSingleton().systemInfo.computerId + "&sid=iloveyouvrzone&tm=" + tm + "&uid=1&--iloveyouvrzone";
                dic.Add("sign", VZ_UpdateMethods.strToMD5(sign).ToUpper());
                url = url + "?from=vrzone&id=" + VZ_AppHelper.getSingleton().systemInfo.computerId + "&sid=iloveyouvrzone&tm=" + tm + "&uid=1&sign=" + VZ_UpdateMethods.strToMD5(sign).ToUpper();
                HttpWebResponse response = HttpWebResponseUtility.CreateGetHttpResponse(url, 2000, null, null);
                Stream responseStream = response.GetResponseStream();
                StreamReader sr = new StreamReader(responseStream, Encoding.GetEncoding(response.CharacterSet));
                String msg = sr.ReadToEnd();
                versionInfo = JsonHelper.DeserializeJsonToObject<VZ_VersionInfo>(msg);
                return;
            }
            catch (Exception ex)
            {
                return;
            }
        }


        public List<VZ_FileInfos> getFull(String basePath)
        {
            List<VZ_FileInfos> fileList = new List<VZ_FileInfos>();
            DirectoryInfo dir = new DirectoryInfo(basePath);
            FileInfo[] files = dir.GetFiles();
            DirectoryInfo[] dirs = dir.GetDirectories();

            foreach (FileInfo info in files)
            {
                VZ_FileInfos fs = new VZ_FileInfos();
                fs.name = info.Name;
                fs.path = info.FullName.Substring(basePath.Length);
                try
                {
                    fs.md5 = VZ_UpdateMethods.GetMD5HashFromFile(info.FullName);
                }
                catch
                {
                    continue;
                }
                fs.url = "";
                fileList.Add(fs);
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
            //如果是非索引目录，返回
            if (noIndexDirList.Contains(dir.FullName)) return fileList;
            FileInfo[] files = dir.GetFiles();
            DirectoryInfo[] dirs = dir.GetDirectories();

            foreach (FileInfo info in files)
            {
                VZ_FileInfos fs = new VZ_FileInfos();
                fs.name = info.Name;
                fs.path = info.FullName.Substring(basePath.Length);
                try
                {
                    fs.md5 = VZ_UpdateMethods.GetMD5HashFromFile(info.FullName);
                }
                catch
                {
                    continue;
                }
                fileList.Add(fs);
                fs.url = "";
            }

            foreach (DirectoryInfo info in dirs)
            {
                fileList.AddRange(getFiles(info, basePath));
            }
            return fileList;
        }
    }
}
