using ContatosGrupo4.Application.DTOs;
using ContatosGrupo4.Domain.Entities;
using ContatosGrupo4.Domain.Interfaces;

namespace ContatosGrupo4.Application.UseCases.Contatos;

public class CriarContatoUseCase (IContatoRepository contatoRepository, ObterContatoPorNomeEmailUseCase obterContatoPorNomeEmail)
{
    private readonly IContatoRepository _contatoRepository = contatoRepository;
    private readonly ObterContatoPorNomeEmailUseCase _obterContatoPorNomeEmail = obterContatoPorNomeEmail;
    public async Task<Contato> ExecuteAsync (CriarContatoDto contatoDto)
    {
        try
        {
            if (string.IsNullOrEmpty(contatoDto.Nome))
            {
                throw new ArgumentNullException("O Nome não pode ser vazio.", nameof(contatoDto.Nome));
            }
            else if (string.IsNullOrEmpty(contatoDto.Telefone))
            {
                throw new ArgumentNullException("O Telefone não pode ser vazio.", nameof(contatoDto.Telefone));
            }
            else if (string.IsNullOrEmpty(contatoDto.Email))
            {
                throw new ArgumentNullException("O E-mail não pode ser vazio", nameof(contatoDto.Email));
            }
            else if (contatoDto.CodigoArea == 0)
            {
                throw new ArgumentNullException("O Código da Área deve ser informado.", nameof(contatoDto.CodigoArea));
            }

            var contatoExistente = await _obterContatoPorNomeEmail.ExecuteAsync(contatoDto.Nome, contatoDto.Email);
            if (contatoExistente != null)
            {
                throw new InvalidOperationException("O Nome/E-mail informados já estão em uso.");
            }

            var contato = new Contato()
            {
                Nome = contatoDto.Nome,
                Telefone = contatoDto.Telefone,
                Email = contatoDto.Email,
                CodigoArea = contatoDto.CodigoArea
            };
            contato.SetDataCriacao();

            await _contatoRepository.PostContatos(contato);
            return contato;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }
}