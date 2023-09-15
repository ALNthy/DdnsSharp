using AlibabaCloud.OpenApiClient.Models;
using AlibabaCloud.SDK.Alidns20150109;
using AlibabaCloud.SDK.Alidns20150109.Models;

namespace DdnsCore.DdnsClient
{
    public class Alidns : Client, IDdnsClient.IDdnsClient
    {
        public Alidns(Config config) : base(config)
        {
        }

        public Alidns(string id, string key) : base(new() {AccessKeyId=id,AccessKeySecret=key })
        { }

        public async Task<string> CreateRecord(dynamic createRecord)
        {
            if (createRecord is not AddDomainRecordRequest _params)
            {
                throw new NullReferenceException($"{nameof(_params)} is Null");
            }
            await base.AddDomainRecordAsync(_params);
            return "";
        }
    }
}

