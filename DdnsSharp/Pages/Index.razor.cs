using DdnsSharp.Core;
using DdnsSharp.Core.DdnsClient;
using DdnsSharp.Core.Model;
using DdnsSharp.IServices;
using DdnsSharp.Model;
using Microsoft.AspNetCore.Components;
using System.Text.Json;

namespace DdnsSharp.Pages
{
    partial class Index
    {
        [Inject]
        IDdnsConfigService DdnsConfigService { get; set; }
        class TTL
        {
            public int? Value { get; set; }
            public string Name { get; set; }
        }

        List<TTL> _ttl;
        List<NetinterfaceData> V6netinterfaceDatas;
        List<NetinterfaceData> V4netinterfaceDatas;

        List<DdnsConfig> configs;
        DdnsConfig ddnsConfig = new();
        protected override async Task OnParametersSetAsync()
        {
            _ttl = new List<TTL>
            {
                new TTL { Value = null, Name = "自动" },
                new TTL { Value = 1, Name = "1秒" },
                new TTL { Value = 5 , Name = "5秒" },
                new TTL { Value = 10 , Name = "10秒" },
                new TTL { Value = 60 , Name = "1分钟" },
                new TTL { Value = 120 , Name = "2分钟" },
                new TTL { Value = 600 , Name = "10分钟" },
                new TTL { Value = 1800 , Name = "30分钟" },
                new TTL { Value = 3600 , Name = "1小时" }
            };
            V6netinterfaceDatas = Utils.V6NetinterfaceDatas().ToList();
            V4netinterfaceDatas = Utils.V4NetinterfaceDatas().ToList();

            configs = await DdnsConfigService.FindAllAsync();
            if (configs!=null&&configs.Any()) 
            {
                ddnsConfig = configs[0];
                await Console.Out.WriteLineAsync(JsonSerializer.Serialize(ddnsConfig));
                await Console.Out.WriteLineAsync(JsonSerializer.Serialize(ddnsConfig.IPV6.Netinterface));

                // 保证IP select 正常加载
                foreach (var i in V4netinterfaceDatas)
                {
                    if (i.Netinterface.Name == ddnsConfig.IPV4.Netinterface.Name && i.Netinterface.Index == ddnsConfig.IPV4.Netinterface.Index)
                    {
                        ddnsConfig.IPV4.Netinterface = i.Netinterface;
                    }
                }
                foreach (var i in V6netinterfaceDatas)
                {
                    if (i.Netinterface.Name == ddnsConfig.IPV6.Netinterface.Name && i.Netinterface.Index == ddnsConfig.IPV6.Netinterface.Index)
                    {
                        ddnsConfig.IPV6.Netinterface = i.Netinterface;
                    }
                }
            }
            else 
            {
                ddnsConfig = new()
                {
                    Guid = Guid.NewGuid(),
                    IPV4 = new() { Netinterface = V4netinterfaceDatas[0].Netinterface },
                    IPV6 = new() { Netinterface = V6netinterfaceDatas[0].Netinterface }
                };
            }
        }

        async Task Test()
        {
            await Console.Out.WriteLineAsync(JsonSerializer.Serialize(ddnsConfig.IPV6.Netinterface));
            await Task.Delay(0);
        }
        async Task SaveAsync()
        {
#if DEBUG
            await Console.Out.WriteLineAsync(JsonSerializer.Serialize(ddnsConfig));
#endif
            if (await DdnsConfigService.FindAnyAsync(ddnsConfig.Guid))
            {
                bool update = await DdnsConfigService.UpdateAsync(ddnsConfig);
                if (update) 
                {
                    
                }
            }
            else 
            {
                await DdnsConfigService.CreateAsync(ddnsConfig);
            }
            
        }
    }
}

