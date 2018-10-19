using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using VRZoneCenter.Classes.Utils;
using VRZoneLib.Classes.Utils;

namespace VRZoneCenter.Classes.Components
{
    /// <summary>
    /// VZ_PagePoint.xaml 的交互逻辑
    /// </summary>
    public partial class VZ_PagePoint : UserControl
    {
        List<Image> imgList;
        BitmapImage bmWhite;
        BitmapImage bmGray;

        public VZ_PagePoint()
        {
            InitializeComponent();
            imgList = new List<Image>();
            bmWhite = VZMethods.getBitmap(@"pack://application:,,,/Res/point_white.png");
            bmGray = VZMethods.getBitmap(@"pack://application:,,,/Res/point_gray.png");
            VerticalAlignment = VerticalAlignment.Bottom;
            HorizontalAlignment = HorizontalAlignment.Center;
        }

        public void setState(int index,int total)
        {
            if (total == 0) return;
            //sp.Children.Clear();
            if (imgList.Count != total)
            {
                int count = imgList.Count;
                if (count < total)
                {
                    for (int i = 0; i < (total - count); i++)
                    {
                        Image img = new Image();
                        img.Stretch = System.Windows.Media.Stretch.Uniform;
                        img.Width = 15;
                        img.Height = 15;
                        img.VerticalAlignment = VerticalAlignment.Center;
                        img.HorizontalAlignment = HorizontalAlignment.Center;
                        img.Margin = new Thickness(10, 0, 10, 0);
                        img.Source = bmGray;
                        imgList.Add(img);           
                    }
                }
                else
                {
                    count = imgList.Count - total;
                    imgList.RemoveRange(total, count);
                }
            }
            Dispatcher.BeginInvoke((Action)(() =>
            {
                foreach (Image img in imgList)
                {
                    img.Source = bmGray;
                    if (!sp.Children.Contains(img))
                    {
                        sp.Children.Add(img);
                    }
                }
                imgList[index].Source = bmWhite;
            }));
        }
    }
}
