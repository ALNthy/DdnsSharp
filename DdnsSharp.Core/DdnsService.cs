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
                if (ddnsConfig.IPV4.Type== GetIPType.Url) 
                {
                
                }
                if (ddnsConfig.IPV4.Type == GetIPType.NetInterface)
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
                        var recordInfoListItem = await ddnsClient.DescribeRecordList(tld);
                        var dict = new Dictionary<string, List<RecordInfoListItem>>
                    {
                        { tld, recordInfoListItem }
                    };
                        recordInfoLists.Add(dict);
                    }
                    foreach (var i in domains)
                    {
                        string domain = i.Subdomain is null ? i.domain : $"{i.Subdomain}.{i.domain}";
                        var recordInfoListItem = recordInfoLists.Find(x => x.ContainsKey(i.domain));
                        if (recordInfoListItem != null)
                        {
                            try
                            {
                                var recordInfoItem = recordInfoListItem[i.domain].Find(x => x.Name == i.Subdomain);
                                if (recordInfoItem != null)
                                {
                                    if (recordInfoItem.Value == IPV4)
                                    {
                                        await HubDdnsMessageSendAsync($"您的IP：{IPV4} 没有变化 域名：{domain}");
                                        continue;
                                    }
                                    else
                                    {
                                        await ddnsClient.ModifyRecord(i.domain, i.Subdomain, "A", "默认", IPV4, recordInfoItem.RecordId, ddnsConfig.Ttl);
                                    }
                                }
                                else
                                {
                                    await ddnsClient.CreateRecord(i.domain, i.Subdomain, "A", "默认", IPV4, ddnsConfig.Ttl);
                                }
                                await HubDdnsMessageSendAsync($"更新域名{domain}成功，IP：{IPV4}");
                            }
                            catch (Exception ex)
                            {
                                await HubDdnsMessageSendAsync($"更新域名{domain}失败。 {ex.Message}");
                            }

                        }
                    }
                }
            }
            if (ddnsConfig.IPV6.Enable)
            {
                if (ddnsConfig.IPV6.Type == GetIPType.Url)
                {

                }
                if (ddnsConfig.IPV6.Type == GetIPType.NetInterface)
                {
                    string[] Domains = ddnsConfig.IPV6.GetDomains();
                    List<(string Subdomain, string domain)> domains = new List<(string Subdomain, string domain)>();
                    HashSet<string> TLDs = new HashSet<string>();
                    foreach (string domain in Domains)
                    {
                        string[] strings = domain.Split(".");
                        if (strings.Length == 2)
                        {
                            domains.Add((null, domain));
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
                        var recordInfoListItem = await ddnsClient.DescribeRecordList(tld);

                        var dict = new Dictionary<string, List<RecordInfoListItem>>
                        {
                            { tld, recordInfoListItem }
                        };
                        recordInfoLists.Add(dict);
                    }
                    foreach (var i in domains)
                    {
                        string domain = i.Subdomain is null ? i.domain : $"{i.Subdomain}.{i.domain}";
                        var recordInfoListItem = recordInfoLists.Find(x => x.ContainsKey(i.domain));
                        if (recordInfoListItem != null)
                        {
                            try
                            {
                                var recordInfoItem = recordInfoListItem[i.domain].Find(x => x.Name == i.Subdomain);
                                if (recordInfoItem != null)
                                {
                                    if (recordInfoItem.Value == IPV6)
                                    {
                                        await HubDdnsMessageSendAsync($"您的IP：{IPV6} 没有变化 域名：{domain}");
                                        continue;
                                    }
                                    else
                                    {
                                        await ddnsClient.ModifyRecord(i.domain, i.Subdomain, "AAAA", "默认", IPV6, recordInfoItem.RecordId, ddnsConfig.Ttl);
                                    }
                                }
                                else
                                {
                                    await ddnsClient.CreateRecord(i.domain, i.Subdomain, "AAAA", "默认", IPV6, ddnsConfig.Ttl);
                                }
                                await HubDdnsMessageSendAsync($"更新域名{domain}成功，IP：{IPV6}");
                            }
                            catch (Exception ex)
                            {
                                await HubDdnsMessageSendAsync($"更新域名{domain}失败。 {ex.Message}");
                            }
                        }
                    }
                }
            }
        }
        private async Task HubDdnsMessageSendAsync(string Message)
        {
            await _hubContext.Clients.All.SendAsync("DdnsMessage", $"{DateTime.Now:yyyy/MM/dd HH:mm:ss} {Message}");
        }

        public async Task DdnsHostedService()
        { 
            List<DdnsConfig> ddnsConfigs = await _dnsConfigService.FindAllAsync(x=>x.Enable);
            var actions = (from i in ddnsConfigs select StartDdns(i));
            await Task.WhenAll(actions);
        }
    }
}
