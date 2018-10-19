using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using VRZoneLib.Classes.Units;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Threading;
using VRZoneCenter.Classes.Forms;
using System.Windows.Threading;
using VRZoneLib.Classes.Utils;

namespace VRZoneCenter.Classes.Utils
{
    class VZ_AppProcessHelper
    {
        List<Process> proList = null;
        VZ_TopPlayerWindow topVideoWindow;
        static VZ_AppInfo currentInfo;
        static long currentTime;
        static System.Timers.Timer timer;

        private static VZ_AppProcessHelper singleton = null;
        public static VZ_AppProcessHelper getSingleton()
        {
            if(singleton == null)
            {
                singleton = new VZ_AppProcessHelper();
            }
            return singleton;
        }

        private VZ_AppProcessHelper()
        {
            proList = new List<Process>();
        }

        public VZ_AppInfo getCurrentGame()
        {
            return currentInfo;
        }


        public Screen getMainScreen()
        {
            for (int i = 0; i < Screen.AllScreens.Length; i++)
            {
                Screen scr = Screen.AllScreens[i];
                if (scr.Bounds.X == 0 && scr.Bounds.Y == 0)
                {
                    return scr;
                }
            }
            return Screen.AllScreens[0];
        }

        public Screen getSecondScreen()
        {
            for (int i = 0; i < Screen.AllScreens.Length;i++)
            {
                Screen scr = Screen.AllScreens[i];
                if((scr.Bounds.X != 0 || scr.Bounds.Y != 0) && !(scr.Bounds.X == 0 && scr.Bounds.Y == 0))
                {
                    return scr;
                }
            }
            return Screen.AllScreens[0];
        }

        public void muteTopVideo(bool isMuted)
        {
            try
            {
                if (topVideoWindow != null)
                {
                    topVideoWindow.setMuted(isMuted);
                }
            }
            catch
            {

            }
        }

        public void runTopVideo()
        {
            try
            {
                if (topVideoWindow == null)
                {
                    topVideoWindow = new VZ_TopPlayerWindow();
                    topVideoWindow.Show();
                }
                topVideoWindow.Visibility = System.Windows.Visibility.Visible;
                topVideoWindow.play();
            }
            catch
            {
                topVideoWindow = null;
            }
        }

        public void closeTopVideo()
        {
            try
            {
                if (topVideoWindow != null)
                {
                    topVideoWindow.Close();
                    topVideoWindow = null;
                }
            }
            catch
            {

            }
        }

        public void hideTopVideo()
        {
            try
            {
                if (topVideoWindow != null)
                {
                    topVideoWindow.pause();
                    topVideoWindow.Visibility = System.Windows.Visibility.Hidden;
                }
            }
            catch
            {

            }
        }

        public void runApp(VZ_AppInfo info)
        {
            try
            {
                if (currentInfo != null)
                {
                    if (currentInfo == info)
                    {
                        return;
                    }
                }
                closeAllApp();
                Process proc = new Process();
                proc.StartInfo.FileName = info.appPath;
                proc.Start();
                currentInfo = info;
                currentTime = DateTime.Now.Ticks;
                AsyncRunApp2(info, proc);
            }
            catch
            {

            }
        }
        

        private void AsyncRunApp2(VZ_AppInfo info,Process proc)
        {
            if(!info.appId.Equals("9"))
            {
                proc.WaitForInputIdle();
            }
            Thread.Sleep(1000);
            System.Drawing.Rectangle monitor;
            if (info.monitorNO == 1)
            {
                monitor = VZ_AppProcessHelper.getSingleton().getMainScreen().WorkingArea;
            }
            else
            {
                monitor = VZ_AppProcessHelper.getSingleton().getSecondScreen().WorkingArea;
            }
            //monitor = VZ_AppProcessHelper.getSingleton().getSecondScreen().WorkingArea;//Screen.AllScreens[sNumberOfMonitor].WorkingArea;
            //change the window to the second monitor
            SetWindowPos(proc.MainWindowHandle, 0,
            monitor.Left, monitor.Top, monitor.Width,
            monitor.Height, 0);
            proList.Add(proc);
            currentInfo = info;

            resetAppTimer();
        }

