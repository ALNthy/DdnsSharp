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
        public Task<string> CreateRecord(string Domain, string RecordType, string RecordLine, string Value, ulong? TTL);
        public Task<string> DeleteRecord(string Domain, ulong? RecordId);
        public Task<string> ModifyRecord(string Domain, string RecordType, string RecordLine, string Value, ulong RecordId, ulong? TTL);
        public IAsyncEnumerable<RecordInfoListItem> DescribeRecordList(string Domain);
    }
}
