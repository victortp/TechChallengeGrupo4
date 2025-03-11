using ContatosGrupo4.Domain.Entities;

namespace ContatosGrupo4.Domain.Interfaces;

public interface IContatoRepository
{
    Task<IEnumerable<Contato>> ObterTodosAsync();

    Task<IEnumerable<Contato>> ObterPorDddsAsync(int codigoArea);

    Task<Contato?> ObterPorNomeEmailAsync(string nome, string email);

    Task<Contato?> ObterPorIdAsync(int idContato);

    Task AtualizarAsync(Contato contato);

    Task AdicionarAsync(Contato contato);

    Task ExcluirAsync(int idContato);
}