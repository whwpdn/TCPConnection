using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPackets
{
    [Serializable]
    public class LoginResult : Packet
    {
        public bool result { get; set;}
        public string reason { get; set; }
        public string timeleft { get; set; }
    }

    [Serializable]
    public class LogoutResult : Packet
    {
        public bool result { get; set; }
        public string reason { get; set; }
    }
}
