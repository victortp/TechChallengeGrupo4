using ContatosGrupo4.Application.DTOs;
using ContatosGrupo4.Application.Validations;
using ContatosGrupo4.Domain.Entities;
using ContatosGrupo4.Domain.Interfaces;

namespace ContatosGrupo4.Application.UseCases.Contatos;

public class CriarContatoUseCase (
    IContatoRepository contatoRepository,
    ObterContatoPorNomeEmailUseCase obterContatoPorNomeEmail)
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

            if (!ContatoValidator.ValidarTelefone(contatoDto.Telefone))
            {
                throw new ArgumentNullException("Telefone não informado ou inválido.", nameof(contatoDto.Telefone));
            }

            if (!ContatoValidator.ValidarEmail(contatoDto.Email))
            {
                throw new ArgumentNullException("E-mail não informado ou inválido.", nameof(contatoDto.Email));
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
                Email = contatoDto.Email
            };
            contato.SetDataCriacao();

            await _contatoRepository.AdicionarAsync(contato);

            return contato;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }
}