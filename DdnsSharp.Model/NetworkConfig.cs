using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DdnsSharp.Model
{
    public class NetworkConfig
    {
        public Guid Guid { get; set; }
        public bool Enable { get; set; }
        public GetType Type { get; set; }
        public string Netinterface { get; set; }
        public byte IpReg { get; set; }
        public string[] Domains { get; set; }
    }
}
