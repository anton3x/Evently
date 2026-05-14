using Evently.Api.Extensions;
using Evently.Common.Application;
using Evently.Common.Infrastructure;
using Evently.Modules.Events.Infrastructure;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddApplication([Evently.Modules.Events.Application.AssemblyReference.Assembly]);
builder.Services.AddInfrastructure(builder.Configuration.GetConnectionString("Database")!);
builder.Configuration.AddModuleConfiguration(["events"]);
builder.Services.AddEventsModule(builder.Configuration);

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApiWithScalar();
    app.ApplyMigrations();
}

app.MapGet("/", () => $"Welcome to Evently API ({(app.Environment.IsDevelopment() ? "Dev" : "Prod")})");

EventsModule.MapEndpoints(app);

app.UseHttpsRedirection();

await app.RunAsync();
