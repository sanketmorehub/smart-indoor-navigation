using Microsoft.EntityFrameworkCore;
using SmartNav.Infrastructure;
using SmartNav.Infrastructure.Persistence;
using SmartNav.Infrastructure.Seeding;
using SmartNav.Navigation;
using SmartNav.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddProblemDetails();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();

var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();
if (allowedOrigins is null)
{
    allowedOrigins = builder.Environment.IsDevelopment()
        ? ["http://localhost:5173", "http://127.0.0.1:5173"]
        : throw new InvalidOperationException("Configure Cors:AllowedOrigins for the deployed visitor application.");
}
builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policy => policy.WithOrigins(allowedOrigins).AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

if (args.Any(arg => string.Equals(arg, "migrate-and-seed", StringComparison.OrdinalIgnoreCase)))
{
    await using var scope = app.Services.CreateAsyncScope();
    var db = scope.ServiceProvider.GetRequiredService<SmartNavDbContext>();
    await db.Database.MigrateAsync();
    await scope.ServiceProvider.GetRequiredService<DemoDataSeeder>().SeedAsync();
    return;
}

app.UseExceptionHandler();
app.UseStatusCodePages();
app.UseCors();

app.MapHealthChecks("/health");
app.MapGet("/health/ready", async (SmartNavDbContext db, CancellationToken cancellationToken) =>
    await db.Database.CanConnectAsync(cancellationToken)
        ? Results.Ok(new { status = "ready" })
        : Results.StatusCode(StatusCodes.Status503ServiceUnavailable))
    .WithTags("Health");

app.MapGet("/api", () => Results.Ok(new
{
    name = "Smart Indoor Navigation API",
    version = "v1",
    documentation = "/swagger",
})).WithTags("API");

app.MapPublicEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();
