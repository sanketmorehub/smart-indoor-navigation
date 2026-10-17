using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SmartNav.Infrastructure.Persistence;
using SmartNav.Infrastructure.Tenancy;
using SmartNav.Shared;

namespace SmartNav.Infrastructure.Tests;

/// <summary>
/// Shared in-memory SQLite database to test EF-level isolation (global query filters +
/// SaveChanges interceptor) fast. Each NewContext() simulates a separate HTTP request scope.
/// MySQL-only guarantees (composite FK enforcement across tenants) get Testcontainers
/// integration tests in a later phase.
/// </summary>
public sealed class TestDb : IDisposable
{
    private readonly SqliteConnection _conn = new("DataSource=:memory:");

    public TestDb()
    {
        _conn.Open();
        using var ctx = NewContext(out _);
        ctx.Database.EnsureCreated();
    }

    public SmartNavDbContext NewContext(out TenantContext tenant)
    {
        tenant = new TenantContext();
        var options = new DbContextOptionsBuilder<SmartNavDbContext>()
            .UseSqlite(_conn)
            .AddInterceptors(new AuditableTenantInterceptor(tenant, new SystemClock()))
            .Options;
        return new SmartNavDbContext(options, tenant);
    }

    public void Dispose() => _conn.Dispose();
}
