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

builder.Services.AddDbContext<SqlDbContext>(x=>x.UseSqlite("Data Source=db.db",b=>b.MigrationsAssembly("DdnsSharp")));

builder.Services.AddAntDesign();

builder.Services.AddCustomIOC();

builder.Services.AddDdnsHostedService();

var app = builder.Build();


app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();


public static class IOCExtend
{

    public static IServiceCollection AddCustomIOC(this IServiceCollection services)
    {
        services.AddScoped<IDdnsConfigRepository, DdnsConfigRepository>();

        services.AddScoped<IDdnsConfigService,DdnsConfigService>();
        return services;
    }

    public static IServiceCollection AddDdnsHostedService(this IServiceCollection services)
    {
        services.AddHostedService<DdnsHostedService>();
        return services;
    }
}
