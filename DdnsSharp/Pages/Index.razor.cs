using AntDesign;
using DdnsSharp.Core;
using DdnsSharp.Core.DdnsClient;
using DdnsSharp.Core.Model;
using DdnsSharp.IServices;
using DdnsSharp.Model;
using DdnsSharp.SignalR;
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
        List<SelectDdnsConfig> selectDdnsConfigs;
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

            if (configs is not null)
            {
                selectDdnsConfigs = (from s in configs select (new SelectDdnsConfig() { Name = s.Name, Value = s })).ToList();
            }

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
                ddnsConfig = GetNewDdnsConfig();
                selectDdnsConfigs.Add(new() { Name = ddnsConfig.Name, Value = ddnsConfig });
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
                await Task.WhenAll(_message.Success("保存成功"),_hubConnection.SendAsync("DdnsMessage", "保存成功"));
            }
            else
            {
                await Task.WhenAll(_message.Error("保存失败"),_hubConnection.SendAsync("DdnsMessage", "保存失败"));
            }
        }

        async Task DdnsMessage(string message)
        {
            await Console.Out.WriteLineAsync(message);
            await InvokeAsync(StateHasChanged);
        }


        async Task AddSelectDdnsConfig()
        {
            ddnsConfig = GetNewDdnsConfig();
            SelectDdnsConfig Items = new() { Name = ddnsConfig.Name, Value = ddnsConfig };
            selectDdnsConfigs.Add(Items);
            await InvokeAsync(StateHasChanged);
        }

        async Task DeleteDdnsConfig()
        {
            bool s = false;
            try 
            {
                if (await DdnsConfigService.FindAnyAsync(ddnsConfig.Guid))
                {
                    s = await DdnsConfigService.DeletedAsync(ddnsConfig);
                    selectDdnsConfigs.Remove(selectDdnsConfigs.Find(x => x.Value.Guid == ddnsConfig.Guid));
                    await Task.WhenAll(_message.Success($"{ddnsConfig.ServiceName}-{ddnsConfig.Guid} 删除成功"), _hubConnection.SendAsync("DdnsMessage", $"{ddnsConfig.ServiceName}-{ddnsConfig.Guid} 删除成功"));
                }
                else
                {
                    await Task.WhenAll(_message.Error($"数据库中不存在 {ddnsConfig.ServiceName}-{ddnsConfig.Guid} 删除失败"), _hubConnection.SendAsync("DdnsMessage", $"数据库中不存在 {ddnsConfig.ServiceName}-{ddnsConfig.Guid} 删除失败"));
                }
            }
            catch (Exception ex) 
            {
                await Task.WhenAll(_message.Error($"{ddnsConfig.ServiceName}-{ddnsConfig.Guid} 删除失败 {ex.Message}"), _hubConnection.SendAsync("DdnsMessage", $"{ddnsConfig.ServiceName}-{ddnsConfig.Guid} 删除失败 {ex.Message}"));
            }
            if (selectDdnsConfigs.Any())
            {
                ddnsConfig = selectDdnsConfigs[0].Value;
            }
            else
            {
                ddnsConfig = GetNewDdnsConfig();
                SelectDdnsConfig Items = new() { Name = ddnsConfig.Name, Value = ddnsConfig };
                selectDdnsConfigs.Add(Items);
            }
        }

        DdnsConfig GetNewDdnsConfig()
        { 
            Guid id = Guid.NewGuid();
            return new()
            {
                Guid = id,Name = $"{ddnsConfig.ServiceName}-{id}",
                ServiceName=ddnsConfig.ServiceName,
                IPV4 = new() { Netinterface = V4netinterfaceDatas[0].Netinterface },
                IPV6 = new() { Netinterface = V6netinterfaceDatas[0].Netinterface }
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

