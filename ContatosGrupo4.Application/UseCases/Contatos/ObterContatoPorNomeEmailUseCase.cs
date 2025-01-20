using ContatosGrupo4.Domain.Entities;
using ContatosGrupo4.Domain.Interfaces;

namespace ContatosGrupo4.Application.UseCases.Contatos
{
    public class ObterContatoPorNomeEmailUseCase (IContatoRepository contatoRepository)
    {
        private readonly IContatoRepository _contatoRepository = contatoRepository;

        public async Task<Contato?> ExecuteAsync(string nome, string email)
        {
            var contato = await _contatoRepository.ObterPorNomeEmailAsync(nome, email);
            return contato;
        }
    }
}