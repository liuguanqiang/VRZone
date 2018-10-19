using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRZoneLib.Classes.Units
{
    public class VZ_AppInfo
    {
        public String appId;
        public String appName;
        public String appExeName;
        public String appPath;
        public String appImgPath;
        public int monitorNO;
        public String type;
        public String moviePath { get; set; }
        public String bgPath;
        public List<String> tagList { get; set; }
    }
}
