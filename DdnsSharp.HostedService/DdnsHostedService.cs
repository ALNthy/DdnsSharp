using DdnsSharp.IServices;
using DdnsSharp.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
                await Task.Delay(300000);
            }
        }

        private async Task WokrAsync(CancellationToken cancellationToken)
        {
            using (var scope = _serviceScope.CreateScope())
            {
                IDdnsConfigService _ddnsConfigService = scope.ServiceProvider.GetService<IDdnsConfigService>();
                await Task.Delay(0);
            };
        }
    }
}