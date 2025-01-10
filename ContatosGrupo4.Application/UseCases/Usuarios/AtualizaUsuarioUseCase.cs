using ContatosGrupo4.Application.DTOs;
using ContatosGrupo4.Domain.Entities;
using ContatosGrupo4.Domain.Interfaces;

namespace ContatosGrupo4.Application.UseCases.Usuarios
{
    public class AtualizaUsuarioUseCase
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ObterUsuarioPorIdUseCase _obterUsuarioPorIdUseCase;

        public AtualizaUsuarioUseCase(
            IUsuarioRepository usuarioRepository,
            ObterUsuarioPorIdUseCase obterUsuarioPorIdUseCase)
        {
            _usuarioRepository = usuarioRepository;
            _obterUsuarioPorIdUseCase = obterUsuarioPorIdUseCase;
        }

        public async Task<Usuario> ExecuteAsync(AtualizarUsuarioDto usuario)
        {
            try
            {
                var usuarioExistente = await _obterUsuarioPorIdUseCase.ExecuteAsync(usuario.Id);

                usuarioExistente!.Login = usuario.Login;
                usuarioExistente.Senha = usuario.Senha;
                usuarioExistente.SetDataAtualizacao();

                await _usuarioRepository.AtualizarAsync(usuarioExistente);
                return usuarioExistente;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
