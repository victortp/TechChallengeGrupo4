using ContatosGrupo4.Application.UseCases.Usuarios;
using Microsoft.Extensions.DependencyInjection;

namespace ContatosGrupo4.Application.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddUseCases(this IServiceCollection services)
        {
            services.AddScoped<CriarUsuarioUseCase>();
            services.AddScoped<ObterTodosUsuariosUseCase>();
            services.AddScoped<ObterUsuarioPorIdUseCase>();
            services.AddScoped<AtualizaUsuarioUseCase>();
            services.AddScoped<ExcluirUsuarioUseCase> ();

            return services;
        }
    }
}
