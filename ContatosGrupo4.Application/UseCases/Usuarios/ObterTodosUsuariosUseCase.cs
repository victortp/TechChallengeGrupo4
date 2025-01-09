using ContatosGrupo4.Domain.Entities;
using ContatosGrupo4.Domain.Interfaces;

namespace ContatosGrupo4.Application.UseCases.Usuarios
{
    public class ObterTodosUsuariosUseCase
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public ObterTodosUsuariosUseCase(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        public async Task<IEnumerable<Usuario>> ExecuteAsync()
        {
            return await _usuarioRepository.ObterTodosAsync();
        }
    }
}
