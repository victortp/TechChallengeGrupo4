﻿using ContatosGrupo4.Application.DTOs;
using ContatosGrupo4.Application.UseCases.Contatos;
using ContatosGrupo4.Domain.Interfaces;
using ContatosGrupo4.Domain.Entities;
using Moq;
using FluentAssertions;
using ContatosGrupo4.Application.UseCases.Usuarios;

namespace ContatosGrupo4.Tests.Application.UseCases.Contatos;

public class CriarContatoUseCaseTests
{
    [Fact]
    public static async Task DeveCriarContatoQuandoDtoForValido()
    {
        var contatoRepository = new Mock<IContatoRepository>();
        var usuarioRepository = new Mock<IUsuarioRepository>();
        var obterContatoPorNomeEmail = new ObterContatoPorNomeEmailUseCase(contatoRepository.Object);
        var obterUsuarioPorIdUseCase = new ObterUsuarioPorIdUseCase(usuarioRepository.Object);
        var contatoUseCase = new CriarContatoUseCase(contatoRepository.Object, obterContatoPorNomeEmail, obterUsuarioPorIdUseCase);
        var dto = new CriarContatoDto() { Nome = "testeContato", Email = "testeemail@google.com", Telefone = "3299999-9999", UsuarioId = 1 };
        var contatoEsperado = new Contato { Nome = dto.Nome, Email = dto.Email, Telefone = dto.Telefone, UsuarioId = dto.UsuarioId };
        contatoRepository
            .Setup(r => r.AdicionarAsync(It.IsAny<Contato>()))
            .Returns(Task.CompletedTask);
        usuarioRepository
            .Setup(r => r.ObterPorIdAsync(It.IsAny<int>()))
            .ReturnsAsync(new Usuario() { });
        
        var contatoCriado = await contatoUseCase.ExecuteAsync(dto);

        contatoCriado.Nome.Should().Be(contatoEsperado.Nome);
        contatoCriado.Email.Should().Be(contatoEsperado.Email);
        contatoCriado.Telefone.Should().Be(contatoEsperado.Telefone);
        contatoCriado.UsuarioId.Should().Be(contatoEsperado.UsuarioId);
        contatoRepository.Verify(repo => repo.AdicionarAsync(It.Is<Contato>(u =>
            u.Nome == contatoEsperado.Nome &&
            u.Email == contatoEsperado.Email &&
            u.Telefone == contatoEsperado.Telefone &&
            u.UsuarioId == contatoEsperado.UsuarioId)), Times.Once);
    }
    
    [Theory]
    [InlineData(null, "testeemail@google.com", "3299999-9999", "Nome (Parameter 'O Nome não pode ser vazio.')")]
    [InlineData("testeContato", null, "3299999-9999", "Email (Parameter 'E-mail não informado ou inválido.')")]
    [InlineData("testeContato", "testeemail@google.com", null, "Telefone (Parameter 'Telefone não informado ou inválido.')")]
    public static async Task DeveLancarExcecaoQuandoDtoForInvalido(string? nome, string? email, string? telefone, string mensagem)
    {
        var contatoRepository = new Mock<IContatoRepository>();
        var usuarioRepository = new Mock<IUsuarioRepository>();
        var obterContatoPorNomeEmail = new ObterContatoPorNomeEmailUseCase(contatoRepository.Object);
        var obterUsuarioPorIdUseCase = new ObterUsuarioPorIdUseCase(usuarioRepository.Object);
        var contatoUseCase = new CriarContatoUseCase(contatoRepository.Object, obterContatoPorNomeEmail, obterUsuarioPorIdUseCase);
        var dto = new CriarContatoDto() { Nome = nome!, Email = email!, Telefone = telefone!};

        usuarioRepository
            .Setup(r => r.ObterPorIdAsync(It.IsAny<int>()))
            .ReturnsAsync(new Usuario() { });

        var act = async () => { await contatoUseCase.ExecuteAsync(dto); };

        await act.Should().ThrowAsync<ArgumentNullException>().WithMessage(mensagem);
    }

    [Fact]
    public static async Task DeveLancarExcecaoQuandoContatoJaExistir()
    {
        var contatoRepository = new Mock<IContatoRepository>();
        var usuarioRepository = new Mock<IUsuarioRepository>();
        var obterContatoPorNomeEmail = new ObterContatoPorNomeEmailUseCase(contatoRepository.Object);
        var obterUsuarioPorIdUseCase = new ObterUsuarioPorIdUseCase(usuarioRepository.Object);
        var contatoUseCase = new CriarContatoUseCase(contatoRepository.Object, obterContatoPorNomeEmail, obterUsuarioPorIdUseCase);
        var dto = new CriarContatoDto() { Nome = "testeContato", Email = "testeemail@google.com", Telefone = "3299999-9999", UsuarioId = 1 };
        var contatoEsperado = new Contato { Nome = dto.Nome, Email = dto.Email, Telefone = dto.Telefone, UsuarioId = dto.UsuarioId };

        contatoRepository
            .Setup(r => r.ObterPorNomeEmailAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(contatoEsperado);
        usuarioRepository
            .Setup(r => r.ObterPorIdAsync(It.IsAny<int>()))
            .ReturnsAsync(new Usuario() { });

        var act = async () => { await contatoUseCase.ExecuteAsync(dto); };

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("O Nome/E-mail informados já estão em uso.");
    }

    [Fact]
    public static async Task DeveLancarExcecaoQuandoHouverErroDeBanco()
    {
        var contatoRepository = new Mock<IContatoRepository>();
        var usuarioRepository = new Mock<IUsuarioRepository>();
        var obterContatoPorNomeEmail = new ObterContatoPorNomeEmailUseCase(contatoRepository.Object);
        var obterUsuarioPorIdUseCase = new ObterUsuarioPorIdUseCase(usuarioRepository.Object);
        var contatoUseCase = new CriarContatoUseCase(contatoRepository.Object, obterContatoPorNomeEmail, obterUsuarioPorIdUseCase);
        var dto = new CriarContatoDto() { Nome = "testeContato", Email = "testeemail@google.com", Telefone = "3299999-9999", UsuarioId = 1 };

        contatoRepository
            .Setup(r => r.AdicionarAsync(It.IsAny<Contato>()))
            .ThrowsAsync(new Exception());

        var act = async () => { await contatoUseCase.ExecuteAsync(dto); };

        await act.Should().ThrowAsync<Exception>();
    }
}