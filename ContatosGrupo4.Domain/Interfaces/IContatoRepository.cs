using ContatosGrupo4.Domain.Entities;

namespace ContatosGrupo4.Domain.Interfaces;

public interface IContatoRepository
{
    Task<IEnumerable<Contato>> GetAllContatos();

    Task<IEnumerable<Contato>> GetContatoPorCodigoArea(int codigoArea);

    Task<Contato?> GetContatoPorNomeEmail(string nome, string email);

    Task<Contato?> GetContatoPorId(int idContato);

    Task PutContato(Contato contato);

    Task PostContatos(Contato contato);

    Task DeleteContatos(int idContato);
}