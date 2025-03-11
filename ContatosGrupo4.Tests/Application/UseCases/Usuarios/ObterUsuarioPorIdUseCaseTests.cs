using ContatosGrupo4.Application.UseCases.Usuarios;
using ContatosGrupo4.Domain.Entities;
using ContatosGrupo4.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace ContatosGrupo4.Tests.Application.UseCases.Usuarios
{
    public class ObterUsuarioPorIdUseCaseTests
    {
        [Fact]
        public async Task Deve_Retornar_Usuario_Quando_Id_Existir()
        {
            var usuarioRepository = new Mock<IUsuarioRepository>();
            var useCase = new ObterUsuarioPorIdUseCase(usuarioRepository.Object);
            var usuarioEsperado = new Usuario() { Id = 1, Login = "Loginteste", Senha = "SenhaTeste" };

            usuarioRepository
                .Setup(r => r.ObterPorIdAsync(It.IsAny<int>()))
                .ReturnsAsync(usuarioEsperado);

            var usuario = await useCase.ExecuteAsync(1);

            usuario.Should().NotBeNull();
            usuario!.Id.Should().Be(1);
            usuario!.Login.Should().Be(usuarioEsperado.Login);
            usuario!.Senha.Should().Be(usuarioEsperado.Senha);
        }

        [Fact]
        public async Task Deve_Lancar_Excecao_Quando_Id_Nao_Existir()
        {
            var usuarioRepository = new Mock<IUsuarioRepository>();
            var useCase = new ObterUsuarioPorIdUseCase(usuarioRepository.Object);

            usuarioRepository
                .Setup(r => r.ObterPorIdAsync(It.IsAny<int>()))
                .ReturnsAsync(default(Usuario));

            var act = async () => await useCase.ExecuteAsync(1);

            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("Usuário com ID 1 não encontrado.");
        }
    }
}
