using DdnsSharp.Core.DdnsClient;
using DdnsSharp.Core.Model;
using DdnsSharp.IServices;
using DdnsSharp.Model;
using DdnsSharp.SignalR;
using Microsoft.AspNetCore.SignalR;

namespace DdnsSharp.Core
{
    public class DdnsService
    {
        private readonly IDdnsConfigService _dnsConfigService;
        private readonly IHubContext<DdnsHub> _hubContext;
        public DdnsService(IDdnsConfigService ddnsConfigService, IHubContext<DdnsHub> hubContext) 
        { 
            _dnsConfigService = ddnsConfigService;
            _hubContext = hubContext;
        }
        private IDdnsClient GetDdnsClient(DdnsConfig ddnsConfig) 
        {
            switch (ddnsConfig.ServiceName)
            {
                case ServiceType.DnsPod:
                    return new DNSPod(ddnsConfig);
                default: throw new Exception("没有此类服务");
            }
        }

        public async Task StartDdns(DdnsConfig ddnsConfig)
        {
            IDdnsClient ddnsClient = GetDdnsClient(ddnsConfig);
            var (IPV4, IPV6) = Utils.GetIP(ddnsConfig);
            if (ddnsConfig.IPV4.Enable)
            {
                string[] Domains = ddnsConfig.IPV4.GetDomains();
                List<(string Subdomain, string domain)> domains = new List<(string Subdomain, string domain)>();
                HashSet<string> TLDs = new HashSet<string>();
                foreach (string domain in Domains)
                {
                    string[] strings = domain.Split(".");
                    if (strings.Length == 2)
                    {
                        domains.Add(("", domain));
                        TLDs.Add(domain);
                    }
                    else if (strings.Length == 3)
                    {
                        domains.Add((strings[0], strings[1] + "." + strings[2]));
                        TLDs.Add(strings[1] + "." + strings[2]);
                    }
                }
                List<Dictionary<string, List<RecordInfoListItem>>> recordInfoLists = new();
                foreach (string tld in TLDs)
                {
                    await Console.Out.WriteLineAsync(tld);
                    var recordInfoListItem = await ddnsClient.DescribeRecordList(tld);
                    var dict = new Dictionary<string, List<RecordInfoListItem>>
                    {
                        { tld, recordInfoListItem }
                    };
                    recordInfoLists.Add(dict);
                }
            }
            if (ddnsConfig.IPV6.Enable)
            {
                string[] Domains = ddnsConfig.IPV6.GetDomains();
                List<(string Subdomain, string domain)> domains = new List<(string Subdomain, string domain)>();
                HashSet<string> TLDs = new HashSet<string>();
                foreach (string domain in Domains)
                {
                    string[] strings = domain.Split(".");
                    if (strings.Length == 2)
                    {
                        domains.Add(("", domain));
                        TLDs.Add(domain);
                    }
                    else if (strings.Length == 3)
                    {
                        domains.Add((strings[0], strings[1] + "." + strings[2]));
                        TLDs.Add(strings[1] + "." + strings[2]);
                    }
                }
                List<Dictionary<string, List<RecordInfoListItem>>> recordInfoLists = new();
                foreach (string tld in TLDs)
                {
                    await Console.Out.WriteLineAsync(tld);
                    var recordInfoListItem = await ddnsClient.DescribeRecordList(tld);
                    recordInfoListItem.ForEach(x => { Console.WriteLine(x.Value); });
                    var dict = new Dictionary<string, List<RecordInfoListItem>>
                    {
                        { tld, recordInfoListItem }
                    };
                    recordInfoLists.Add(dict);
                }
            }
            // 还没写完
        }
    }
}
