using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using VRZoneCenter.Classes.Utils;
using VRZoneLib.Classes.Units;
using VRZoneCenter.Classes.Components;
using System.Timers;
using MahApps.Metro.Controls;
using VRZoneCenter.Classes.Forms;
using System.Media;
using System.Collections;
using VRZoneLib.Classes.Utils;

namespace VRZoneCenter
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window, VZ_GameInfoUC.VZ_GameInfoUCCallback
    {
        private int pageIndex = 0;
        private int pageTotal = 0;
        VZ_PagePoint vpp;

        List<Grid> gameGrid = new List<Grid>();
        Grid grid3;
        Grid gridReady;
        Button quitBt;

        //Image headerImg;
        //TextBlock tbName;
        TextBlock tbGameTime;
        TextBlock tbTime;

        Image playGameImg;
        TextBlock tbState;
        TextBlock tbGame;

        Image gameTopImg;

        Button btLeft;
        Button btRight;

        Hashtable bmpHash = new Hashtable();

        System.Timers.Timer secondTimer;

        VZ_GameInfoUC gameInfoUC;

        public MainWindow()
        {
            InitializeComponent();
            VZ_AppHelper.GetInstance();

            pageTotal = VZ_AppHelper.GetInstance().appList.Count / 3;
            if(VZ_AppHelper.GetInstance().appList.Count % 3 > 0)
            {
                pageTotal++;
            }

            /*主界面4个行
             * 第一行，控制栏
             * 第二行，用户信息
             * 第三行，应用选择菜单
             * 第四行，留空 
             */
            RowDefinition row1 = new RowDefinition();
            row1.Height = new GridLength(126);
            RowDefinition row2 = new RowDefinition();
            row2.Height = new GridLength(100);
            RowDefinition row3 = new RowDefinition();
            row3.Height = new GridLength(1, GridUnitType.Star);
            RowDefinition row4 = new RowDefinition();
            row4.Height = new GridLength(80);
            RowDefinition row5 = new RowDefinition();
            row5.Height = new GridLength(80);
            mainGrid.RowDefinitions.Add(row1);
            mainGrid.RowDefinitions.Add(row2);
            mainGrid.RowDefinitions.Add(row3);
            mainGrid.RowDefinitions.Add(row4);
            mainGrid.RowDefinitions.Add(row5);

            //第一行Grid
            //图标、游戏名字、退出游戏、音量组件
            Grid grid1 = new Grid();
            Grid.SetRow(grid1, 0);
            //grid1.Background = new SolidColorBrush(Color.FromArgb(128,93, 195,255));

            Grid grid10 = new Grid();
            Grid.SetRow(grid10, 0);
            grid10.Margin = new Thickness(0, 0, 166, 0);
            mainGrid.Children.Add(grid10);
            gameTopImg = new Image();
            grid10.Children.Add(gameTopImg);
            gameTopImg.Stretch = Stretch.UniformToFill;

            Image topImage = VZMethods.getImageContent(@"Res\top_bar.png");
            topImage.Stretch = Stretch.Fill;
            grid10.Children.Add(topImage);


            mainGrid.Children.Add(grid1);

            ColumnDefinition col11 = new ColumnDefinition();
            ColumnDefinition col12 = new ColumnDefinition();
            ColumnDefinition col13 = new ColumnDefinition();
            ColumnDefinition col14 = new ColumnDefinition();
            ColumnDefinition col15 = new ColumnDefinition();
            col11.Width = new GridLength(155, GridUnitType.Pixel);
            col12.Width = new GridLength(526, GridUnitType.Pixel);
            col13.Width = new GridLength(1, GridUnitType.Star);
            col14.Width = new GridLength(410, GridUnitType.Pixel);
            col15.Width = new GridLength(166, GridUnitType.Pixel);
            grid1.ColumnDefinitions.Add(col11);
            grid1.ColumnDefinitions.Add(col12);
            grid1.ColumnDefinitions.Add(col13);
            grid1.ColumnDefinitions.Add(col14);
            grid1.ColumnDefinitions.Add(col15);

            //gameTopImg = new Image();
            //grid1.Children.Add(gameTopImg);
            //Grid.SetColumnSpan(gameTopImg, 4);
            //gameTopImg.Stretch = Stretch.UniformToFill;

            //Image topImage = VZMethods.getImageContent(@"Res\top_bar.png");
            //topImage.Stretch = Stretch.Fill;
            //Grid.SetColumnSpan(topImage, 4);
            //grid1.Children.Add(topImage);

            Grid grid11 = new Grid();
            Grid.SetColumn(grid11, 0);
            grid11.Width = 144;
            grid11.Height = 114;
            grid11.VerticalAlignment = VerticalAlignment.Center;
            grid11.HorizontalAlignment = HorizontalAlignment.Center;
            grid1.Children.Add(grid11);

            playGameImg = new Image();
            playGameImg.Stretch = Stretch.UniformToFill;
            grid11.Children.Add(playGameImg);
            playGameImg.Width = 144;
            playGameImg.Height = 114;
            playGameImg.Stretch = Stretch.UniformToFill;
            playGameImg.Margin = new Thickness(7.5);
            playGameImg.HorizontalAlignment = HorizontalAlignment.Center;
            playGameImg.VerticalAlignment = VerticalAlignment.Center;

            Image img = VZMethods.getImageContent(@"Res\game_border.png");
            grid11.Children.Add(img);
            img.HorizontalAlignment = HorizontalAlignment.Center;
            img.VerticalAlignment = VerticalAlignment.Center;

            tbState = new TextBlock();
            tbState.Text = "准备就绪";
            tbState.Foreground = new SolidColorBrush(Colors.White);
            tbState.FontSize = 20;
            tbState.TextAlignment = TextAlignment.Center;
            tbState.HorizontalAlignment = HorizontalAlignment.Center;
            tbState.VerticalAlignment = VerticalAlignment.Center;
            grid11.Children.Add(tbState);

            tbGame = new TextBlock();
            tbGame.TextAlignment = TextAlignment.Left;
            tbGame.HorizontalAlignment = HorizontalAlignment.Left;
            tbGame.VerticalAlignment = VerticalAlignment.Center;
            tbGame.FontSize = 40;
            tbGame.Foreground = new SolidColorBrush(Colors.White);
            tbGame.Margin = new Thickness(20, 0, 0, 0);
            Grid.SetColumn(tbGame, 1);
            grid1.Children.Add(tbGame);

            VZ_VolumeController volumeCtr = new VZ_VolumeController();
            Grid.SetColumn(volumeCtr, 3);
            grid1.Children.Add(volumeCtr);

            quitBt = VZMethods.getNoBgImageButton(VZMethods.getImageContent(@"Res\quit_game.png"));
            Grid.SetColumn(quitBt, 1);
            quitBt.Height = 50;
            quitBt.Visibility = Visibility.Hidden;
            quitBt.VerticalAlignment = VerticalAlignment.Center;
            quitBt.HorizontalAlignment = HorizontalAlignment.Right;
            quitBt.Click += QuitBt_Click;
            grid1.Children.Add(quitBt);
            quitBt.Margin = new Thickness(0, 0, 50, 0);

            Image tmpImg = VZMethods.getImageContent(@"Res\bt_help3.png");
            tmpImg.Stretch = Stretch.Fill;
            tmpImg.Width = 166;
            tmpImg.Height = 126;

            Tile tile = VZMethods.getNoBgImageButton(tmpImg);
            tile.Click += Tile_Click;
            tile.VerticalAlignment = VerticalAlignment.Center;
            tile.HorizontalAlignment = HorizontalAlignment.Center;
            tile.Width = 166;
            tile.Height = 126;
            tile.Margin = new Thickness(-10);
            Grid.SetColumn(tile, 4);
            grid1.Children.Add(tile);

            Grid grid12 = new Grid();
            grid12.Margin = new Thickness(0, 20, 50, 20);
            grid12.Background = new ImageBrush(VZMethods.getBitmap("pack://application:,,,/Res/time_bg.png"));// (@"/Res/time_bg.png"));
            Grid.SetColumn(grid12, 2);
            grid12.HorizontalAlignment = HorizontalAlignment.Center;
            grid1.Children.Add(grid12);

            StackPanel sp = new StackPanel();
            sp.Margin = new Thickness(20,10,40,10);
            grid12.Children.Add(sp);
            sp.HorizontalAlignment = HorizontalAlignment.Center;
            sp.Orientation = Orientation.Horizontal;

            TextBlock tb = new TextBlock();
            tb.Text = "倒计时";
            tb.Margin = new Thickness(0, 0, 30, 0);
            tb.FontSize = 20;
            tb.HorizontalAlignment = HorizontalAlignment.Right;
            tb.Foreground = new SolidColorBrush(Colors.White);
            tb.VerticalAlignment = VerticalAlignment.Center;
            sp.Children.Add(tb);

            tbGameTime = new TextBlock();
            tbGameTime.Text = "00 : 00 : 00";
            tbGameTime.FontSize = 50;
            tbGameTime.HorizontalAlignment = HorizontalAlignment.Left;
            tbGameTime.Foreground = new SolidColorBrush(Colors.White);
            tbGameTime.VerticalAlignment = VerticalAlignment.Center;
            sp.Children.Add(tbGameTime);

            Button payBt = VZMethods.getNoBgImageButton(VZMethods.getImageContent(@"Res\bt_payagain.png"));
            payBt.Height = 60;
            payBt.Margin = new Thickness(30, 0, 0, 0);
            sp.Children.Add(payBt);
            payBt.Click += PayBt_Click;

            //第二行Grid
            //用户头像、用户名、剩余时间、时间
            Grid grid2 = new Grid();
            grid2.Margin = new Thickness(160, 0, 160, 0);
            Grid.SetRow(grid2, 1);
            mainGrid.Children.Add(grid2);

            ColumnDefinition col21 = new ColumnDefinition();
            ColumnDefinition col22 = new ColumnDefinition();
            ColumnDefinition col23 = new ColumnDefinition();
            ColumnDefinition col24 = new ColumnDefinition();
            col21.Width = new GridLength(100, GridUnitType.Pixel);
            col22.Width = new GridLength(300, GridUnitType.Pixel);
            col23.Width = new GridLength(1, GridUnitType.Star);
            col24.Width = new GridLength(400, GridUnitType.Pixel);
            grid2.ColumnDefinitions.Add(col21);
            grid2.ColumnDefinitions.Add(col22);
            grid2.ColumnDefinitions.Add(col23);
            grid2.ColumnDefinitions.Add(col24);

            //headerImg = new Image();
            //headerImg.Margin = new Thickness(10);
            //Grid.SetColumn(headerImg, 0);
            //VZMethods.getNetImage(headerImg, VZ_AppHelper.getSingleton().payInfo.headimgurl);
            //grid2.Children.Add(headerImg);

            //tbName = new TextBlock();
            //tbName.Text = VZ_AppHelper.getSingleton().payInfo.nickname;
            //tbName.FontSize = 25;
            //tbName.Margin = new Thickness(0, 0, 0, 30);
            //tbName.Foreground = new SolidColorBrush(Colors.LightBlue);
            //tbName.VerticalAlignment = VerticalAlignment.Center;
            //Grid.SetColumn(tbName, 1);
            //grid2.Children.Add(tbName);

            //if(VZ_AppHelper.getSingleton().systemInfo.isDebug)
            //{
            //    tbName.Text = VZ_AppHelper.getSingleton().payInfo.nickname + " **调试模式**";
            //}

            //TextBlock tb = new TextBlock();
            //tb.Text = "微信已登录";
            //tb.FontSize = 12.5;
            //tb.Margin = new Thickness(0, 30, 0, 0);
            //tb.Foreground = new SolidColorBrush(Colors.LightBlue);
            //tb.VerticalAlignment = VerticalAlignment.Center;
            //Grid.SetColumn(tb, 1);
            //grid2.Children.Add(tb);

            tbTime = new TextBlock();
            tbTime.Text = "00:00";
            tbTime.HorizontalAlignment = HorizontalAlignment.Right;
            tbTime.FontSize = 20;
            tbTime.Foreground = new SolidColorBrush(Colors.LightBlue);
            tbTime.VerticalAlignment = VerticalAlignment.Center;
            Grid.SetColumn(tbTime, 3);
            grid2.Children.Add(tbTime);

            //第三行Grid
            //←、游戏1、游戏2、游戏3、→
            gridReady = new Grid();
            Grid.SetRow(gridReady, 2);
            Image readyImg = VZMethods.getImageContent(@"Res\main_ready.png");
            gridReady.Children.Add(readyImg);
            Button readyBt = VZMethods.getNoBgImageButton(VZMethods.getImageContent(@"Res\bt_ok3.png"));
            readyBt.Width = 405;
            readyBt.Height = 90;
            readyBt.HorizontalAlignment = HorizontalAlignment.Center;
            readyBt.HorizontalContentAlignment = HorizontalAlignment.Center;
            readyBt.VerticalContentAlignment = VerticalAlignment.Center;
            readyBt.VerticalAlignment = VerticalAlignment.Bottom;
            readyBt.Margin = new Thickness(0, 0, 0, -45);
            readyBt.Click += ReadyBt_Click;
            gridReady.Children.Add(readyBt);
            mainGrid.Children.Add(gridReady);

            grid3 = new Grid();
            grid3.Visibility = Visibility.Hidden;
            
            Grid.SetRow(grid3, 2);
            mainGrid.Children.Add(grid3);
            ColumnDefinition col31 = new ColumnDefinition();
            ColumnDefinition col32 = new ColumnDefinition();
            ColumnDefinition col33 = new ColumnDefinition();
            ColumnDefinition col34 = new ColumnDefinition();
            ColumnDefinition col35 = new ColumnDefinition();
            col31.Width = new GridLength(160);
            col32.Width = new GridLength(1, GridUnitType.Star);
            col33.Width = new GridLength(1, GridUnitType.Star);
            col34.Width = new GridLength(1, GridUnitType.Star);
            col35.Width = new GridLength(160);
            grid3.ColumnDefinitions.Add(col31);
            grid3.ColumnDefinitions.Add(col32);
            grid3.ColumnDefinitions.Add(col33);
            grid3.ColumnDefinitions.Add(col34);
            grid3.ColumnDefinitions.Add(col35);

            btLeft = VZMethods.getNoBgImageButton(VZMethods.getImageContent(@"Res\left_disable.png"));
            btLeft.Height = 100;
            Grid.SetColumn(btLeft, 0);
            grid3.Children.Add(btLeft);
            btLeft.VerticalAlignment = VerticalAlignment.Center;
            btLeft.HorizontalAlignment = HorizontalAlignment.Center;
            btLeft.Click += BtLeft_Click;

            btRight = VZMethods.getNoBgImageButton(VZMethods.getImageContent(@"Res\right_disable.png"));
            btRight.Height = 100;
            Grid.SetColumn(btRight, 4);
            grid3.Children.Add(btRight);
            btRight.VerticalAlignment = VerticalAlignment.Center;
            btRight.HorizontalAlignment = HorizontalAlignment.Center;
            btRight.Click += BtRight_Click;

            //增加app娱乐项目

            int counter = 0;
            foreach(VZ_AppInfo info in VZ_AppHelper.GetInstance().appList)
            {
                Grid grid = new Grid();
                grid.Margin = new Thickness(0,40,0,40);
                Button bt = VZMethods.getNoBgImageButton(VZMethods.getImageContentByAbsolutePath(info.appImgPath));
                bt.Margin = new Thickness(0);
                bt.Tag = counter++;
                bt.Click += Bt_Click;
                bt.ClickMode = ClickMode.Release;
                grid.Children.Add(bt);

                if (info.type.Equals("game"))
                {
                    img = VZMethods.getImageContent(@"Res\game_bottom_bar.png");
                    img.VerticalAlignment = VerticalAlignment.Bottom;
                    img.Margin = new Thickness(20, 0, 20, 0);
                    grid.Children.Add(img);

                    img = VZMethods.getImageContent(@"Res\game_tag.png");
                    img.VerticalAlignment = VerticalAlignment.Top;
                    img.Width = 80;
                    img.Height = 48;
                    img.HorizontalAlignment = HorizontalAlignment.Right;
                    img.Margin = new Thickness(0, 30, 60, 0);
                    grid.Children.Add(img);

                    StackPanel tmpSp = new StackPanel();
                    tmpSp.Orientation = Orientation.Horizontal;
                    tmpSp.Height = 80;
                    tmpSp.Background = new SolidColorBrush(Colors.Transparent);
                    tmpSp.VerticalAlignment = VerticalAlignment.Bottom; 
                    grid.Children.Add(tmpSp);

                    tb = new TextBlock();
                    tb.VerticalAlignment = VerticalAlignment.Bottom;
                    tb.Margin = new Thickness(60, 0, 20, 20);
                    tb.Foreground = new SolidColorBrush(Colors.White);
                    tb.Opacity = 0.8;
                    tb.FontSize = 25;
                    tb.Text = info.appName;
                    tmpSp.Children.Add(tb);

                    for (int i = 0; i < info.tagList.Count; i++)
                    {
                        Grid tagGrid = new Grid();
                        tagGrid.Width = 60;
                        tagGrid.Height = 34;
                        tagGrid.Margin = new Thickness(0, 13, 10, 0);
                        tagGrid.Background = new ImageBrush(VZMethods.getBitmap(@"pack://application:,,,/Res/tag_bg.png"));
                        TextBlock tb2 = new TextBlock();
                        tb2.Foreground = new SolidColorBrush(Color.FromArgb(150,255,255,255));
                        tb2.VerticalAlignment = VerticalAlignment.Center;
                        tb2.HorizontalAlignment = HorizontalAlignment.Center;
                        tb2.FontSize = 15;
                        tb2.Text = info.tagList[i];
                        tagGrid.Children.Add(tb2);

                        tmpSp.Children.Add(tagGrid);
                    }

                    grid.Tag = info;
                }

                img = VZMethods.getImageContent(@"Res\gaming.png");
                img.VerticalAlignment = VerticalAlignment.Top;
                img.Width = 139;
                img.Height = 48;
                img.HorizontalAlignment = HorizontalAlignment.Right;
                img.Stretch = Stretch.None;
                img.Margin = new Thickness(0, 30, 60, 0);
                img.Name = "img_cover";
                img.Visibility = Visibility.Hidden;
                grid.Children.Add(img);
                gameGrid.Add(grid);
            }
            //第四行 增加页码
            vpp = new VZ_PagePoint();
            Grid.SetRow(vpp, 3);
            vpp.Visibility = Visibility.Hidden;
            mainGrid.Children.Add(vpp);
            showPage(pageIndex);

            secondTimer = new System.Timers.Timer(1000);
            secondTimer.AutoReset = true;
            secondTimer.Elapsed += SecondTimer_Elapsed;
            secondTimer.Start();

            this.Activate();
        }

        private void ReadyBt_Click(object sender, RoutedEventArgs e)
        {
            gridReady.Visibility = Visibility.Hidden;
            grid3.Visibility = Visibility.Visible;
            vpp.Visibility = Visibility.Visible;
            Timer helperTimer = new Timer(500);
            helperTimer.AutoReset = false;
            helperTimer.Elapsed += HelperTimer_Elapsed;
            helperTimer.Start();
        }

        private void HelperTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Dispatcher.BeginInvoke((Action)(() =>
            {
                showHelp();
            }));
        }

        public void showHelp()
        {
            VZ_MainHelpWindow win = new VZ_MainHelpWindow();
            win.Show();
        }

        private void Tile_Click(object sender, RoutedEventArgs e)
        {
            VZ_ZoneHelpWindow win = new VZ_ZoneHelpWindow();
            win.Show();
        }

        private void PayBt_Click(object sender, RoutedEventArgs e)
        {
            showPayWin();
        }

        private void BtLeft_Click(object sender, RoutedEventArgs e)
        {
            pageIndex--;
            showPage(pageIndex);
        }

        private void BtRight_Click(object sender, RoutedEventArgs e)
        {
            pageIndex++;
            showPage(pageIndex);
        }


        private void SecondTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Dispatcher.BeginInvoke((Action)(() =>
            {
                tbTime.Text = string.Format("{0:t}", DateTime.Now);
                tbGameTime.Text = getRemainTime();
            }));

            if ((VZ_AppHelper.GetInstance().payInfo.expire < VZ_AppHelper.GetInstance().payInfo.current_time) && !(VZ_AppHelper.GetInstance().systemInfo.isDebug))
            {
                Dispatcher.BeginInvoke((Action)(() =>
                {
                    secondTimer.Stop();
                    secondTimer = null;
                    VZ_AppProcessHelper.getSingleton().closeAllApp();
                    EnterV3Window win = new EnterV3Window();
                    win.Show();
                    VZ_FinishV2Window win2 = new VZ_FinishV2Window();
                    win2.Show();
                    //EnterV2Window win = new EnterV2Window();
                    //win.Show();
                    //VZ_FinishWindow win2 = new VZ_FinishWindow();
                    //win2.Show();
                    this.Close();
                }));
            }

            VZ_AppProcessHelper.getSingleton().checkRunStateApp();

            VZ_AppInfo gameInfo = VZ_AppProcessHelper.getSingleton().getCurrentGame();

            Dispatcher.BeginInvoke((Action)(() =>
            {
                if(gameInfoUC != null)
                {
                    VZ_AppProcessHelper.getSingleton().muteTopVideo(true);
                }

                if (gameInfo == null)
                {
                    playGameImg.Source = null;
                    tbState.Text = "准备就绪";
                    quitBt.Visibility = Visibility.Hidden;
                    gameTopImg.Source = null;
                    tbGame.Text = "";

                    foreach(Grid grid in gameGrid)
                    {
                        Image img = grid.FindChild<Image>("img_cover");
                        if(img != null)
                        {
                            img.Visibility = Visibility.Hidden;
                        }
                    }
                    VZ_AppProcessHelper.getSingleton().runTopVideo();
                }
                else
                {
                    BitmapSource bmp = getGameImg(gameInfo.appImgPath);
                    playGameImg.Source = bmp;
                    tbState.Text = "游戏中";
                    gameTopImg.Source = getGameCropImg(gameInfo.appImgPath);
                    quitBt.Visibility = Visibility.Visible;
                    tbGame.Text = gameInfo.appName;

                    foreach(Grid grid in gameGrid)
                    {
                        Image img = grid.FindChild<Image>("img_cover");
                        if (grid.Tag == gameInfo)
                        {
                            if (img != null)
                            {
                                img.Visibility = Visibility.Visible;
                            }
                        }
                        else
                        {
                            if (img != null)
                            {
                                img.Visibility = Visibility.Hidden;
                            }
                        }
                    }
                    VZ_AppProcessHelper.getSingleton().hideTopVideo();
                }
            }));
        }

        private BitmapSource GetPartImage(BitmapSource bmp)
        {
            return new CroppedBitmap(BitmapFrame.Create(bmp), new Int32Rect(0, (int)bmp.Height / 2, (int)bmp.Width, (int)bmp.Height / 3));
        }

        BitmapSource getGameImg(String path)
        {
            if(bmpHash.Contains(path))
            {
                return (BitmapSource)bmpHash[path];
            }
            else
            {
                BitmapImage bmp = new BitmapImage(new Uri(path, UriKind.Absolute));
                bmpHash[path] = bmp;
                return bmp;
            }
        }

        BitmapSource getGameCropImg(String path)
        {
            String key = path + "crop";
            if(bmpHash.Contains(key))
            {
                return (BitmapSource)bmpHash[key];
            }
            else
            {
                BitmapSource bmp = getGameImg(path);
                BitmapSource bs = GetPartImage(bmp);
                bmpHash[key] = bs;
                return bs;
            }
        }


        long lastCurrent = 0;
        long bet = 0;

        private String getRemainTime()
        {
            long expire = VZ_AppHelper.GetInstance().payInfo.expire;
            long current = VZ_AppHelper.GetInstance().payInfo.current_time;
            if(bet == 0)
            {
                bet = expire - current;
                lastCurrent = current;
            }
            if(lastCurrent == current)
            {
                bet--;
            }
            else
            {
                bet = expire - current;
                lastCurrent = current;
            }
            if(bet == 30)
            {
                Dispatcher.BeginInvoke((Action)(() =>
                {
                    String voice = AppDomain.CurrentDomain.BaseDirectory + @"\Res\payagain.wav";
                    SoundPlayer player = new SoundPlayer(voice);
                    player.Play();

                    //showPayWin();
                }));
            }
            String str = String.Format("{0:00} : {1:00} : {2:00}", bet/3600, bet %3600 / 60, bet % 60);
            return str;
        }

        void showPayWin()
        {
            //if (VZ_AppHelper.getSingleton().payInfo.priceArr != null && VZ_AppHelper.getSingleton().payInfo.priceArr.Length >= 2)
            //{
            //    VZ_PayDoubleWindow win = new VZ_PayDoubleWindow();
            //    win.Show();
            //}
            //else
            //{
            //    VZ_PayWindow win = new VZ_PayWindow();
            //    win.Show();
            //}
            VZ_PayWindow win = new VZ_PayWindow();
            win.Show();
        }


        protected override void OnClosed(EventArgs e)
        {
            if (secondTimer != null)
            {
                secondTimer.Stop();
                secondTimer = null;
            }
        }

        private void QuitBt_Click(object sender, RoutedEventArgs e)
        {
            VZ_AppProcessHelper.getSingleton().closeAllApp();
            if(gameInfoUC != null)
            {
                gameInfoUC.updateState();
            }
        }

        private void Bt_Click(object sender, RoutedEventArgs e)
        {
            int tag = (int)((Button)(sender)).Tag;
            VZ_AppInfo info = VZ_AppHelper.GetInstance().appList[tag];
            if(info.appId.Equals("1"))
            {
                showMovieOpeningWin();
                VZ_AppProcessHelper.getSingleton().runApp(info);
                //VZ_AppHelper.getSingleton().sendAppLog(info);
                return;
            }
            //VZ_AppProcessHelper.getSingleton().runApp(info);

            //VZ_AppHelper.getSingleton().sendAppLog(info);
            //if(info.type.Equals("game"))
            //{
            //    this.Activate();
            //}
            //showNoticeAync();

            if(gameInfoUC != null)
            {
                gameInfoUC = null;
            }
            Dispatcher.BeginInvoke((Action)(() =>
            {
                gameInfoUC = new VZ_GameInfoUC(info, this);
                Grid.SetRowSpan(gameInfoUC, 2);
                Grid.SetRow(gameInfoUC, 2);
                mainGrid.Children.Add(gameInfoUC);
                grid3.Visibility = Visibility.Hidden;
                vpp.Visibility = Visibility.Hidden;
                VZ_AppProcessHelper.getSingleton().muteTopVideo(true);
            }));
            
        }

        private void showNoticeAync()
        {
            VZ_StartAppWindow win = new VZ_StartAppWindow();
            win.Show();
        }

        private void showPage(int index)
        {
            int start = index * 3;
            if(start + 3 < VZ_AppHelper.GetInstance().appList.Count)
            {
                btRight.Content = VZMethods.getImageContent(@"Res\right_enable.png");
                btRight.IsEnabled = true;
            }
            else
            {
                btRight.Content = VZMethods.getImageContent(@"Res\right_disable.png");
                btRight.IsEnabled = false;
            }
            if(start > 0)
            {
                btLeft.Content = VZMethods.getImageContent(@"Res\left_enable.png");
                btLeft.IsEnabled = true;
            }
            else
            {
                btLeft.Content = VZMethods.getImageContent(@"Res\left_disable.png");
                btLeft.IsEnabled = false;
            }

            foreach (Grid grid in gameGrid)
            {
                if(grid3.Children.Contains(grid))
                {
                    grid3.Children.Remove(grid);
                }
            }
            for(int i = 0; i < 3;i++)
            {
                if(start + i >= gameGrid.Count)
                {
                    break;
                }
                Grid grid = gameGrid[start + i];
                Grid.SetColumn(grid, i + 1);
                grid3.Children.Add(grid);
            }
            vpp.setState(index, pageTotal);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            VZ_AppProcessHelper.getSingleton().closeTopVideo();
        }

        void makePosition()
        {
            System.Windows.Forms.Screen s = VZ_AppProcessHelper.getSingleton().getMainScreen();

            System.Drawing.Rectangle r = s.WorkingArea;
            this.Top = r.Top;
            this.Left = r.Left;
            this.Width = r.Width;
            this.Height = r.Height;

            this.WindowStartupLocation = WindowStartupLocation.Manual;
            //this.Top = 0;
            //this.Left = System.Windows.SystemParameters.MaximizedPrimaryScreenWidth;
            this.WindowState = System.Windows.WindowState.Normal;
            this.WindowStyle = System.Windows.WindowStyle.None;
            this.WindowState = System.Windows.WindowState.Maximized;
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            makePosition();
        }

        public void onClose(VZ_GameInfoUC view)
        {
            if (mainGrid.Children.Contains(view))
            {
                mainGrid.Children.Remove(view);
                gameInfoUC = null;
            }
            grid3.Visibility = Visibility.Visible;
            vpp.Visibility = Visibility.Visible;
            VZ_AppProcessHelper.getSingleton().muteTopVideo(false);
        }

        public void onPlay(VZ_GameInfoUC view, VZ_AppInfo info)
        {
            showGameOpeningWin();
            VZ_AppProcessHelper.getSingleton().runApp(info);

            if (info.type.Equals("game") && !info.appExeName.Equals("DrumGame.exe"))
            {
                this.Activate();
            }
            view.setGameIsPlaying(true);
        }

        public void onQuit(VZ_GameInfoUC view, VZ_AppInfo info)
        {
            VZ_AppProcessHelper.getSingleton().closeAllApp();
            view.setGameIsPlaying(false);
        }

        private void showGameOpeningWin()
        {
            VZ_StartAppWindow win = new VZ_StartAppWindow();
            win.Show();
        }

        private void showMovieOpeningWin()
        {
            VZ_StartMovieWindow win = new VZ_StartMovieWindow();
            win.Show();
        }
    }
}
