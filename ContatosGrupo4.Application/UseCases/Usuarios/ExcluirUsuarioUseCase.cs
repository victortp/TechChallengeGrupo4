using ContatosGrupo4.Domain.Interfaces;

namespace ContatosGrupo4.Application.UseCases.Usuarios
{
    public class ExcluirUsuarioUseCase
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public ExcluirUsuarioUseCase(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        public async Task ExecuteAsync(int id)
        {
            try
            {
                await _usuarioRepository.ExcluirAsync(id);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
