using DdnsSharp.IServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using DdnsSharp.SignalR;
using Microsoft.AspNetCore.SignalR;

namespace DdnsSharp.HostedService
{
    public class DdnsHostedService : BackgroundService, IAsyncDisposable
    {
        private readonly IServiceScopeFactory _serviceScope;

        Timer timer;

        public DdnsHostedService(IServiceScopeFactory serviceScope)
        {
            _serviceScope = serviceScope;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            timer = new Timer(async (s) =>
            {
                await WokrAsync(stoppingToken);
            }, null, TimeSpan.Zero, TimeSpan.FromSeconds(300));
            await Task.Delay(0,stoppingToken);
        }

        private async Task WokrAsync(CancellationToken cancellationToken)
        {
            using (var scope =  _serviceScope.CreateAsyncScope())
            {
                IDdnsConfigService _ddnsConfigService = scope.ServiceProvider.GetService<IDdnsConfigService>();
                IHubContext<DdnsHub> hubContext = scope.ServiceProvider.GetService<IHubContext<DdnsHub>>();
                await _ddnsConfigService.FindAllAsync(x=>x.Enable);
                await hubContext.Clients.All.SendAsync("DdnsMessage", "测试内容",cancellationToken);
            };
        }

        public async ValueTask DisposeAsync()
        {
            if (timer is not null)
            {
                await timer.DisposeAsync();
            }
        }
    }
}