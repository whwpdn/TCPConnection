using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPackets
{
    [Serializable]
    public class Login : Packet
    {
        public string id_str { get; set; }
        public string pw_str { get; set; }
        //public string pcno_str { get; set; }

    }

    [Serializable]
    public class Logout : Packet
    {
        public string id_str { get; set; }
        public string timeleft { get; set; }
        //public string pcno_str { get; set; }
    }
}
