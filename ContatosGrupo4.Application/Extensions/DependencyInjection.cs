﻿using System.Diagnostics.CodeAnalysis;
using ContatosGrupo4.Application.UseCases.Contatos;
using Microsoft.Extensions.DependencyInjection;

namespace ContatosGrupo4.Application.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class DependencyInjection
    {
        public static IServiceCollection AddUseCases(this IServiceCollection services)
        {
            services.AddScoped<AtualizarContatoUseCase>();
            services.AddScoped<CriarContatoUseCase>();
            services.AddScoped<ExcluirContatoUseCase>();
            services.AddScoped<ObterContatoPorIdUseCase>();
            services.AddScoped<ObterContatoPorNomeEmailUseCase>();
            services.AddScoped<ObterTodosContatosUseCase>();
            services.AddScoped<ObterContatosPorDddUseCase>();

            return services;
        }
    }
}
