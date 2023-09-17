using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DdnsSharp.Core.Model
{
    public class NetworkModel
    {
        public List<Dictionary<string, List<string>>> Ipv4 { get; set; }
        public List<Dictionary<string, List<string>>> Ipv6 { get; set; }
    }
}
