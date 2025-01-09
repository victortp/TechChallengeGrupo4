using ContatosGrupo4.Domain.Interfaces;

namespace ContatosGrupo4.Application.UseCases.Usuarios
{
    public class ExcluirUsuarioUseCase
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ObterUsuarioPorIdUseCase _obterUsuarioPorIdUseCase;

        public ExcluirUsuarioUseCase(
            IUsuarioRepository usuarioRepository,
            ObterUsuarioPorIdUseCase obterUsuarioPorIdUseCase)
        {
            _usuarioRepository = usuarioRepository;
            _obterUsuarioPorIdUseCase = obterUsuarioPorIdUseCase;
        }

        public async Task ExecuteAsync(int id)
        {
            try
            {
                var usuarioExistente = await _obterUsuarioPorIdUseCase.ExecuteAsync(id);
                await _usuarioRepository.ExcluirAsync(id);
            }
            catch (ArgumentException)
            {
                throw;
            }
        }
    }
}
