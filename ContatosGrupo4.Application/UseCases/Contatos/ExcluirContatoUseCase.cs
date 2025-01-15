using ContatosGrupo4.Domain.Interfaces;

namespace ContatosGrupo4.Application.UseCases.Contatos;

public class ExcluirContatoUseCase (IContatoRepository contatoRepository)
{
    private readonly IContatoRepository _contatoRepository = contatoRepository;

    public async Task ExecuteAsync (int idContato)
    {
        await _contatoRepository.DeleteContatos(idContato);
    }
}