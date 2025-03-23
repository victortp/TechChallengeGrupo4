using Prometheus;
using System.Diagnostics.CodeAnalysis;

namespace ContatosGrupo4.Api.Extensions;

[ExcludeFromCodeCoverage]
public static class DependencyInjection
{
    public static IServiceCollection AddPrometheus(this IServiceCollection services)
    {
        services.AddSingleton<CollectorRegistry>(new CollectorRegistry());
        services.AddMetrics();

        return services;
    }

    public static IApplicationBuilder UsePrometheus(this IApplicationBuilder app)
    {
        app.UseHttpMetrics();
        app.UseMetricServer();

        return app;
    }
}