using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VRZoneLib.Classes.Units;
using System.IO;
using System.Net;
using System.Timers;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows;

namespace VRZoneLib.Classes.Utils
{
    public class VZ_AppHelper
    {
        private static VZ_AppHelper instance = null;

        public List<VZ_AppInfo> appList;
        public VZ_SystemInfo systemInfo;
        public VZ_StateInfo payInfo = new VZ_StateInfo();

        private Timer timer = null;

        public static String baseURL = @"http://api.vrkongfu.cn/V1/";
        //static String baseURL = @"http://api.360looker.com/V2/";

        public static VZ_AppHelper GetInstance()
        {
            if (instance == null)
            {
                instance = new VZ_AppHelper();
            }
            return instance;
        }

        public void close()
        {
            if (timer != null)
            {
                timer.Stop();
                timer = null;
            }
        }

        public void debugMsg(String msg)
        {
            if (true)
            {
                MessageBox.Show(msg);
            }
        }

        private VZ_AppHelper()
        {
            clearQR();

            String jsonStr = readJson(AppDomain.CurrentDomain.BaseDirectory + "\\Datas\\ConfigInfo.json");
            systemInfo = JsonHelper.DeserializeJsonToObject<VZ_SystemInfo>(jsonStr);

            jsonStr = readJson(AppDomain.CurrentDomain.BaseDirectory + "\\Datas\\app.json");
            appList = JsonHelper.DeserializeJsonToList<VZ_AppInfo>(jsonStr);
            foreach (VZ_AppInfo info in appList)
            {
                info.appImgPath = AppDomain.CurrentDomain.BaseDirectory + "Apps\\" + info.appImgPath;
                info.appPath = AppDomain.CurrentDomain.BaseDirectory + "Apps\\" + info.appPath;
                info.bgPath = AppDomain.CurrentDomain.BaseDirectory + "Apps\\" + info.bgPath;
                info.moviePath = AppDomain.CurrentDomain.BaseDirectory + "Apps\\" + info.moviePath;
            }

            int counter = appList.Count;
            for (int i = counter - 1; i >= 0; i--)
            {
                if (!File.Exists(appList[i].appImgPath))
                {
                    appList.RemoveAt(i);
                }
            }

            getServerInfo();
            timer = new Timer(systemInfo.frequencyOfRequest * 1000);
            timer.Elapsed += Timer_Elapsed;
            timer.AutoReset = true;
            timer.Start();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            getServerInfo();
        }

        private String readJson(String path)
        {
            try
            {
                StreamReader sr = new StreamReader(path, System.Text.Encoding.UTF8);
                string temp = sr.ReadToEnd();
                sr.Close();
                return temp;
            }
            catch
            {
                return "";
            }
        }

        public void getServerInfo()
        {
            try
            {
                String url = baseURL + "wxpay/pay";
                String tm = "" + VZMethods.ConvertDateTimeInt(DateTime.Now);
                Dictionary<String, String> dic = new Dictionary<string, string>();
                dic.Add("from", "vrzone");
                dic.Add("id", "" + systemInfo.computerId);
                dic.Add("sid", "iloveyouvrzone");
                dic.Add("tm", tm);
                dic.Add("uid", "1");
                String sign = "from=vrzone&id=" + systemInfo.computerId + "&sid=iloveyouvrzone&tm=" + tm + "&uid=1&--iloveyouvrzone";
                dic.Add("sign", VZMethods.strToMD5(sign).ToUpper());
                url = url + "?from=vrzone&id=" + systemInfo.computerId + "&sid=iloveyouvrzone&tm=" + tm + "&uid=1&sign=" + VZMethods.strToMD5(sign).ToUpper();
                HttpWebResponse response = HttpWebResponseUtility.CreateGetHttpResponse(url, 2000, null, null);
                Stream responseStream = response.GetResponseStream();
                StreamReader sr = new StreamReader(responseStream, Encoding.GetEncoding(response.CharacterSet));
                String msg = sr.ReadToEnd();
                payInfo = JsonHelper.DeserializeJsonToObject<VZ_StateInfo>(msg);
                return;
            }
            catch
            {
                return;
            }
        }

        public void sendAppLog(VZ_AppInfo info)
        {
            try
            {
                String url = baseURL + "wxpay/wxmovieplay";
                String tm = "" + VZMethods.ConvertDateTimeInt(DateTime.Now);
                Dictionary<String, String> dic = new Dictionary<string, string>();
                dic.Add("from", "vrzone");
                dic.Add("id", "" + systemInfo.computerId);
                dic.Add("sid", "iloveyouvrzone");
                dic.Add("tm", tm);
                dic.Add("uid", "1");
                dic.Add("cate", info.type);
                dic.Add("fname", info.appName);
                String sign = "cate=" + info.type + "&fname=" + info.appName + "&from=vrzone&id=" + systemInfo.computerId + "&sid=iloveyouvrzone&tm=" + tm + "&uid=1&--iloveyouvrzone";
                dic.Add("sign", VZMethods.strToMD5(sign).ToUpper());
                HttpWebResponse response = HttpWebResponseUtility.CreatePostHttpResponse(url, dic, 2000, null, Encoding.UTF8, null);
                Stream responseStream = response.GetResponseStream();
                StreamReader sr = new StreamReader(responseStream, Encoding.GetEncoding(response.CharacterSet));
                String msg = sr.ReadToEnd();
                return;
            }
            catch
            {

            }
        }

