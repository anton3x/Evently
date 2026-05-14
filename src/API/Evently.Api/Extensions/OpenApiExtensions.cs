using Scalar.AspNetCore;

namespace Evently.Api.Extensions;

internal static class OpenApiExtensions
{
    internal static void MapOpenApiWithScalar(this IEndpointRouteBuilder app)
    {
        app.MapOpenApi();
        app.MapScalarApiReference("docs", options =>
        {
            options.Title = "Evently Api Documentation";
            options.Layout = ScalarLayout.Classic;
            options.Theme = ScalarTheme.Solarized;
            options.Metadata = new Dictionary<string, string>
            {
                { "version", "1.0" },
                { "description", "This is the Evently Api Documentation" }
            };
            options.DarkMode = false;
        });
    }
}
