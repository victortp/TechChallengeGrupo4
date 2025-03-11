using ContatosGrupo4.Domain.Entities;
using ContatosGrupo4.Domain.Interfaces;

namespace ContatosGrupo4.Application.UseCases.Contatos;

public class ObterContatoPorIdUseCase (IContatoRepository contatoRepository)
{
    private readonly IContatoRepository _contatoRepository = contatoRepository;

    public async Task<Contato?> ExecuteAsync(int idContato)
    {
        var contato = await _contatoRepository.ObterPorIdAsync(idContato);
        return contato ?? throw new ArgumentException($"Contato com ID {idContato} não encontrado.");
    }
}