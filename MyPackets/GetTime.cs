using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPackets
{
    [Serializable]
    public class GetTime : Packet
    {
        public string id_str { get; set; }
        public string timeleft { get; set; }
    }
}
