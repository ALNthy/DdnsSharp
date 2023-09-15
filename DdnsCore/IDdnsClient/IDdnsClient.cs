using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TencentCloud.Dnspod.V20210323.Models;

namespace DdnsCore.IDdnsClient
{
    public interface IDdnsClient
    {
        public Task<string> CreateRecord(dynamic createRecord);
    }
}
