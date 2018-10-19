using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using VRZoneCenter.Classes.Utils;
using VRZoneLib.Classes.Utils;

namespace VRZoneCenter.Classes.Components
{
    public class VZ_VolumeController:Grid
    {
        Image[] volumeImg = new Image[10];
        Button volumeAddBt;
        Button volumeSubBt;
        int volumeValue = 50;

        public VZ_VolumeController()
        {
            this.Margin = new System.Windows.Thickness(20, 0, 20, 0);
            ColumnDefinition col1 = new ColumnDefinition();
            ColumnDefinition col2 = new ColumnDefinition();
            ColumnDefinition col3 = new ColumnDefinition();

            col1.Width = new System.Windows.GridLength(100);
            col2.Width = new System.Windows.GridLength(1, System.Windows.GridUnitType.Star);
            col3.Width = new System.Windows.GridLength(100);

            this.ColumnDefinitions.Add(col1);
            this.ColumnDefinitions.Add(col2);
            this.ColumnDefinitions.Add(col3);

            Grid gridV = new Grid();
            gridV.Margin = new System.Windows.Thickness(20, 0, 20, 0);
            Grid.SetColumn(gridV, 1);
            this.Children.Add(gridV);
            ColumnDefinition [] volumeCol = new ColumnDefinition[10];
            for (int i = 0; i < 10; i++)
            {
                volumeImg[i] = VZMethods.getImageContent(@"Res\volume_state_close.png");
                volumeImg[i].Width = 10;
                volumeImg[i].Height = 19;
                volumeImg[i].Stretch = Stretch.Fill;
                volumeCol[i] = new ColumnDefinition();
                volumeCol[i].Width = new System.Windows.GridLength(1, System.Windows.GridUnitType.Star);
                gridV.ColumnDefinitions.Add(volumeCol[i]);
                Grid.SetColumn(volumeImg[i], i);
                gridV.Children.Add(volumeImg[i]);
            }

            volumeAddBt = VZMethods.getNoBgImageButton(VZMethods.getImageContent(@"Res\volume_add.png"));
            volumeSubBt = VZMethods.getNoBgImageButton(VZMethods.getImageContent(@"Res\volume_sub.png"));
            volumeAddBt.Height = 80;
            volumeSubBt.Height = 80;
            Grid.SetColumn(volumeAddBt, 2);
            Grid.SetColumn(volumeSubBt, 0);
            this.Children.Add(volumeAddBt);
            this.Children.Add(volumeSubBt);
            volumeAddBt.Click += VolumeAddBt_Click;
            volumeSubBt.Click += VolumeSubBt_Click;

            SetsysVolume.SetVolunme(50);
            updateValumeIcon();
            return;
        }

        private void VolumeAddBt_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            volumeValue += 10;
            if (volumeValue >= 100)
            {
                volumeValue = 100;
            }
            SetsysVolume.SetVolunme(volumeValue);
            updateValumeIcon();
        }

        private void VolumeSubBt_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            volumeValue -= 10;
            if (volumeValue <= 0)
            {
                volumeValue = 0;
            }
            SetsysVolume.SetVolunme(volumeValue);
            updateValumeIcon();
        }

        private void updateValumeIcon()
        {
            for(int i = 0;i< 10;i++)
            {
                if(i * 10 < volumeValue)
                {
                    volumeImg[i].Source = new BitmapImage(new Uri(@"Res\volume_state_open.png", UriKind.Relative));
                }
                else
                {
                    volumeImg[i].Source = new BitmapImage(new Uri(@"Res\volume_state_close.png", UriKind.Relative));
                }
            }
        }
    }
}
