using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TencentCloud.Common;
using TencentCloud.Common.Profile;
using TencentCloud.Dnspod.V20210323.Models;

namespace DdnsCore.DdnsClient
{
    public class DNSPod : AbstractClient, IDdnsClient.IDdnsClient
    {
        public DNSPod(string endpoint, string version, Credential credential, string region, ClientProfile profile) 
            : base(endpoint, version, credential, region, profile)
        {
        }
        public DNSPod(string id, string key) : base("dnspod.tencentcloudapi.com", "2021-03-23", credential: new() { SecretId = id, SecretKey = key }, "", new() {HttpProfile =new() {Endpoint= "dnspod.tencentcloudapi.com" } })
        {
            
        }
        public async Task<string> CreateRecord(dynamic createRecord)
        {
            if (createRecord is not CreateRecordRequest _params)
            {   
                throw new NullReferenceException($"{nameof(_params)} is Null");
            }
            return await base.InternalRequest(_params, "CreateRecord");
        }
    }
}
