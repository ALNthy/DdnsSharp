using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DdnsSharp.Model
{
    public class NetworkConfig
    {
        public Guid Guid { get; init; } =Guid.NewGuid();
        public bool Enable { get; set; }
        public GetType Type { get; set; }
        public Netinterface Netinterface { get; set; }
        public string Domains { get; set; }

        public string[] GetDomains()
        {
            return Domains.Split("\n");
        }
    }
}
