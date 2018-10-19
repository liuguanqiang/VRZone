using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace VRZoneLib
{

    /// <summary> 基于Http的数据请求 </summary>
    public static class HttpServices
    {
        /// <summary> 设置Cookie容器 此项能在需要的时候保存Cookies </summary>
        public static CookieContainer CookieContainers = new CookieContainer();

        public static Dictionary<string, string> Cookies = new Dictionary<string, string>();

        /// <summary> 设置需要证书请求的时候默认为true </summary>
        static HttpServices()
        {
            ServicePointManager.ServerCertificateValidationCallback += (se, cert, chain, sslerror) => { return true; };
        }

        /// <summary> 向HTTP流中添加数据头 </summary>
        /// <param name="url"> 请求的URL </param>
        /// <param name="method"> 请求使用的方法 GET、POST </param>
        /// <returns> 返回创建的 HttpWebRequest </returns>
        public static HttpWebRequest CreatRequest(this string url, string method = "get")
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);

            //优化多线程超时响应
            req.KeepAlive = false;

            //修改对象上允许的最大并发连接数
            ServicePointManager.DefaultConnectionLimit = 50;
            req.Method = method.ToUpper();
            req.AllowAutoRedirect = true;
            req.CookieContainer = CookieContainers;
            req.ContentType = "application/x-www-form-urlencoded";
            req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            //超时时间设置为5s
            req.Timeout = 5 * 1000;

            return req;
        }

        /// <summary> 根据URL获取回传的 Stream 无编码格式的确认 </summary>
        /// <param name="url"> 请求的URL </param>
        /// <returns> 返回的数据流 </returns>
        public static Stream GetResponseOfStream(this string url, string method = "get", string data = "")
        {
            try
            {
                var req = CreatRequest(url, method);

                if (method.ToUpper() == "POST" && data != null)
                {
                    var postBytes = new ASCIIEncoding().GetBytes(data);
                    req.ContentLength = postBytes.Length;
                    Stream st = req.GetRequestStream();
                    st.Write(postBytes, 0, postBytes.Length);
                    st.Close();
                }

                HttpWebResponse res = (HttpWebResponse)req.GetResponse();

                using (var stream = res.GetResponseStream())
                {
                    //优化多线程内存流的释放
                    MemoryStream ms = new MemoryStream();
                    stream.CopyTo(ms);
                    //接收到的数据流需要重新设置读取起始位
                    ms.Position = 0;

                    //优化多线程多个实例时的端口占用
                    res.Close();
                    req.Abort();

                    return ms;
                }
            }
            catch
            {
                return null;
            }
        }

        /// <summary> 以字符串形式获取返回值 </summary>
        /// <param name="url"> 请求的URl </param>
        /// <param name="method"> 传递方法 </param>
        /// <param name="data"> 传递数据 </param>
        /// <returns> 返回的字符串 UTF-8 编码 </returns>
        public static string GetResponseOfString(this string url, string method = "get", string data = "")
        {
            try
            {
                var req = CreatRequest(url, method);

                if (method.ToUpper() == "POST" && data != null)
                {
                    var postBytes = new ASCIIEncoding().GetBytes(data);
                    req.ContentLength = postBytes.Length;
                    Stream st = req.GetRequestStream();
                    st.Write(postBytes, 0, postBytes.Length);
                    st.Close();
                }

                HttpWebResponse res = (HttpWebResponse)req.GetResponse();

                foreach (Cookie cookie in res.Cookies)
                {
                    Cookies[cookie.Name] = cookie.Value;
                    CookieContainers.Add(cookie);
                }

                //优化多线程内存流的释放
                using (var stream = res.GetResponseStream())
                {
                    var sr = new StreamReader(stream, Encoding.UTF8).ReadToEnd();

                    //优化多线程多个实例时的端口占用
                    res.Close();
                    req.Abort();
                    return sr;
                }
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
