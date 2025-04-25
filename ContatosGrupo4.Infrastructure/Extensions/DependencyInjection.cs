using System.Diagnostics.CodeAnalysis;
using ContatosGrupo4.Application.Configurations;
using ContatosGrupo4.Application.Interfaces;
using ContatosGrupo4.Domain.Interfaces;
using ContatosGrupo4.Infrastructure.Data.Contexts;
using ContatosGrupo4.Infrastructure.Data.Repositories;
using ContatosGrupo4.Infrastructure.Messaging.Producers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace ContatosGrupo4.InfraStructure.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        { 
            services.AddDbContext<AppDbContext>(c =>
            {
                c.UseLazyLoadingProxies();
                c.UseSqlServer(configuration.GetValue<string>("SqlServer:ConnectionString"));
            }, ServiceLifetime.Scoped);

            services.AddScoped<IContatoRepository, ContatoRepository>();
            services.AddMemoryCache();

            services.AddSingleton<IConnectionFactory>(sp => {
                var rabbitConfig = sp.GetRequiredService<IOptions<RabbitMQOptions>>().Value;
                var factory = new ConnectionFactory()
                {
                    HostName = rabbitConfig.HostName,
                    UserName = rabbitConfig.UserName,
                    Password = rabbitConfig.Password
                };
                return factory;
            });

            services.AddScoped<IMessagePublisher, RabbitMQPublisher>();

            return services;
        }
    }
}