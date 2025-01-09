using ContatosGrupo4.Domain.Entities;
using ContatosGrupo4.Domain.Interfaces;

namespace ContatosGrupo4.Application.UseCases
{
    public class ObterUsuarioPorIdUseCase
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public ObterUsuarioPorIdUseCase(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        public async Task<Usuario?> ExecuteAsync(int id)
        {
            var usuario = await _usuarioRepository.ObterPorIdAsync(id);
            return usuario ?? throw new ArgumentException($"Usuário com ID {id} não encontrado.");
        }
    }
}