        private void resetAppTimer()
        {
            if (timer != null)
            {
                timer.Stop();
                timer.Close();
                timer = null;
            }
            timer = new System.Timers.Timer(5000);
            timer.AutoReset = true;
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
        }

        private void closeAppTimer()
        {
            if (timer != null)
            {
                timer.Stop();
                timer.Close();
                timer = null;
            }
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if(currentInfo == null || proList.Count == 0)
            {
                closeAppTimer();
                return;
            }
            System.Drawing.Rectangle monitor;
            if (currentInfo.monitorNO == 1)
            {
                monitor = VZ_AppProcessHelper.getSingleton().getMainScreen().WorkingArea;
            }
            else
            {
                monitor = VZ_AppProcessHelper.getSingleton().getSecondScreen().WorkingArea;
            }
            //change the window to the second monitor
            SetWindowPos(proList[0].MainWindowHandle, 0,
            monitor.Left, monitor.Top, monitor.Width,
            monitor.Height, 0);
        }

        public void closeAllApp()
        {
            try
            {
                if(currentInfo != null)
                {
                    currentTime = DateTime.Now.Ticks - currentTime;
                    currentTime = currentTime / 10000000;
                    VZ_AppHelper.GetInstance().sendAppLog(currentInfo, (int)currentTime);
                }
                currentInfo = null;
                closeAppTimer();
                foreach (Process proc in proList)
                {
                    if (!proc.HasExited)
                    {
                        proc.Kill();
                    }
                }

                Process[] proArray = Process.GetProcesses();
                foreach (Process proc in proArray)
                {
                    foreach (VZ_AppInfo app in VZ_AppHelper.GetInstance().appList)
                    {
                        if (app.appExeName.Equals(proc.ProcessName))
                        {
                            proc.Kill();
                            break;
                        }
                    }
                }

                proList.Clear();
            }
            catch
            {

            }
        }

        public void checkRunStateApp()
        {
            try
            {
                if (proList == null) return;
                int count = proList.Count;
                for (int i = 0; ;)
                {
                    if (i >= proList.Count) break;
                    Process proc = proList[i];
                    if (proc.HasExited)
                    {
                        proList.Remove(proc);
                    }
                    else
                    {
                        i++;
                    }
                }
                if (proList.Count == 0)
                {
                    currentInfo = null;
                }
                else
                {
                    foreach (VZ_AppInfo info in VZ_AppHelper.GetInstance().appList)
                    {
                        if (proList[0].ProcessName == info.appExeName)
                        {
                            currentInfo = info;
                            break;
                        }
                    }
                    if (proList.Count > 1)
                    {
                        for (int i = 1; i < proList.Count; i++)
                        {
                            proList[i].Kill();
                        }
                    }
                }
            }
            catch
            {

            }
        }


        public void quitSteamVRHome()
        {
            try
            {
                Process[] proArray = Process.GetProcesses();
                foreach (Process proc in proArray)
                {
                    if (proc.ProcessName.StartsWith("steamtours"))
                    {
                        proc.Kill();
                        break;
                    }
                }
            }
            catch
            {

            }
        }


        #region 启动其他app代码
        /// <summary>
        /// Functions por set the position of a window
        /// </summary>
        [DllImport("user32")]
        public static extern long SetWindowPos(IntPtr hwnd, int hWndInsertAfter, int X, int y, int cx, int cy, int wFlagslong);
        const short SWP_NOSIZE = 0x0001;
        const short SWP_NOMOVE = 0x0002;
        const int SWP_NOZORDER = 0x0004;
        const int SWP_SHOWWINDOW = 0x0040;

        [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
        public static extern int GetWindowLong(
            IntPtr hwnd,
            int nIndex
        );

        const int WS_THICKFRAME = 0x00040000;
        const int GWL_STYLE = -16;

        [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
        public static extern int SetWindowLong(
            IntPtr hwnd,
            int nIndex,
            int dwNewLong
        );

        #endregion
    }
}
