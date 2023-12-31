﻿using DdnsSharp.Core.DdnsClient;
using DdnsSharp.Core.Model;
using DdnsSharp.IServices;
using DdnsSharp.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;

namespace DdnsSharp.Core
{
    public class DdnsService
    {
        private readonly IDdnsConfigService _dnsConfigService;
        private readonly IHubContext<DdnsHub> _hubContext;
        private readonly DdnsMessageContainer _messageContainer;
        private readonly HttpClient _httpClient;
        public DdnsService(IDdnsConfigService ddnsConfigService, IHubContext<DdnsHub> hubContext,DdnsMessageContainer ddnsMessageContainer,IHttpClientFactory httpClientFactory) 
        { 
            _dnsConfigService = ddnsConfigService;
            _hubContext = hubContext;
            _messageContainer = ddnsMessageContainer;
            _httpClient = httpClientFactory.CreateClient("ddns");
        }
        private IDdnsClient GetDdnsClient(DdnsConfig ddnsConfig) 
        {
            return ddnsConfig.ServiceName switch
            {
                ServiceType.DnsPod => new DNSPod(ddnsConfig),
                _ => throw new Exception("没有此类服务"),
            };
        }

        public async Task StartDdns(DdnsConfig ddnsConfig)
        {
            IDdnsClient ddnsClient = GetDdnsClient(ddnsConfig);
            var (IPV4, IPV6) = Utils.GetIP(ddnsConfig);
            try
            {
                if (ddnsConfig.IPV4.Enable)
                {
                    if (ddnsConfig.IPV4.Type == GetIPType.Url)
                    {
                        IPV4 = await _httpClient.GetStringAsync(ddnsConfig.IPV4.Url);
                    }
                    await Ddns(IPV4,ddnsConfig.IPV4.GetDomains(),ddnsClient,ddnsConfig.Ttl,AddressType.IPV4);
                }
                if (ddnsConfig.IPV6.Enable)
                {
                    if (ddnsConfig.IPV6.Type == GetIPType.Url)
                    {
                        IPV6 = await _httpClient.GetStringAsync(ddnsConfig.IPV6.Url);
                    }
                    await Ddns(IPV6, ddnsConfig.IPV6.GetDomains(), ddnsClient, ddnsConfig.Ttl, AddressType.IPV6);
                }
            }
            catch(Exception ex)
            {
                await HubDdnsMessageSendAsync($"{ex}");
            }
        }

        private async Task Ddns(string ip,string[] Domains, IDdnsClient ddnsClient,ulong? ttl,AddressType addressType)
        {
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
                try
                {
                    var recordInfoListItem = await ddnsClient.DescribeRecordList(tld);

                    var dict = new Dictionary<string, List<RecordInfoListItem>>
                                {
                                    { tld, recordInfoListItem }
                                };
                    recordInfoLists.Add(dict);
                }
                catch (Exception ex)
                {
                    await HubDdnsMessageSendAsync($"{ex} 域名：{tld}");
                }
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
                            if (recordInfoItem.Value == ip)
                            {
                                await HubDdnsMessageSendAsync($"您的IP：{ip} 没有变化 域名：{domain}");
                                continue;
                            }
                            else
                            {
                                switch (addressType)
                                {
                                    case AddressType.IPV4:
                                        await ddnsClient.ModifyRecord(i.domain, i.Subdomain, "A", "默认", ip, recordInfoItem.RecordId, ttl);
                                        break;
                                    case AddressType.IPV6:
                                        await ddnsClient.ModifyRecord(i.domain, i.Subdomain, "AAAA", "默认", ip, recordInfoItem.RecordId, ttl);
                                        break;
                                }
                                
                            }
                        }
                        else
                        {
                            switch (addressType)
                            {
                                case AddressType.IPV4:
                                    await ddnsClient.CreateRecord(i.domain, i.Subdomain, "A", "默认", ip, ttl);
                                    break;
                                case AddressType.IPV6:
                                    await ddnsClient.CreateRecord(i.domain, i.Subdomain, "AAAA", "默认", ip, ttl);
                                    break;
                            }
                            
                        }
                        await HubDdnsMessageSendAsync($"更新域名{domain}成功，IP：{ip}");
                    }
                    catch (Exception ex)
                    {
                        await HubDdnsMessageSendAsync($"更新域名{domain}失败。 {ex.Message}");
                    }

                }
            }
        }

        private async Task HubDdnsMessageSendAsync(string Message)
        {
            string msg = $"{DateTime.Now:yyyy/MM/dd HH:mm:ss} {Message}";
            _messageContainer.AddMessage(msg);
            await _hubContext.Clients.All.SendAsync("DdnsMessage",msg);
        }

        public async Task DdnsHostedService()
        { 
            List<DdnsConfig> ddnsConfigs = await _dnsConfigService.FindAllAsync(x=>x.Enable);
            var actions = (from i in ddnsConfigs select StartDdns(i));
            await Task.WhenAll(actions);
        }
    }
}
