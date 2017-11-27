﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCPConnection
{
    [Serializable]
    class RegisterUserResult : Packet
    {
        public bool result { get; set; }
        public string reason { get; set; }
    }
}