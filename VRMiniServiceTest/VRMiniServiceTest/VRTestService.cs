using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using Cjwdev.WindowsApi;
using System.Runtime.InteropServices;

namespace VRMiniServiceTest
{
    public partial class VRTestService : ServiceBase
    {
        //private const String appPath = @"C:\Program Files\GoPro\GoPro VR Player 2.0\";
        //private const String appName = "GoProVRPlayer_x64.exe";
        //private const String appOnlyName = "GoProVRPlayer_x64";
        private const String appPath = @"C:\Program Files (x86)\VRKongFu\VRZoneCenter\";
        private const String appName = "VRZoneCenter.exe";
        private const String appOnlyName = "VRZoneCenter";

        [DllImport("user32.dll", SetLastError = true)]
        static extern void SwitchToThisWindow(IntPtr hWnd, bool fAltTab);

        public VRTestService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            //Debugger.Launch();

            //init();

            //Timer timer1 = new Timer(); timer1.Elapsed += new System.Timers.ElapsedEventHandler(timer1_Elapsed);
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = 3 * 1000;
            timer.AutoReset = true;
            timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
            timer.Start();
        }

        protected override void OnStop()
        {
        }

        private void init()
        {
            if (System.Diagnostics.Process.GetProcessesByName(appName).ToList().Count > 0)
            {
                //存在
                System.Diagnostics.Process.GetProcessesByName(appName);
            }
            else
            {
                //不存在
                Process ip = Process.Start(System.IO.Path.Combine(appPath, appName));
                ip.StartInfo.CreateNoWindow = false;
                ip.StartInfo.UseShellExecute = true;
                ip.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
            }
        }
        //InstallUtil VRMiniServiceTest.exe
        //installutil.exe /u VRMiniServiceTest.exe

        private void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (System.Diagnostics.Process.GetProcessesByName(appOnlyName).ToList().Count > 0)
            {
                //存在
                //System.Console.WriteLine("检测到" + appOnlyName);
                //System.Diagnostics.Process.GetProcessesByName(appOnlyName)[0].MainWindowHandle 
                //ShowWindowAsync(System.Diagnostics.Process.GetProcessesByName(appOnlyName)[0].MainWindowHandle, 1);//显示
                //SetForegroundWindow(System.Diagnostics.Process.GetProcessesByName(appOnlyName)[0].MainWindowHandle);//当到最前端
                //ApiDefinitions.ShowWindow(System.Diagnostics.Process.GetProcessesByName(appOnlyName)[0].MainWindowHandle, ApiDefinitions.);
                //bool isForeground = ApiDefinitions.SetWindowPos(System.Diagnostics.Process.GetProcessesByName(appOnlyName)[0].MainWindowHandle, IntPtr.Zero, 0, 0, 0, 0, ApiDefinitions.SWP_NOMOVE | ApiDefinitions.SWP_NOSIZE);
                IntPtr ptr = Process.GetProcessesByName(appOnlyName)[0].MainWindowHandle;
                SwitchToThisWindow(ptr, true);
               
            }
            else
            {
                //不存在
                //System.Console.WriteLine("启动"+ appOnlyName);
                AppStart(appPath + appName);
            }
        }

        public void AppStart(string appPath)
        {
            try
            {

                string appStartPath = appPath;
                IntPtr userTokenHandle = IntPtr.Zero;
                ApiDefinitions.WTSQueryUserToken(ApiDefinitions.WTSGetActiveConsoleSessionId(), ref userTokenHandle);

                ApiDefinitions.PROCESS_INFORMATION procInfo = new ApiDefinitions.PROCESS_INFORMATION();
                ApiDefinitions.STARTUPINFO startInfo = new ApiDefinitions.STARTUPINFO();
                startInfo.cb = (uint)Marshal.SizeOf(startInfo);
                

                ApiDefinitions.CreateProcessAsUser(
                    userTokenHandle,
                    appStartPath,
                    "",
                    IntPtr.Zero,
                    IntPtr.Zero,
                    false,
                    0,
                    IntPtr.Zero,
                    null,
                    ref startInfo,
                    out procInfo);

                if (userTokenHandle != IntPtr.Zero)
                {
                    ApiDefinitions.CloseHandle(userTokenHandle);
                }

                int _currentAquariusProcessId = (int)procInfo.dwProcessId;
            }
            catch (Exception ex)
            {
            }
        }
    }
}
