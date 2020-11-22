using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NeighBot
{
    public class WebProxySettings
    {
        public bool Enabled { get; set; }
        public string Host { get; set; }
        public ushort Port { get; set; }
    }
}
