using ContatosGrupo4.Application.DTOs;
using ContatosGrupo4.Application.UseCases.Usuarios;
using ContatosGrupo4.Domain.Entities;
using ContatosGrupo4.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace ContatosGrupo4.Tests.Application.UseCases
{
    public class CriarUsuarioUseCaseTests
    {
        [Fact]
        public async Task Deve_Criar_Usuario_Quando_Dto_For_Valido()
        {
            var usuarioRepository = new Mock<IUsuarioRepository>();
            var useCase = new CriarUsuarioUseCase(usuarioRepository.Object);
            var dto = new CriarUsuarioDto() { Login = "teste", Senha = "teste" };
            var usuarioEsperado = new Usuario() { Login = "teste", Senha = "teste" };
            usuarioRepository
                .Setup(r => r.AdicionarAsync(It.IsAny<Usuario>()))
                .Returns(Task.CompletedTask);

            var usuarioCriado = await useCase.ExecuteAsync(dto);

            usuarioCriado.Login.Should().Be(usuarioEsperado.Login);
            usuarioCriado.Senha.Should().Be(usuarioEsperado.Senha);
            usuarioRepository.Verify(repo => repo.AdicionarAsync(It.Is<Usuario>(u =>
                u.Login == "teste" &&
                u.Senha == "teste")), Times.Once);
        }

        [Theory]
        [InlineData(null, "teste", "Login (Parameter 'O login não pode ser vazio.')")]
        [InlineData("teste", null, "Senha (Parameter 'A senha não pode ser vazia.')")]
        public async Task Deve_Lancar_Excecao_Quando_Dto_For_Invalido(string? login, string? senha, string mensagem)
        {
            var usuarioRepository = new Mock<IUsuarioRepository>();
            var useCase = new CriarUsuarioUseCase(usuarioRepository.Object);
            var dto = new CriarUsuarioDto() { Login = login!, Senha = senha! };
            var act = async () => { await useCase.ExecuteAsync(dto); };

            await act.Should().ThrowAsync<ArgumentNullException>()
                .WithMessage(mensagem);
        }

        [Fact]
        public async Task Deve_Lancar_Excecao_Quando_Usuario_Ja_existir()
        {
            var usuarioRepository = new Mock<IUsuarioRepository>();
            var useCase = new CriarUsuarioUseCase(usuarioRepository.Object);
            var dto = new CriarUsuarioDto() { Login = "teste", Senha = "teste" };
            var usuarioEsperado = new Usuario() { Login = "teste", Senha = "teste" };

            usuarioRepository
                .Setup(r => r.ObterPorLoginAsync(It.IsAny<string>()))
                .ReturnsAsync(usuarioEsperado);

            var act = async () => { await useCase.ExecuteAsync(dto); };

            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("O login informado já está em uso.");
        }

        [Fact]
        public async Task Deve_Lancar_Excecao_Quando_Houver_Erro_De_Banco()
        {
            var usuarioRepository = new Mock<IUsuarioRepository>();
            var useCase = new CriarUsuarioUseCase(usuarioRepository.Object);
            var dto = new CriarUsuarioDto() { Login = "teste", Senha = "teste" };
            var usuarioEsperado = new Usuario() { Login = "teste", Senha = "teste" };

            usuarioRepository
                .Setup(r => r.AdicionarAsync(It.IsAny<Usuario>()))
                .ThrowsAsync(new Exception());

            var act = async () => { await useCase.ExecuteAsync(dto); };

            await act.Should().ThrowAsync<Exception>();
        }
    }
}
