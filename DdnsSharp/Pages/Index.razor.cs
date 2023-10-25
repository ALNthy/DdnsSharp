using AntDesign;
using DdnsSharp.Core;
using DdnsSharp.Core.DdnsClient;
using DdnsSharp.Core.Model;
using DdnsSharp.IServices;
using DdnsSharp.Model;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using System.Text.Json;

namespace DdnsSharp.Pages
{
    partial class Index
    {
        [Inject]
        IDdnsConfigService DdnsConfigService { get; set; }
        [Inject]
        IMessageService _message {  get; set; }
        [Inject]
        NavigationManager navigationManager { get; set; }
        [Inject]
        DdnsService ddnsService { get; set; }
        [Inject]
        DdnsMessageContainer messageContainer { get; set; }

        private HubConnection _hubConnection;

        class TTL
        {
            public int? Value { get; set; }
            public string Name { get; set; }
        }

        class SelectDdnsConfig
        {
            public string Name { get; set; }
            public DdnsConfig Value { get; set; }
        }

        List<TTL> _ttl;
        List<NetinterfaceData> V6netinterfaceDatas;
        List<NetinterfaceData> V4netinterfaceDatas;

        List<DdnsConfig> configs;
        List<SelectDdnsConfig> selectDdnsConfigs = new();
        DdnsConfig ddnsConfig = new();
        bool _visible = false;

        List<string> logs = new();

        protected override async Task OnParametersSetAsync()
        {
            foreach (string message in messageContainer.GetMessages())
            {
                logs.Add(message);
            }
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

            configs = await DdnsConfigService.FindAllAsync(x=>x.Enable);

            if (configs is not null)
            {
                selectDdnsConfigs = (from s in configs select (new SelectDdnsConfig() { Name = s.Name, Value = s })).ToList();
            }

            if (configs!=null&&configs.Any()) 
            {
                ddnsConfig = configs[0];

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
                ddnsConfig = GetNewDdnsConfig();
                selectDdnsConfigs.Add(new() { Name = $"{ddnsConfig.Name}-(未保存)", Value = ddnsConfig });
            }
            string baseUrl = navigationManager.BaseUri;
            var _hubUrl = baseUrl.TrimEnd('/') + DdnsHub.HubUrl;
            _hubConnection = new HubConnectionBuilder().WithUrl(_hubUrl).Build();
            await _hubConnection.StartAsync();
            _hubConnection.On<string>("DdnsMessage", DdnsMessage);
        }
        async Task SaveAsync()
        {
#if DEBUG
            await Console.Out.WriteLineAsync(JsonSerializer.Serialize(ddnsConfig));
#endif
            bool s = false;
            if (await DdnsConfigService.FindAnyAsync(ddnsConfig.Guid))
            {
                s = await DdnsConfigService.UpdateAsync(ddnsConfig);
            }
            else 
            {
                s = await DdnsConfigService.CreateAsync(ddnsConfig);
            }
            if (s)
            {
                var selectddnsconfig = selectDdnsConfigs.Find(x => x.Value.Guid == ddnsConfig.Guid);
                int index = selectDdnsConfigs.IndexOf(selectddnsconfig);
                selectDdnsConfigs[index] = new() { Name =selectddnsconfig.Value.Name, Value = selectddnsconfig.Value };
                await Task.WhenAll(_hubConnection.SendAsync("DdnsMessage", $"{ddnsConfig.Name}保存成功"));
            }
            else
            {
                await Task.WhenAll(_hubConnection.SendAsync("DdnsMessage", $"{ddnsConfig.Name}保存失败"));
            }
            await ddnsService.StartDdns(ddnsConfig);
        }

        async Task DdnsMessage(string message)
        {
            logs.Add(message);

            await Task.WhenAll(_message.Info(message),InvokeAsync(StateHasChanged));
        }


        async Task AddSelectDdnsConfig()
        {
            var config = GetNewDdnsConfig();
            selectDdnsConfigs.Add(new() { Name = $"{config.Name}-(未保存)", Value = config });
            await InvokeAsync(StateHasChanged);
            await Task.Delay(1);
            ddnsConfig = config;
        }

        async Task DeleteDdnsConfig()
        {
            if (await DdnsConfigService.FindAnyAsync(ddnsConfig.Guid))
            {
                await DdnsConfigService.DeletedAsync(ddnsConfig);
            }
            selectDdnsConfigs.Remove(selectDdnsConfigs.Find(x => x.Value.Guid == ddnsConfig.Guid));
            if (selectDdnsConfigs.Any())
            {
                ddnsConfig = selectDdnsConfigs[0].Value;
            }
            else
            {
                ddnsConfig = GetNewDdnsConfig();
                SelectDdnsConfig Items = new() { Name = $"{ddnsConfig.Name}-(未保存)", Value = ddnsConfig };
                selectDdnsConfigs.Add(Items);
            }
            await Task.WhenAll(_hubConnection.SendAsync("DdnsMessage", $"{ddnsConfig.ServiceName}-{ddnsConfig.Guid} 删除成功"));
        }

        DdnsConfig GetNewDdnsConfig()
        { 
            Guid id = Guid.NewGuid();
            return new()
            {
                Guid = id,Name = $"{ddnsConfig.ServiceName}-{id}",
                ServiceName=ddnsConfig.ServiceName,
                IPV4 = new() { Netinterface = V4netinterfaceDatas[0].Netinterface,Url="https://4.ipw.cn"},
                IPV6 = new() { Netinterface = V6netinterfaceDatas[0].Netinterface, Url = "https://6.ipw.cn" }
            };
        }

        public async ValueTask DisposeAsync()
        {
            if (_hubConnection is not null)
            {
                await _hubConnection.StopAsync();
                await _hubConnection.DisposeAsync();
            }
        }
    }
}

