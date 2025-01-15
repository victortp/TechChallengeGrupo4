using ContatosGrupo4.Application.DTOs;
using ContatosGrupo4.Application.UseCases.Usuarios;
using ContatosGrupo4.Domain.Entities;
using ContatosGrupo4.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace ContatosGrupo4.Tests.Application.UseCases
{
    public class AtualizarUsuarioUseCaseTests
    {
        [Fact]
        public async Task Deve_Lancar_Excecao_Quando_Usuario_Nao_Existir()
        {
            var usuarioRepository = new Mock<IUsuarioRepository>();
            var obterUsuarioPorIdUSeCase = new ObterUsuarioPorIdUseCase(usuarioRepository.Object);
            var useCase = new AtualizarUsuarioUseCase(usuarioRepository.Object, obterUsuarioPorIdUSeCase);
            var dto = new AtualizarUsuarioDto() { Id = 1 };

            usuarioRepository
                .Setup(r => r.ObterPorIdAsync(It.IsAny<int>()))
                .ReturnsAsync(default(Usuario));

            var act = async () => await useCase.ExecuteAsync(dto);

            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("Usuário com ID 1 não encontrado.");
        }

        [Fact]
        public async Task Deve_Atualizar_E_Retornar_Usuario()
        {
            var usuarioRepository = new Mock<IUsuarioRepository>();
            var obterUsuarioPorIdUSeCase = new ObterUsuarioPorIdUseCase(usuarioRepository.Object);
            var useCase = new AtualizarUsuarioUseCase(usuarioRepository.Object, obterUsuarioPorIdUSeCase);
            var dto = new AtualizarUsuarioDto() { Id = 1, Login = "LoginAtualizado", Senha = "SenhaAtualizada" };
            var usuarioEsperado = new Usuario() { Id = dto.Id, Login = dto.Login, Senha = dto.Senha };

            usuarioRepository
                .Setup(r => r.ObterPorIdAsync(It.IsAny<int>()))
                .ReturnsAsync(usuarioEsperado);
            usuarioRepository
                .Setup(r => r.AtualizarAsync(It.IsAny<Usuario>()))
                .Returns(Task.CompletedTask);

            var usuario = await useCase.ExecuteAsync(dto);

            usuario.Id.Should().Be(1);
            usuario.Login.Should().Be(usuarioEsperado.Login);
            usuario.Senha.Should().Be(usuarioEsperado.Senha);
            usuarioRepository.Verify(r => r.AtualizarAsync(It.Is<Usuario>(u =>
                u.Id == dto.Id &&
                u.Login == dto.Login &&
                u.Senha == dto.Senha)), Times.Once);
        }
    }
}
