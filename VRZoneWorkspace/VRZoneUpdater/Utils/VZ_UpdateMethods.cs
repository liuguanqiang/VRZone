using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.Threading;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using VRZoneLib.Classes.Units;

namespace VRZoneUpdater.Units
{
    public static class VZ_UpdateMethods
    {
        public static string strToMD5(string str)
        {
            byte[] data = Encoding.UTF8.GetBytes(str);
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] OutBytes = md5.ComputeHash(data);

            string OutString = "";
            for (int i = 0; i < OutBytes.Length; i++)
            {
                OutString += OutBytes[i].ToString("x2");
            }
            // return OutString.ToUpper();
            return OutString.ToLower();
        }


        public static int ConvertDateTimeInt(System.DateTime time)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            return (int)(time - startTime).TotalSeconds;
        }


        public static string MD5ToString(String argString)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] data = System.Text.Encoding.Default.GetBytes(argString);
            byte[] result = md5.ComputeHash(data);
            String strReturn = String.Empty;
            for (int i = 0; i < result.Length; i++)
                strReturn += result[i].ToString("x").PadLeft(2, '0');
            return strReturn;
        }

        public static string GetMD5HashFromFile(string fileName)
        {
            try
            {
                FileStream file = new FileStream(fileName, FileMode.Open);
                System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(file);
                file.Close();

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        public static List<VZ_FileInfos> getFull(String basePath)
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
                fs.md5 = GetMD5HashFromFile(info.FullName);
                fs.url = "";

            }

            foreach (DirectoryInfo info in dirs)
            {
                fileList.AddRange(getFiles(info, basePath));
            }
            return fileList;

        }

        public static void saveFile(String path,String data)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            FileStream fs = new FileStream(path, FileMode.Create);
            byte[] buf = Encoding.UTF8.GetBytes(data);
            fs.Write(buf, 0, buf.Length);
            fs.Flush();
            fs.Close();
        }

        public static String readFileToString(String path)
        {
            try
            {
                String str = "";
                using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    StreamReader sr = new StreamReader(fs, Encoding.UTF8);
                    str = sr.ReadToEnd();
                    sr.Close();
                }
                return str;
            }
            catch
            {
                return "";
            }
        }
        private static List<VZ_FileInfos> getFiles(DirectoryInfo dir, String basePath)
        {
            List<VZ_FileInfos> fileList = new List<VZ_FileInfos>();
            FileInfo[] files = dir.GetFiles();
            DirectoryInfo[] dirs = dir.GetDirectories();

            foreach (FileInfo info in files)
            {
                VZ_FileInfos fs = new VZ_FileInfos();
                fs.name = info.Name;
                fs.path = info.FullName.Substring(basePath.Length);
                fs.md5 = GetMD5HashFromFile(info.FullName);
                fs.url = "";
                fileList.Add(fs);
            }

            foreach (DirectoryInfo info in dirs)
            {
                fileList.AddRange(getFiles(info, basePath));
            }
            return fileList;
        }
    }
}
