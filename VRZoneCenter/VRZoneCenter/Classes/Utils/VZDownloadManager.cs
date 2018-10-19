using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using VRZoneLib.Classes.Units;
using VRZoneCenter.Classes.Utils;
using VRZoneLib.Classes.Utils;

namespace VRZoneCenter.Classes.Utils
{
    public interface VZDownloadCallback
    {
        void VZDownloadProgress(VZ_FileInfos info, int progress);
        void VZDownloadError(VZ_FileInfos info);
        void VZDownloadComplete(VZ_FileInfos info);

        void VZDownloadAllComplete();
        void VZDownloadMsg(String msg);
    }

    class VZDownloadManager
    {
        const int bytebuff = 1024;
        const int ReadWriteTimeOut = 1024;
        const int TimeOutWait = 1024;
        const int MaxTryTime = 12;
        static Dictionary<string, int> TryNumDic = new Dictionary<string, int>();
        Thread downloadThread = null;

        VZDownloadCallback callback = null;

        List<VZ_FileInfos> fileList;

        Dictionary<String, String> oldFileDic;

        private static VZDownloadManager singleton;

        public string updatePath = AppDomain.CurrentDomain.BaseDirectory + @"\Update\" + VZ_AppHelper.GetInstance().systemInfo.updateBuild + @"\";

        private VZDownloadManager()
        {
            //if(Properties.Settings.Default.updatelist == null || Properties.Settings.Default.updatelist.Equals(""))
            //{
            //    Properties.Settings.Default.updatelist = JsonHelper.SerializeObject("");
            //    Properties.Settings.Default.Save();
            //}
        }

        public static VZDownloadManager getSingleton()
        {
            if(singleton == null)
            {
                singleton = new VZDownloadManager();
            }
            return singleton;
        }

        public bool isUpdating()
        {
            if(downloadThread!= null && downloadThread.IsAlive)
            {
                return true;
            }
            else{
                return false;
            }
        }

        public VZDownloadCallback getCallback()
        {
            return callback;
        }

        public void setListener(VZDownloadCallback l)
        {
            callback = l;
        }

        public void start(VZDownloadCallback l)
        {
            updatePath = AppDomain.CurrentDomain.BaseDirectory + @"\Update\" + VZ_AppHelper.GetInstance().systemInfo.updateBuild + @"\";
            if (l != null)
            {
                callback = l;
            }
            if (fileList != null && fileList.Count > 0)
            {
                if (downloadThread == null)
                {
                    downloadThread = new Thread(new ThreadStart(() =>
                    {
                        int progress = 0;
                        for (;;)
                        {
                            if (fileList.Count <= 0)
                            {
                                //Properties.Settings.Default.isUpdateReady = true;
                                //Properties.Settings.Default.updatelist = "";
                                //Properties.Settings.Default.Save();
                                if (callback != null)
                                {
                                    callback.VZDownloadAllComplete();
                                }
                                break;
                            }
                            else
                            {
                                VZ_FileInfos infos = fileList[0];
                                try
                                {
                                    if(oldFileDic.ContainsKey(infos.path))
                                    {
                                        if(oldFileDic[infos.path].Equals(infos.md5))
                                        {
                                            if (callback != null)
                                            {
                                                callback.VZDownloadComplete(infos);
                                            }
                                            fileList.Remove(infos);
                                            continue;
                                        }
                                    }
                                    downloadFile(infos, infos.url, updatePath + infos.path, ref progress);
                                }
                                catch(Exception ex)
                                {
                                    break;
                                }
                                fileList.Remove(infos);
                            }
                        }
                    }));
                    downloadThread.Start();
                }
                else
                {
                    return;
                }
            }
        }

        public void pause()
        {
            if (downloadThread != null && downloadThread.IsAlive)
            {
                downloadThread.Abort();
            }
            downloadThread = null;
        }

        public void setUpdateList(String newJson,long buildNo, String currentJson)
        {
            oldFileDic = new Dictionary<string, string>();
            fileList = JsonHelper.DeserializeJsonToList<VZ_FileInfos>(currentJson);
            if(fileList != null)
            {

                foreach(VZ_FileInfos info in fileList)
                {
                    oldFileDic[info.path] = info.md5;
                }
                fileList.Clear();
                fileList = null;
            }

            fileList = JsonHelper.DeserializeJsonToList<VZ_FileInfos>(newJson);
        }

