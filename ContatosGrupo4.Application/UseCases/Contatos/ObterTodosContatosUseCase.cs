using ContatosGrupo4.Domain.Entities;
using ContatosGrupo4.Domain.Interfaces;

namespace ContatosGrupo4.Application.UseCases.Contatos;

public class ObterTodosContatosUseCase (IContatoRepository contatoRepository)
{
    private readonly IContatoRepository _contatoRepository = contatoRepository;

    public async Task<IEnumerable<Contato>> ExecuteAsync()
    {
        return await _contatoRepository.ObterTodosAsync();
    }
}