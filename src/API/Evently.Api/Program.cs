using Evently.Api.Extensions;
using Evently.Api.Middleware;
using Evently.Common.Application;
using Evently.Common.Infrastructure;
using Evently.Common.Presentation.Endpoints;
using Evently.Modules.Events.Infrastructure;
using Evently.Modules.Ticketing.Infrastructure;
using Evently.Modules.Users.Infrastructure;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, loggerConfig) => loggerConfig.ReadFrom.Configuration(context.Configuration));

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddOpenApi();

string databaseConnectionString = builder.Configuration.GetConnectionString("Database") 
                                  ?? throw new ArgumentException("Database connection string is required");

string redisConnectionString = builder.Configuration.GetConnectionString("Cache") 
                               ?? throw new ArgumentException("Cache connection string is required");

builder.Services.AddApplication([
    Evently.Modules.Events.Application.AssemblyReference.Assembly, 
    Evently.Modules.Users.Application.AssemblyReference.Assembly, 
    Evently.Modules.Ticketing.Application.AssemblyReference.Assembly
]);
builder.Services.AddInfrastructure(
    [TicketingModule.ConfigureConsumers],
    databaseConnectionString, 
    redisConnectionString);
builder.Configuration.AddModuleConfiguration(["events", "users", "ticketing"]);

string keycloakHealthUrl = builder.Configuration.GetSection("KeyCloak:HealthUrl").Value;

if (string.IsNullOrEmpty(keycloakHealthUrl))
{
    throw new ArgumentException("KeyCloak Health Url is required");
}

builder.Services.AddHealthChecks()
    .AddNpgSql(databaseConnectionString)
    .AddRedis(redisConnectionString)
    .AddUrlGroup(new Uri(keycloakHealthUrl), HttpMethod.Get, "keycloak");

builder.Services.AddEventsModule(builder.Configuration);
builder.Services.AddUsersModule(builder.Configuration);
builder.Services.AddTicketingModule(builder.Configuration);

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApiWithScalar();
    app.ApplyMigrations();
}

app.MapGet("/", () => $"Welcome to Evently API ({(app.Environment.IsDevelopment() ? "Dev" : "Prod")})");

app.MapEndpoints();

//for failover / alerts about the api
app.MapHealthChecks("health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.UseHttpsRedirection();

app.UseSerilogRequestLogging();

app.UseExceptionHandler();

app.UseAuthentication();

app.UseAuthorization();

await app.RunAsync();
