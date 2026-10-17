using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using SmartNav.Infrastructure.Tenancy;

namespace SmartNav.Infrastructure.Persistence;

/// <summary>Used only by `dotnet ef` to build migrations. No DB connection is opened.</summary>
public sealed class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<SmartNavDbContext>
{
    public SmartNavDbContext CreateDbContext(string[] args)
    {
        var cs = Environment.GetEnvironmentVariable("SMARTNAV_DB")
                 ?? "Server=localhost;Port=3306;Database=smartnav;User=smartnav;Password=design-time-only;GuidFormat=Binary16";
        var options = new DbContextOptionsBuilder<SmartNavDbContext>()
            .UseMySql(cs, DependencyInjection.MySqlVersion)
            .Options;
        return new SmartNavDbContext(options, new TenantContext());
    }
}
