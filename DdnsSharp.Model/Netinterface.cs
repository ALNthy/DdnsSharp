using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DdnsSharp.Model
{
    public class Netinterface
    {
        public Netinterface() { }
        public Netinterface(string name,byte index) 
        {
            Name = name;
            Index = index;
        }
        public Guid Guid { get; init; } = Guid.NewGuid();
        public string Name { get; set; }
        public byte Index { get; set; }
    }
}
