using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace VRZoneLib.Classes.Utils
{
    public static class ImageExtensions
    {
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        public static ImageSource get_Bitmap(string url)
        {
            IntPtr hBitmap = IntPtr.Zero;
            try
            {
                System.Net.WebRequest webreq = System.Net.WebRequest.Create(url);
                System.Net.WebResponse webres = webreq.GetResponse();
                System.IO.Stream stream = webres.GetResponseStream();
                System.Drawing.Image img1 = System.Drawing.Image.FromStream(stream);
                System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(img1);
                bmp.Save(AppDomain.CurrentDomain.BaseDirectory + @"\Datas\qr.png");
                hBitmap = bmp.GetHbitmap();
                ImageSource WpfBitmap = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                return WpfBitmap;
            }
            catch (Exception e)
            {
                return null;
            }
            //注意：一定要添加调用DeleteObject API否则存在大量内存泄漏
            finally
            {
                if (hBitmap != IntPtr.Zero)
                    DeleteObject(hBitmap);
            }
        }
        /// <summary> 从文件中读取 ImageSource </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>读取到的 ImageSource </returns>
        public static ImageSource GetImageSource(this string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    using (var fs = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read))
                    {
                        var binReader = new BinaryReader(fs);
                        byte[] bytes = binReader.ReadBytes((int)fs.Length);
                        var bitmap = new BitmapImage();
                        bitmap.CreateOptions = BitmapCreateOptions.IgnoreImageCache;

                        bitmap.BeginInit();
                        bitmap.StreamSource = new MemoryStream(bytes);
                        bitmap.EndInit();

                        return bitmap;
                    }
                }
            }
            catch { }
            return null;
        }

        /// <summary> 从文件中读取 ImageSource </summary>
        /// <param name="url">网络路径</param>
        /// <returns>读取到的 ImageSource </returns>
        public static ImageSource GetHttpImageSource(this string url)
        {
            try
            {
                var webRequest = HttpServices.CreatRequest(url);
                using (Stream responseStream = webRequest.GetResponse().GetResponseStream())
                {
                    var binReader = new BinaryReader(responseStream);
                    byte[] bytes = GetImageFromResponse(responseStream);
                    var bitmap = new BitmapImage();
                    bitmap.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                    bitmap.BeginInit();
                    bitmap.StreamSource = new MemoryStream(bytes);
                    bitmap.EndInit();
                    return bitmap;
                }
            }
            catch (Exception ex)
            {
            }
            return null;
        }

        private static byte[] GetImageFromResponse(Stream stream)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                Byte[] buffer = new Byte[1024];
                int current = 0;
                while ((current = stream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    ms.Write(buffer, 0, current);
                }
                return ms.ToArray();
            }
        }
    }
}
