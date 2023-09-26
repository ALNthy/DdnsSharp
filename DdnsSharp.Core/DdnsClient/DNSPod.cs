using DdnsSharp.Core.Model;
using DdnsSharp.Model;
using Newtonsoft.Json.Linq;
using TencentCloud.Common;
using TencentCloud.Common.Profile;
using TencentCloud.Dnspod.V20210323;
using TencentCloud.Dnspod.V20210323.Models;

namespace DdnsSharp.Core.DdnsClient
{
    public class DNSPod : IDdnsClient   
    {

        private readonly DnspodClient m_client;

        public DNSPod(string Id, string Key) 
        {
            m_client = new(new() {SecretId=Id,SecretKey=Key },"");
        }
        public DNSPod(DdnsConfig ddnsConfig)
        {
            m_client = new(new() { SecretId = ddnsConfig.Id, SecretKey = ddnsConfig.Key }, "");
        }

        async Task<string> IDdnsClient.CreateRecord(string Domain, string RecordType, string RecordLine, string Value, ulong? TTL)
        {
            await m_client.CreateRecord(new() {
                Domain=Domain,
                RecordType=RecordType,
                RecordLine=RecordLine,
                Value=Value
            });
            return "添加成功";
        }

        async Task<string> IDdnsClient.DeleteRecord(string Domain, ulong? RecordId)
        {
            await m_client.DeleteRecord(new() { Domain = Domain,RecordId = RecordId});
            return "删除成功";
        }

        async Task<List<RecordInfoListItem>> IDdnsClient.DescribeRecordList(string Domain)
        {
            List<RecordInfoListItem> recordInfoListItems = new List<RecordInfoListItem>();
            var res = await m_client.DescribeRecordList(new() { Domain = Domain });
            foreach (var i in res.RecordList)
            {
                recordInfoListItems.Add(new()
                {
                    Name = i.Name,
                    RecordId = i.RecordId,
                    Value = i.Value,
                    Status = i.Status,
                    UpdatedOn = i.UpdatedOn,
                    Type = i.Type,
                    Weight = i.Weight,
                    Remark = i.Remark,
                    TTL = i.TTL
                });
            }
            return recordInfoListItems;
        }

        async Task<string> IDdnsClient.ModifyRecord(string Domain, string RecordType, string RecordLine, string Value, ulong RecordId, ulong? TTL)
        {
            await m_client.ModifyRecord(new() { Domain = Domain, RecordType=RecordType, RecordLine=RecordLine, Value=Value, RecordId=RecordId,TTL=TTL});
            return "修改成功";
        }
    }
}
