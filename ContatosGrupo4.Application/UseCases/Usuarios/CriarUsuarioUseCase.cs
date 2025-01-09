using ContatosGrupo4.Application.DTOs;
using ContatosGrupo4.Domain.Entities;
using ContatosGrupo4.Domain.Interfaces;

namespace ContatosGrupo4.Application.UseCases.Usuarios
{
    public class CriarUsuarioUseCase
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public CriarUsuarioUseCase(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        public async Task<Usuario> ExecuteAsync(UsuarioCriarDto dto)
        {
            if (string.IsNullOrEmpty(dto.Login))
            {
                throw new ArgumentNullException("O login não pode ser vazio.", nameof(dto.Login));
            }

            if (string.IsNullOrEmpty(dto.Senha))
            {
                throw new ArgumentNullException("A senha não pode ser vazia.", nameof(dto.Senha));
            }

            var usuarioExistente = await _usuarioRepository.ObterPorLoginAsync(dto.Login);
            if (usuarioExistente != null)
            {
                throw new InvalidOperationException("O login informado já está em uso.");
            }

            var usuario = new Usuario()
            {
                Login = dto.Login,
                Senha = dto.Senha
            };
            usuario.SetDataCriacao();

            try
            {
                await _usuarioRepository.AdicionarAsync(usuario);
                return usuario;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
    }
}