        public void sendAppLog(VZ_AppInfo info, int time)
        {
            try
            {
                String url = baseURL + "wxpay/wxmovieplay";
                String tm = "" + VZMethods.ConvertDateTimeInt(DateTime.Now);
                Dictionary<String, String> dic = new Dictionary<string, string>();
                dic.Add("from", "vrzone");
                dic.Add("id", "" + systemInfo.computerId);
                dic.Add("sid", "iloveyouvrzone");
                dic.Add("tm", tm);
                dic.Add("uid", "1");
                dic.Add("cate", info.type);
                dic.Add("fname", info.appName);
                dic.Add("play_time", "" + time);
                String sign = "cate=" + info.type + "&fname=" + info.appName + "&from=vrzone&id=" + systemInfo.computerId + "&play_time=" + time + "&sid=iloveyouvrzone&tm=" + tm + "&uid=1&--iloveyouvrzone";
                dic.Add("sign", VZMethods.strToMD5(sign).ToUpper());
                HttpWebResponse response = HttpWebResponseUtility.CreatePostHttpResponse(url, dic, 2000, null, Encoding.UTF8, null);
                Stream responseStream = response.GetResponseStream();
                StreamReader sr = new StreamReader(responseStream, Encoding.GetEncoding(response.CharacterSet));
                String msg = sr.ReadToEnd();
                return;
            }
            catch
            {

            }
        }

        public static void clearQR()
        {
            String local = AppDomain.CurrentDomain.BaseDirectory + @"\Datas\qr.png";
            if (File.Exists(local))
            {
                File.Delete(local);
            }
        }

        public static ImageSource getQRCode()
        {
            String url = getQRCodeAddres();// baseURL + @"wechat/qrcode?deviceid=" + VZ_AppHelper.getSingleton().systemInfo.computerId;
            String local = AppDomain.CurrentDomain.BaseDirectory + @"\Datas\qr.png";
            ImageSource imageSource = null;

            if (File.Exists(local))
            {
                BitmapImage bi = new BitmapImage(new Uri(local, UriKind.Absolute));
                imageSource = bi;
                if (imageSource != null)
                {
                    imageSource.Freeze();
                    return imageSource;
                }
            }

            for (int i = 0; i < 10; i++)
            {
                imageSource = ImageExtensions.get_Bitmap(url);
                if (imageSource != null)
                {
                    break;
                }
            }
            imageSource.Freeze();
            return imageSource;
        }

        public static String getQRCodeAddres()
        {
            String url = baseURL + "wechat/qrcode?";
            String tm = "" + VZMethods.ConvertDateTimeInt(DateTime.Now);

            String sign = "from=vrzone&deviceid=" + VZ_AppHelper.GetInstance().systemInfo.computerId + "&sid=iloveyouvrzone&tm=" + tm + "&uid=1&--iloveyouvrzone";
            sign = VZMethods.strToMD5(sign).ToUpper();
            url += "from=vrzone&deviceid=" + VZ_AppHelper.GetInstance().systemInfo.computerId + "&sid=iloveyouvrzone&tm=" + tm + "&uid=1&sign=" + sign;
            return url;
        }

        #region 获取二维码链接，1.2.2 以前的版本
        public static String getQRCodeURL()
        {
            String url = baseURL + "wxpay/qrcode?";
            String tm = "" + VZMethods.ConvertDateTimeInt(DateTime.Now);

            String sign = "from=vrzone&id=" + VZ_AppHelper.GetInstance().systemInfo.computerId + "&sid=iloveyouvrzone&tm=" + tm + "&uid=1&--iloveyouvrzone";
            sign = VZMethods.strToMD5(sign).ToUpper();
            url += "from=vrzone&id=" + VZ_AppHelper.GetInstance().systemInfo.computerId + "&sid=iloveyouvrzone&tm=" + tm + "&uid=1&sign=" + sign;
            return url;
        }

        public static String getQRCodeURL(int index)
        {
            String url = baseURL + "wxpay/qrcode?";
            String tm = "" + VZMethods.ConvertDateTimeInt(DateTime.Now);

            String sign = "from=vrzone&id=" + VZ_AppHelper.GetInstance().systemInfo.computerId + "&sid=iloveyouvrzone&sonid=" + (index + 1) + "&tm=" + tm + "&uid=1&--iloveyouvrzone";
            sign = VZMethods.strToMD5(sign).ToUpper();
            url += "from=vrzone&id=" + VZ_AppHelper.GetInstance().systemInfo.computerId + "&sid=iloveyouvrzone&sonid=" + (index + 1) + "&tm=" + tm + "&uid=1&sign=" + sign;
            return url;
        }
        #endregion
    }
}
