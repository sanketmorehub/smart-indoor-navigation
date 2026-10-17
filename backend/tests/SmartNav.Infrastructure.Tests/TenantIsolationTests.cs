using Microsoft.EntityFrameworkCore;
using SmartNav.Domain.Buildings;
using SmartNav.Domain.Tenancy;
using SmartNav.Infrastructure.Qr;

namespace SmartNav.Infrastructure.Tests;

public sealed class TenantIsolationTests : IDisposable
{
    private readonly TestDb _db = new();
    private readonly Guid _hospitalId;
    private readonly Guid _hotelId;
    private readonly Guid _hospitalBuildingId;

    public TenantIsolationTests()
    {
        using var setup = _db.NewContext(out _);
        var hospital = new Tenant { Slug = "abc-hospital", Name = "ABC Hospital" };
        var hotel = new Tenant { Slug = "grand-palace", Name = "Grand Palace Hotel" };
        setup.Tenants.AddRange(hospital, hotel);
        setup.SaveChanges();
        (_hospitalId, _hotelId) = (hospital.Id, hotel.Id);

        using (var h = _db.NewContext(out var ht))
        {
            ht.SetTenant(_hospitalId);
            var b = new Building { Slug = "main", Name = "Hospital Main" };
            h.Buildings.Add(b);
            h.SaveChanges();
            _hospitalBuildingId = b.Id;
        }
        using (var o = _db.NewContext(out var ot))
        {
            ot.SetTenant(_hotelId);
            o.Buildings.Add(new Building { Slug = "main", Name = "Hotel Tower" });
            o.SaveChanges();
        }
    }

    [Fact]
    public async Task Tenant_only_sees_its_own_buildings()
    {
        using var ctx = _db.NewContext(out var t);
        t.SetTenant(_hotelId);
        var names = await ctx.Buildings.Select(b => b.Name).ToListAsync();
        Assert.Equal(["Hotel Tower"], names);
    }

    [Fact]
    public async Task Fetching_another_tenants_building_by_id_returns_null_not_forbidden()
    {
        using var ctx = _db.NewContext(out var t);
        t.SetTenant(_hotelId);
        // Simulates IDOR: hotel admin calls GET /admin/buildings/{hospitalBuildingId}
        var building = await ctx.Buildings.FirstOrDefaultAsync(b => b.Id == _hospitalBuildingId);
        Assert.Null(building); // API maps this to 404 - existence is not leaked
    }

    [Fact]
    public async Task No_tenant_context_means_no_tenant_rows_fail_closed()
    {
        using var ctx = _db.NewContext(out _);
        Assert.Empty(await ctx.Buildings.ToListAsync());
    }

    [Fact]
    public void TenantId_is_stamped_from_context_not_from_input()
    {
        using var ctx = _db.NewContext(out var t);
        t.SetTenant(_hospitalId);
        var b = new Building { Slug = "annex", Name = "Annex" };
        ctx.Buildings.Add(b);
        ctx.SaveChanges();
        Assert.Equal(_hospitalId, b.TenantId);
    }

    [Fact]
    public void Writing_without_tenant_context_throws()
    {
        using var ctx = _db.NewContext(out _);
        ctx.Buildings.Add(new Building { Slug = "x", Name = "x" });
        Assert.Throws<InvalidOperationException>(() => ctx.SaveChanges());
    }

    [Fact]
    public void Forged_TenantId_on_insert_is_blocked()
    {
        using var ctx = _db.NewContext(out var t);
        t.SetTenant(_hotelId);
        var b = new Building { Slug = "evil", Name = "Evil" };
        ctx.Buildings.Add(b);
        ctx.Entry(b).Property(nameof(Building.TenantId)).CurrentValue = _hospitalId; // attacker-controlled
        Assert.Throws<InvalidOperationException>(() => ctx.SaveChanges());
    }

    [Fact]
    public void Tenant_context_cannot_be_switched_within_a_request()
    {
        _ = _db.NewContext(out var t);
        t.SetTenant(_hospitalId);
        Assert.Throws<InvalidOperationException>(() => t.SetTenant(_hotelId));
    }

    [Fact]
    public void Soft_deleted_rows_are_hidden()
    {
        using (var ctx = _db.NewContext(out var t))
        {
            t.SetTenant(_hospitalId);
            var b = ctx.Buildings.Single(x => x.Id == _hospitalBuildingId);
            b.DeletedAt = DateTime.UtcNow;
            ctx.SaveChanges();
        }
        using var read = _db.NewContext(out var rt);
        rt.SetTenant(_hospitalId);
        Assert.Null(read.Buildings.FirstOrDefault(x => x.Id == _hospitalBuildingId));
    }

    public void Dispose() => _db.Dispose();
}

public sealed class QrTokenGeneratorTests
{
    [Fact]
    public void Tokens_are_22_char_base62_and_unique()
    {
        var gen = new QrTokenGenerator();
        var set = Enumerable.Range(0, 10_000).Select(_ => gen.NewToken()).ToHashSet();
        Assert.Equal(10_000, set.Count);
        Assert.All(set, t => Assert.Matches("^[0-9A-Za-z]{22}$", t));
    }
}
