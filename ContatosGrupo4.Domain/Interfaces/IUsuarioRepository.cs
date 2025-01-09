using ContatosGrupo4.Domain.Entities;

namespace ContatosGrupo4.Domain.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<IEnumerable<Usuario>> ObterTodosAsync();
        Task<Usuario?> ObterPorIdAsync(int id);
        Task<Usuario?> ObterPorLoginAsync(string login);
        Task AdicionarAsync(Usuario usuario);
        Task AtualizarAsync(Usuario usuario);
        Task ExcluirAsync(int id);
    }
}
