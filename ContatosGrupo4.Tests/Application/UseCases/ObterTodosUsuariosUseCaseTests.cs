using ContatosGrupo4.Application.UseCases.Usuarios;
using ContatosGrupo4.Domain.Entities;
using ContatosGrupo4.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace ContatosGrupo4.Tests.Application.UseCases
{
    public class ObterTodosUsuariosUseCaseTests
    {
        [Fact]
        public async Task Deve_Retornar_Todos_Usuarios()
        {
            var usuarioRepository = new Mock<IUsuarioRepository>();
            var useCase = new ObterTodosUsuariosUseCase(usuarioRepository.Object);
            var usuarioEsperado = new Usuario() { Login = "Loginteste", Senha = "SenhaTeste" };

            usuarioRepository
                .Setup(r => r.ObterTodosAsync())
                .ReturnsAsync([usuarioEsperado]);

            var usuarios = await useCase.ExecuteAsync();

            usuarios.Should().HaveCount(1);
            usuarios.FirstOrDefault()?.Login.Should().Be(usuarioEsperado.Login);
            usuarios.FirstOrDefault()?.Senha.Should().Be(usuarioEsperado.Senha);
        }
    }
}
