﻿using System.Diagnostics.CodeAnalysis;
using ContatosGrupo4.Domain.Interfaces;
using ContatosGrupo4.Infrastructure.Data.Contexts;
using ContatosGrupo4.Infrastructure.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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

            return services;
        }
    }
}