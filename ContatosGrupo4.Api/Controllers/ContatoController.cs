using ContatosGrupo4.Application.DTOs;
using ContatosGrupo4.Application.UseCases.Contatos;
using ContatosGrupo4.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace ContatosGrupo4.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ContatoController(
    ObterTodosContatosUseCase obterTodosContatosUseCase,
    ObterContatoPorIdUseCase obterContatoPorIdUseCase,
    CriarContatoUseCase criarContatoUseCase,
    AtualizarContatoUseCase atualizarContatoUseCase,
    ExcluirContatoUseCase excluirContatoUseCase,
    ObterContatosPorDddUseCase obterContatosPorDddUseCase) : ControllerBase
{
    private readonly ObterTodosContatosUseCase _obterTodosContatosUseCase = obterTodosContatosUseCase;
    private readonly ObterContatoPorIdUseCase _obterContatoPorIdUseCase = obterContatoPorIdUseCase;
    private readonly CriarContatoUseCase _criarContatoUseCase = criarContatoUseCase;
    private readonly AtualizarContatoUseCase _atualizarContatoUseCase = atualizarContatoUseCase;
    private readonly ExcluirContatoUseCase _excluirContatoUseCase = excluirContatoUseCase;
    private readonly ObterContatosPorDddUseCase _obterContatosPorDddUseCase = obterContatosPorDddUseCase;

    [HttpGet]
    public async Task<IActionResult> ObterTodosContatos()
    {
        var contatos = await _obterTodosContatosUseCase.ExecuteAsync();
        var contatoDtos = contatos.Select(contato => new ContatoDto
        {
            Id = contato.Id,
            DataCriacao = contato.DataCriacao,
            DataAtualizacao = contato.DataAtualizacao,
            Nome = contato.Nome,
            Telefone = contato.Telefone,
            Email = contato.Email
        }).ToList();

        return Ok(contatoDtos);
    }

    [HttpGet("ddd/{codigo}")]
    public async Task<IActionResult> ObterTodosContatosDDD(int codigo)
    {
        var contatos = await _obterContatosPorDddUseCase.ExecuteAsync(codigo);
        var contatoDtos = contatos.Select(contato => new ContatoDto
        {
            Id = contato.Id,
            DataCriacao = contato.DataCriacao,
            DataAtualizacao = contato.DataAtualizacao,
            Nome = contato.Nome,
            Telefone = contato.Telefone,
            Email = contato.Email
        }).ToList();

        return Ok(contatoDtos);
    }

    [HttpGet("{idContato}")]
    public async Task<IActionResult> ObterContatoPorId(int idContato)
    {
        try
        {
            var contato = await _obterContatoPorIdUseCase.ExecuteAsync(idContato);

            if (contato is null) return NotFound($"Contato {idContato} não encontrado");

            var contatoDto = new ContatoDto
            {
                Id = contato.Id,
                DataCriacao = contato.DataCriacao,
                DataAtualizacao = contato.DataAtualizacao,
                Nome = contato.Nome,
                Telefone = contato.Telefone,
                Email = contato.Email
            };

            return Ok(contatoDto);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro interno ao buscar contato: {ex.Message}");
        }
    }

    [HttpPost]
    public async Task<IActionResult> CriarContato([FromBody] CriarContatoDto contatoCriarDto)
    {
        try
        {
            if (contatoCriarDto == null) return BadRequest("Dados inválidos");

            var contato = await _criarContatoUseCase.ExecuteAsync(contatoCriarDto);

            var contatoDto = new ContatoDto
            {
                Id = contato.Id,
                DataCriacao = contato.DataCriacao,
                DataAtualizacao = contato.DataAtualizacao,
                Nome = contato.Nome,
                Telefone = contato.Telefone,
                Email = contato.Email
            };

            return CreatedAtAction(nameof(ObterContatoPorId), new { idContato = contato.Id }, contatoDto);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro interno ao criar contato: {ex.Message}");
        }
    }

    [HttpPut("{idContato}")]
    public async Task<IActionResult> AtualizarContato(int idContato, [FromBody] AtualizarContatoDto contatoAtualizarDto)
    {
        if (contatoAtualizarDto == null || idContato != contatoAtualizarDto.Id) return BadRequest("Dados inválidos");

        try
        {
            var contato = await _atualizarContatoUseCase.ExecuteAsync(contatoAtualizarDto);

            if (contato == null) return NotFound($"Contato {idContato} não encontrado para atualização.");

            var contatoDto = new ContatoDto
            {
                Id = contato.Id,
                DataCriacao = contato.DataCriacao,
                DataAtualizacao = contato.DataAtualizacao,
                Nome = contato.Nome,
                Telefone = contato.Telefone,
                Email = contato.Email
            };

            return Ok(contatoDto);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro interno ao atualizar contato: {ex.Message}");
        }
    }

    [HttpDelete("{idContato}")]
    public async Task<IActionResult> ExcluirContato(int idContato)
    {
        try
        {
            await _excluirContatoUseCase.ExecuteAsync(idContato);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro interno ao excluir contato: {ex.Message}");
        }
    }
}