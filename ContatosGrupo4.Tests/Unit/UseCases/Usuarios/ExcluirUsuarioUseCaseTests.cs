using ContatosGrupo4.Application.DTOs;
using ContatosGrupo4.Application.UseCases.Usuarios;
using ContatosGrupo4.Domain.Entities;
using ContatosGrupo4.Domain.Interfaces;
using Moq;

namespace ContatosGrupo4.Tests.Application.UseCases.Usuarios
{
    public class ExcluirUsuarioUseCaseTests
    {
        [Fact]
        public async Task Deve_Excluir_Usuario()
        {
            var usuarioRepository = new Mock<IUsuarioRepository>();
            var useCase = new ExcluirUsuarioUseCase(usuarioRepository.Object);
            var usuario = new Usuario() { Id = 1, Login = "Login", Senha = "Senha" };

            usuarioRepository
                .Setup(r => r.ExcluirAsync(It.IsAny<int>()))
                .Returns(Task.CompletedTask);

            await useCase.ExecuteAsync(1);

            usuarioRepository.Verify(r => r.ExcluirAsync(It.Is<int>(id => id == usuario.Id)), Times.Once);
        }
    }
}