        /// <summary>
        /// 下载文件（同步）  支持断点续传
        /// </summary>
        /// <param name="url">文件url</param>
        /// <param name="savePath">本地保存路径</param>
        /// <param name="size">下载文件大小</param>
        void downloadFile(VZ_FileInfos info, String url, String savePath, ref int progress, long size = 0)
        {
            //打开上次下载的文件
            long lStartPos = 0;
            FileStream fs;
            if (File.Exists(savePath))
            {
                fs = File.OpenWrite(savePath);
                lStartPos = fs.Length;
                fs.Seek(lStartPos, SeekOrigin.Current);//移动文件流中的当前指针
            }
            else
            {
                string direName = Path.GetDirectoryName(savePath);
                if (!Directory.Exists(direName))//如果不存在保存文件夹路径，新建文件夹
                {
                    Directory.CreateDirectory(direName);
                }
                fs = new FileStream(savePath, FileMode.Create);
                lStartPos = 0;
            }

            HttpWebRequest request = null;
            try
            {
                if (size == 0)
                {
                    size = GetFileContentLength(url);
                }
                if (size != 0 && size == lStartPos)
                {
                    //下载完成
                    fs.Close();
                    return;
                }

                request = (HttpWebRequest)WebRequest.Create(url);
                request.ReadWriteTimeout = ReadWriteTimeOut;
                request.Timeout = TimeOutWait;
                if (lStartPos > 0)
                    request.AddRange((int)lStartPos);//设置Range值，断点续传

                //向服务器请求，获得服务器回应数据流
                WebResponse respone = request.GetResponse();
                long totalSize = respone.ContentLength + lStartPos;
                long curSize = lStartPos;
                progress = (int)(curSize / totalSize * 100.0f);

                Stream ns = respone.GetResponseStream();

                byte[] nbytes = new byte[bytebuff];
                int nReadSize = ns.Read(nbytes, 0, bytebuff);
                curSize += nReadSize;
                while (nReadSize > 0)
                {
                    fs.Write(nbytes, 0, nReadSize);
                    nReadSize = ns.Read(nbytes, 0, bytebuff);

                    curSize += nReadSize;
                    //下载进度计算
                    if (progress < (int)((double)curSize / totalSize * 100))
                        progress = (int)((double)curSize / totalSize * 100);
                    if (callback != null)
                    {
                        callback.VZDownloadProgress(info, progress);
                    }
                }
                fs.Flush();
                ns.Close();
                fs.Close();
                if (curSize != totalSize && totalSize != -1)//文件长度不等于下载长度，下载出错
                {
                    throw new Exception();
                }
                if (callback != null)
                {
                    callback.VZDownloadComplete(info);
                }
                if (request != null)
                {
                    request.Abort();
                }
                TryNumDic.Remove(url);
                //下载完毕

            }
            catch(Exception ex)
            {
                if (request != null)
                {
                    request.Abort();
                }

                fs.Close();
                if (TryNumDic.ContainsKey(url))
                {
                    if (TryNumDic[url] > MaxTryTime)
                    {
                        TryNumDic.Remove(url);
                        if (callback != null)
                        {
                            callback.VZDownloadError(info);
                        }
                        throw new Exception();
                    }
                    else
                    {
                        TryNumDic[url]++;
                    }
                }
                else
                {
                    TryNumDic.Add(url, 1);
                }
                downloadFile(info, url, savePath, ref progress, size);
            }
        }

        /// <summary>
        /// 获取下载文件长度
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public long GetFileContentLength(string url)
        {
            HttpWebRequest request = null;
            try
            {
                request = (HttpWebRequest)HttpWebRequest.Create(url);
                request.Timeout = ReadWriteTimeOut;
                request.ReadWriteTimeout = TimeOutWait;
                //向服务器请求，获得服务器回应数据流
                WebResponse respone = request.GetResponse();
                request.Abort();
                return respone.ContentLength;
            }
            catch (Exception e)
            {
                if (request != null)
                    request.Abort();
                return 0;
            }
        }
    }
}
