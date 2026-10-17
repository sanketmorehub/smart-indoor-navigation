using Microsoft.EntityFrameworkCore;
using SmartNav.Infrastructure;
using SmartNav.Infrastructure.Persistence;
using SmartNav.Infrastructure.Seeding;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();

var app = builder.Build();

// `dotnet run -- migrate-and-seed` applies migrations and loads demo data, then exits.
if (args.Contains("migrate-and-seed"))
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<SmartNavDbContext>();
    if (!db.Database.GetMigrations().Any())
        throw new InvalidOperationException(
            "No EF Core migrations found. Run from /backend: dotnet ef migrations add InitialSchema " +
            "-p src/SmartNav.Infrastructure -s src/SmartNav.Infrastructure -o Persistence/Migrations");
    await db.Database.MigrateAsync();
    await scope.ServiceProvider.GetRequiredService<DemoDataSeeder>().SeedAsync();
    return;
}

if (app.Environment.IsDevelopment()) app.MapOpenApi();

app.UseExceptionHandler();
app.UseHttpsRedirection();

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.Run();

public partial class Program;
