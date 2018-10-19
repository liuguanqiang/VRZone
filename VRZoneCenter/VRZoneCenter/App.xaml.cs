using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using VRZoneCenter.Classes.Utils;

namespace VRZoneCenter
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        private void Application_Exit(object sender, ExitEventArgs e)
        {
            VZ_AppProcessHelper.getSingleton().closeAllApp();
            VZ_AppProcessHelper.getSingleton().closeTopVideo();
        }
    }
}
