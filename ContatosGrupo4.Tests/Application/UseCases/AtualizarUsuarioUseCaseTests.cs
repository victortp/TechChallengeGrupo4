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
            var useCase = new AtualizaUsuarioUseCase(usuarioRepository.Object, obterUsuarioPorIdUSeCase);
            var dto = new AtualizarUsuarioDto() { Id = 1 };

            usuarioRepository
                .Setup(r => r.ObterPorIdAsync(It.IsAny<int>()))
                .ReturnsAsync(default(Usuario));

            var act = async () => await useCase.ExecuteAsync(dto);

            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("Usuário com ID 1 não encontrado.");
        }
    }
}
