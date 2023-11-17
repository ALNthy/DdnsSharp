using DdnsSharp.Core;
using DdnsSharp.EFCore;
using DdnsSharp.HostedService;
using DdnsSharp.IRepository;
using DdnsSharp.IServices;
using DdnsSharp.Repository;
using DdnsSharp.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddHttpClient("ddns");
builder.Services.AddSignalR();

builder.Services.AddDbContext<SqlDbContext>(x=>x.UseSqlite("Data Source=db.db",b=>b.MigrationsAssembly("DdnsSharp")));

builder.Services.AddAntDesign();

builder.Services.AddCustomIOC();

builder.Services.AddDdnsHostedService();

var app = builder.Build();


app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();

app.MapHub<DdnsHub>(DdnsHub.HubUrl);

app.MapFallbackToPage("/_Host");

app.Run();


public static class IOCExtend
{

    public static IServiceCollection AddCustomIOC(this IServiceCollection services)
    {
        services.AddScoped<IDdnsConfigRepository, DdnsConfigRepository>();

        services.AddScoped<IDdnsConfigService,DdnsConfigService>();

        services.AddScoped<DdnsService>();
        services.AddSingleton<DdnsMessageContainer>();

        return services;
    }

    public static IServiceCollection AddDdnsHostedService(this IServiceCollection services)
    {
        services.AddHostedService<DdnsHostedService>();
        return services;
    }
}
