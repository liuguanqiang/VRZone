using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRZoneLib.Classes.Units
{
    public class VZ_StateInfo
    {
        public String return_code = "";
        public String nickname = "VRZone";
        public String headimgurl = "";
        public String transaction_id = "";
        public long expire;
        public long firstPayTime;
        public long pay_time;
        public long current_time;
        public String price = "?";
        public String watch_time = "?";
        public int ret;

        public String[] priceArr;
        public String[] watchtimeArr;
    }
}
