using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using VRZone.Log;

namespace Scratch.Log
{
    public class LogConfig
    {
        /// <summary> 日志输出接口 </summary>
        public static ILog Logger { get; private set; }

        /// <summary> Output输出,在界面中显示,并同时输出到日志中 </summary>
        public static ILog Output { get; private set; }

        /// <summary> 静态构造函数 </summary>
        static LogConfig()
        {
            Logger = new Logger(false);
            Output = new Logger(true);
        }
    }
}
