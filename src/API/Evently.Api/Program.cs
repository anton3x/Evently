using Evently.Api.Extensions;
using Evently.Modules.Events.Infrastructure;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddEventsModule(builder.Configuration);

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.ApplyMigrations();
}

app.MapGet("/", () => app.Environment.IsDevelopment() ? "Development" : "Production");

EventsModule.MapEndpoints(app);

app.UseHttpsRedirection();

await app.RunAsync();
