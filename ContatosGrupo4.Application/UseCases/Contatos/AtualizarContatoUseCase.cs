using ContatosGrupo4.Application.DTOs;
using ContatosGrupo4.Domain.Entities;
using ContatosGrupo4.Domain.Interfaces;

namespace ContatosGrupo4.Application.UseCases.Contatos;

public class AtualizarContatoUseCase (IContatoRepository contatoRepository, ObterContatoPorIdUseCase contatoPorIdUseCase)
{
    private readonly IContatoRepository _contatoRepository = contatoRepository;
    private readonly ObterContatoPorIdUseCase _contatoPorIdUseCase = contatoPorIdUseCase;

    public async Task<Contato> ExecuteAsync(AtualizarContatoDto atualizarContato)
    {
        try
        {
            var contato = await _contatoPorIdUseCase.ExecuteAsync(atualizarContato.Id);

            contato!.Nome = atualizarContato.Nome;
            contato.Telefone = atualizarContato.Telefone;
            contato.Email = atualizarContato.Email;
            contato.CodigoArea = atualizarContato.CodigoArea;
            contato.SetDataAtualizacao();

            await _contatoRepository.PutContato(contato);
            return contato;
        }
        catch (Exception)
        {
            throw;
        }
    }
}