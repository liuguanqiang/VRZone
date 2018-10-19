using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VRZoneLib.Classes.Units;
using VRZoneCenter.Classes.Utils;

namespace VRZoneCenter.Classes.Components
{
    /// <summary>
    /// VZ_GameInfoUC.xaml 的交互逻辑
    /// </summary>
    public partial class VZ_GameInfoUC : UserControl
    {

        public interface VZ_GameInfoUCCallback
        {
            void onClose(VZ_GameInfoUC view);
            void onPlay(VZ_GameInfoUC view,VZ_AppInfo info);
            void onQuit(VZ_GameInfoUC view, VZ_AppInfo info);
        }

        VZ_GameInfoUCCallback callback;
        VZ_AppInfo gameInfo;
        bool isGameInPlaying = false;

        public VZ_GameInfoUC(VZ_AppInfo info, VZ_GameInfoUCCallback listener)
        {
            try
            {
                InitializeComponent();
            }catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            callback = listener;
            gameInfo = info;
            if (info != null)
            {
                try
                {
                    //gameVideo.Source = new Uri(info.moviePath, UriKind.Absolute);
                    gameVideo.LoadMedia(info.moviePath);
                    gameVideo.Width = 1300;
                    gameVideo.Height = 780;
                    gameVideo.Volume = 75;
                    gameVideo.IsMute = false;
                    new Thread(new ThreadStart(() => {
                        //Thread.Sleep(500);
                        Dispatcher.BeginInvoke((Action)(() => {
                            if(gameVideo != null)
                            {
                                gameVideo.Play();
                            }
                        }));
                    })).Start();
                    bgImage.Source = new BitmapImage(new Uri(info.bgPath, UriKind.Absolute));
                }
                catch (Exception ex)
                {
                    Scratch.Log.LogConfig.Logger.Error("出错：", ex);
                }
            }

            updateState();
        }

        public void updateState()
        {
            if (VZ_AppProcessHelper.getSingleton().getCurrentGame() != null)
            {
                setGameIsPlaying(true);
            }
            else
            {
                setGameIsPlaying(false);
            }
        }

        private void btClose_Click(object sender, RoutedEventArgs e)
        {
            if (gameVideo != null)
            {
                gameVideo.Stop();
                gameVideo.Dispose();
                gameVideo = null;
            }
            if (callback != null)
            {
                callback.onClose(this);
            }
        }

        public void setGameIsPlaying(bool isPlaying)
        {
            if(VZ_AppProcessHelper.getSingleton().getCurrentGame() != gameInfo)
            {
                isGameInPlaying = false;
            }
            else
            {
                isGameInPlaying = isPlaying;
            }
            Dispatcher.BeginInvoke((Action)(() =>
            {
                if (isPlaying)
                {
                    gameVideo.IsMute = true;
                }
                else
                {
                    gameVideo.IsMute = false;
                }
                if(isGameInPlaying)
                {
                    runBt.Content = "退出游戏";
                    runBt.Background = new ImageBrush(new BitmapImage(new Uri(@"pack://application:,,,/Res/bt_play_click.png", UriKind.RelativeOrAbsolute)));
                }
                else
                {
                    runBt.Content = "点我开始游戏";
                    runBt.Background = new ImageBrush(new BitmapImage(new Uri(@"pack://application:,,,/Res/bt_play.png", UriKind.RelativeOrAbsolute)));
                }
            }));
        }

        private void runBt_Click(object sender, RoutedEventArgs e)
        {
            if (callback != null)
            {
                if(isGameInPlaying)
                {
                    callback.onQuit(this, gameInfo);
                }
                else
                {
                    callback.onPlay(this, gameInfo);
                }
            }
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            if (gameVideo != null)
            {
                gameVideo.Stop();
                gameVideo.Dispose();
                gameVideo = null;
            }
        }
    }
}
