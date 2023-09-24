using DdnsSharp.IServices;
using DdnsSharp.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using DdnsSharp.SignalR;
using Microsoft.AspNetCore.SignalR;

namespace DdnsSharp.HostedService
{
    public class DdnsHostedService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScope;

        public DdnsHostedService(IServiceScopeFactory serviceScope)
        {
            _serviceScope = serviceScope;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (true)
            {
                _ = Task.Run(async () =>
                {
                    await WokrAsync(stoppingToken);
                }, stoppingToken);
                await Task.Delay(30000);
            }
        }

        private async Task WokrAsync(CancellationToken cancellationToken)
        {
            using (var scope =  _serviceScope.CreateAsyncScope())
            {
                IDdnsConfigService _ddnsConfigService = scope.ServiceProvider.GetService<IDdnsConfigService>();
                IHubContext<DdnsHub> hubContext = scope.ServiceProvider.GetService<IHubContext<DdnsHub>>();
                await _ddnsConfigService.FindAllAsync();
                await hubContext.Clients.All.SendAsync("DdnsMessage", "测试内容",cancellationToken);
                await Task.Delay(0,cancellationToken);
            };
        }
    }
}