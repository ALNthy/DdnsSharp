using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using DdnsSharp.Core;

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
                DdnsService ddnsService = scope.ServiceProvider.GetService<DdnsService>();
                await ddnsService.DdnsHostedService();
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