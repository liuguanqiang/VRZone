using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Timers;
using System.Windows;
using VRZoneLib.Classes.Units;
using VRZoneLib.Classes.Utils;

namespace VRZoneCenter.Classes.Utils
{
    class VZ_BGUpdater
    {
        private static VZ_BGUpdater singleton = null;
        public VZ_VersionInfo versionInfo;

        String updateList;

        private VZ_BGUpdater()
        {
            Timer timer = new Timer(3600*1000);
            timer.AutoReset = true;
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (VZDownloadManager.getSingleton().getCallback() != null)
            {
                request4VersionInfo();
            }
        }

        public static VZ_BGUpdater getSingleton()
        {
            if(singleton == null)
            {
                singleton = new VZ_BGUpdater();
            }
            return singleton;
        }

        public void request4VersionInfo()
        {
            try
            {
                String url = VZ_AppHelper.baseURL + "/wxpay/version";
                String tm = "" + VZMethods.ConvertDateTimeInt(DateTime.Now);
                Dictionary<String, String> dic = new Dictionary<String, String>();
                dic.Add("from", "vrzone");
                dic.Add("id", "" + VZ_AppHelper.GetInstance().systemInfo.computerId);
                dic.Add("sid", "iloveyouvrzone");
                dic.Add("tm", tm);
                dic.Add("uid", "1");
                String sign = "from=vrzone&id=" + VZ_AppHelper.GetInstance().systemInfo.computerId + "&sid=iloveyouvrzone&tm=" + tm + "&uid=1&--iloveyouvrzone";
                dic.Add("sign", VZMethods.strToMD5(sign).ToUpper());
                url = url + "?from=vrzone&id=" + VZ_AppHelper.GetInstance().systemInfo.computerId + "&sid=iloveyouvrzone&tm=" + tm + "&uid=1&sign=" + VZMethods.strToMD5(sign).ToUpper();
                HttpWebResponse response = HttpWebResponseUtility.CreateGetHttpResponse(url, 2000, null, null);
                Stream responseStream = response.GetResponseStream();
                StreamReader sr = new StreamReader(responseStream, Encoding.GetEncoding(response.CharacterSet));
                String msg = sr.ReadToEnd();
                versionInfo = JsonHelper.DeserializeJsonToObject<VZ_VersionInfo>(msg);
                checkVersion();
                return;
            }
            catch(Exception ex)
            {
                //MessageBox.Show(ex.ToString() + "");
                return;
            }
        }

        void getUpdateList()
        {
            if(versionInfo.version_url == null || versionInfo.version_url.Equals(""))
            {
                return;
            }
            else
            {
                HttpWebResponse response = HttpWebResponseUtility.CreateGetHttpResponse(versionInfo.version_url, 6000, null, null);
                Stream responseStream = response.GetResponseStream();
                StreamReader sr = new StreamReader(responseStream);
                String msg = sr.ReadToEnd();
                //保存到本地
                VZMethods.saveFile(AppDomain.CurrentDomain.BaseDirectory + @"Update\" + versionInfo.build + ".json", msg);
                if (msg != null && !msg.Equals(""))
                {
                    updateList = msg;
                }
            }
        }

        public void runUpdateApp()
        {
            if (!Directory.Exists(VZDownloadManager.getSingleton().updatePath))
            {
                checkVersion();
            }
            else
            {
                //启动更新程序,0 关闭主程序 1 覆盖项目，2 生成新的md5，3 删除更新文件
                try
                {
                    Process proc = new Process();
                    proc.StartInfo.FileName = AppDomain.CurrentDomain.BaseDirectory + "VRZoneUpdater.exe";
                    proc.Start();
                    Application.Current.Shutdown(-1);
                }
                catch (Exception ex)
                {

                }
                return;
            }
        }

        void checkVersion()
        {
            if(versionInfo == null || versionInfo.version == null)
            {
                return;
            }
            long currentBuild = VZ_AppHelper.GetInstance().systemInfo.build;            //当前版本
            long onlineBuild = versionInfo.build;                                        //线上最新版本
            long updateBuild = VZ_AppHelper.GetInstance().systemInfo.updateBuild;       //正在更新的版本

            //if(onlineBuild == currentBuild || onlineBuild != updateBuild)
            //{
            //    Properties.Settings.Default.isUpdateReady = false;
            //    Properties.Settings.Default.Save();
            //}
            //MessageBox.Show(currentBuild + " " + onlineBuild + " " + updateBuild + " ready?" + Properties.Settings.Default.isUpdateReady);

            if (onlineBuild > currentBuild)
            {
                if(onlineBuild == updateBuild)
                {
                    updateVRZone();
                }
                else if(currentBuild != updateBuild)
                {
                    if (VZDownloadManager.getSingleton().isUpdating())
                    {
                        VZDownloadManager.getSingleton().pause();
                    }
                    try
                    {
                        if (Directory.Exists(VZDownloadManager.getSingleton().updatePath))
                        {
                            Directory.Delete(VZDownloadManager.getSingleton().updatePath, true);
                        }
                    }
                    catch
                    {

                    }
                    VZ_AppHelper.GetInstance().systemInfo.updateBuild = VZ_AppHelper.GetInstance().systemInfo.build;
                    updateList = "";
                    updateVRZone();
                }
                else
                {
                    updateVRZone();
                }
            }
        }

        void updateVRZone()
        {
            if (updateList == null || updateList.Equals("") || updateList.Equals("\"\""))
            {
                getUpdateList();
            }
            if (VZDownloadManager.getSingleton().isUpdating()) return;

            String currentJsonPath = AppDomain.CurrentDomain.BaseDirectory + @"Update\current.json";
            VZ_AppHelper.GetInstance().systemInfo.updateBuild = versionInfo.build;
            saveBuildInfo();
            VZDownloadManager.getSingleton().setUpdateList(updateList, versionInfo.build, VZMethods.readFileToString(currentJsonPath));
            VZDownloadManager.getSingleton().start(null);
        }

        void saveBuildInfo()
        {
            VZ_SystemInfo info = VZ_AppHelper.GetInstance().systemInfo;
            String newConfig = JsonHelper.SerializeObject(info);
            VZMethods.saveFile(AppDomain.CurrentDomain.BaseDirectory + "\\Datas\\ConfigInfo.json", newConfig);
        }
    }
}
