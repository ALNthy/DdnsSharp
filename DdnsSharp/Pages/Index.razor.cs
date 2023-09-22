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

        string ServiceName = ServiceType.DnsPod.ToString();

        string getType = Model.GetType.NetInterface.ToString();
        class TTL
        {
            public int? Value { get; set; }
            public string Name { get; set; }
        }

        List<TTL> _ttl;
        int? _selectedValue = null;

        List<NetinterfaceData> V6netinterfaceDatas;
        List<NetinterfaceData> V4netinterfaceDatas;

        protected override async Task OnInitializedAsync()
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
            var DdnsConfigs = await DdnsConfigService.FindAllAsync();
            if (DdnsConfigs != null && DdnsConfigs.Count > 0)
            {
                await Console.Out.WriteLineAsync("12312312");
            }
        }
        DdnsConfig ddnsConfig = new() { Guid = Guid.NewGuid(), IPV4 = new(), IPV6 = new() };
    }
}

