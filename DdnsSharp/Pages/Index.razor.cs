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

        //private record TTL
        //{
        //    public int? Value { get; set; }
        //    public string Name { get; set; }

        //    public TTL(int? Value, string Name)
        //    { 
        //        this.Value = Value;
        //        this.Name = Name;
        //    }
        //}

        private record TTL(int? Value, string Name);
        private (string id, string key,string oid,string okey) user = (string.Empty, string.Empty,string.Empty,string.Empty);
        private const string cover = "************";
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

        DdnsConfig UpdateDdnsConfig(DdnsConfig config)
        { 
            ddnsConfig = config;
            if (!string.IsNullOrEmpty(ddnsConfig.Id))
            {
                user.id = cover;
                user.oid = ddnsConfig.Id;
            }
            else
            {
                user.id = string.Empty;
                user.oid = string.Empty;
            }
            if (!string.IsNullOrEmpty(ddnsConfig.Key))
            {
                user.key = cover;
                user.okey = ddnsConfig.Key;
            }
            else
            {
                user.key = string.Empty;
                user.okey = string.Empty;
            }
            return ddnsConfig;
        }
        protected override async Task OnParametersSetAsync()
        {
            foreach (string message in messageContainer.GetMessages())
            {
                logs.Add(message);
            }
            _ttl = new List<TTL>
            {
                new TTL(null,"自动"),
                new TTL (1, "1秒"),
                new TTL (5, "5秒"),
                new TTL (10, "10秒"),
                new TTL (60, "1分钟"),
                new TTL (120, "2分钟"),
                new TTL (600, "10分钟"),
                new TTL (1800, "30分钟"),
                new TTL (3600, "1小时")
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
                ddnsConfig = UpdateDdnsConfig(configs[0]);

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
                ddnsConfig = UpdateDdnsConfig(GetNewDdnsConfig());
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
            if (string.IsNullOrEmpty(user.oid))
            {
                ddnsConfig.Id = user.id;
            }
            else
            {
                if (user.id != cover)
                {
                    ddnsConfig.Id = user.id;
                }
                else
                {
                    ddnsConfig.Id = user.oid;
                }
            }
            if (string.IsNullOrEmpty(user.okey))
            {
                ddnsConfig.Key = user.okey;
            }
            else
            {
                if (user.key != cover)
                {
                    ddnsConfig.Key = user.key;
                }
                else
                {
                    ddnsConfig.Key = user.okey;
                }
            }
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
            _=Task.Run(async () =>
            {
                await Task.WhenAll(_message.Info(message), InvokeAsync(StateHasChanged));
            });
            await Task.Yield();
        }


        async Task AddSelectDdnsConfig()
        {
            var config = GetNewDdnsConfig();
            selectDdnsConfigs.Add(new() { Name = $"{config.Name}-(未保存)", Value = config });
            await InvokeAsync(StateHasChanged);
            await Task.Delay(1);
            ddnsConfig = UpdateDdnsConfig(config);
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
                ddnsConfig = UpdateDdnsConfig(selectDdnsConfigs[0].Value);
            }
            else
            {
                ddnsConfig = UpdateDdnsConfig(GetNewDdnsConfig());
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

