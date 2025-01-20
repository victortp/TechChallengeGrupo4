using ContatosGrupo4.Application.DTOs;
using ContatosGrupo4.Domain.Entities;
using ContatosGrupo4.Domain.Interfaces;

namespace ContatosGrupo4.Application.UseCases.Contatos;

public class ObterTodosContatosUseCase (IContatoRepository contatoRepository)
{
    private readonly IContatoRepository _contatoRepository = contatoRepository;

    public async Task<IEnumerable<ContatoDto>> ExecuteAsync()
    {
        List<ContatoDto> contatoDtos = [];
        var contatos = await _contatoRepository.ObterTodosAsync();

        foreach (var contato in contatos)
        {
            contatoDtos.Add(new ContatoDto
            {
                Id = contato.Id,
                DataCriacao = contato.DataCriacao,
                DataAtualizacao = contato.DataAtualizacao,
                Nome = contato.Nome,
                Telefone = contato.Telefone,
                Email = contato.Email,
                UsuarioId = contato.UsuarioId
            });
        }

        return contatoDtos;
    }
}