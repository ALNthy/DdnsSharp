using DdnsSharp.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TencentCloud.Dnspod.V20210323.Models;

namespace DdnsSharp.Core
{
    public interface IDdnsClient
    {
        public Task CreateRecord(string Domain,string SubDomain, string RecordType, string RecordLine, string Value, ulong? TTL);
        public Task DeleteRecord(string Domain, ulong? RecordId);
        public Task ModifyRecord(string Domain,string SubDomain, string RecordType, string RecordLine, string Value, ulong? RecordId, ulong? TTL);
        public Task<List<RecordInfoListItem>> DescribeRecordList(string Domain);
    }
}
