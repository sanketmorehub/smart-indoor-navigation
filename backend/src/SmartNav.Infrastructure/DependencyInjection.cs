using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartNav.Application.Abstractions;
using SmartNav.Infrastructure.Persistence;
using SmartNav.Infrastructure.Qr;
using SmartNav.Infrastructure.Seeding;
using SmartNav.Infrastructure.Tenancy;
using SmartNav.Shared;

namespace SmartNav.Infrastructure;

public static class DependencyInjection
{
    /// <summary>Pinned so migrations never need a live DB to detect the version.</summary>
    public static readonly MySqlServerVersion MySqlVersion = new(new Version(8, 0, 36));

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        var cs = config.GetConnectionString("Default")
                 ?? throw new InvalidOperationException("ConnectionStrings:Default is not configured.");

        services.AddSingleton<IClock, SystemClock>();
        services.AddScoped<TenantContext>();
        services.AddScoped<ITenantContext>(sp => sp.GetRequiredService<TenantContext>());
        services.AddScoped<ITenantContextSetter>(sp => sp.GetRequiredService<TenantContext>());
        services.AddScoped<AuditableTenantInterceptor>();
        services.AddSingleton<IQrTokenGenerator, QrTokenGenerator>();

        services.AddDbContext<SmartNavDbContext>((sp, o) =>
        {
            o.UseMySql(cs, MySqlVersion, my => my.EnableRetryOnFailure(3));
            o.AddInterceptors(sp.GetRequiredService<AuditableTenantInterceptor>());
        });

        services.AddScoped<DemoDataSeeder>();
        return services;
    }
}
