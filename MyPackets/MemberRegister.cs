﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPackets
{
    [Serializable]
    public class MemberRegister : Packet
    {
        public string id_str { get; set; }
        public string pw_str { get; set; }
        public string nickname_str { get; set; }
    }
}
