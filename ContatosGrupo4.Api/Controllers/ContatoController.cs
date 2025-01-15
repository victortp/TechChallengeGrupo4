using ContatosGrupo4.Application.DTOs;
using ContatosGrupo4.Application.UseCases.Contatos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace ContatosGrupo4.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ContatoController(ObterTodosContatosUseCase obterTodosContatosUseCase
                                , ObterContatoPorIdUseCase obterContatoPorIdUseCase
                                , CriarContatoUseCase criarContatoUseCase
                                , AtualizarContatoUseCase atualizarContatoUseCase
                                , ExcluirContatoUseCase excluirContatoUseCase
                                , IMemoryCache memoryCache) : ControllerBase
{
    private readonly ObterTodosContatosUseCase _obterTodosContatosUseCase = obterTodosContatosUseCase;
    private readonly ObterContatoPorIdUseCase _obterContatoPorIdUseCase = obterContatoPorIdUseCase;
    private readonly CriarContatoUseCase _criarContatoUseCase = criarContatoUseCase;
    private readonly AtualizarContatoUseCase _atualizarContatoUseCase = atualizarContatoUseCase;
    private readonly ExcluirContatoUseCase _excluirContatoUseCase = excluirContatoUseCase;
    private readonly IMemoryCache _memoryCache = memoryCache;

    [HttpGet]
    public async Task<IActionResult> ObterTodosContatos()
    {
        const string cacheKey = "TodosContratos";

        if (!_memoryCache.TryGetValue(cacheKey, out var contatos))
        {
            contatos = await _obterTodosContatosUseCase.ExecuteAsync();
            var cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
                SlidingExpiration = TimeSpan.FromMinutes(2)
            };
            _memoryCache.Set(cacheKey, contatos, cacheEntryOptions);
        }
        return Ok(contatos);
    }

    [HttpGet("{idContato}")]
    public async Task<IActionResult> ObterContatoPorId(int idContato)
    {
        var cacheKey = $"Contato_{idContato}";

        if (!_memoryCache.TryGetValue(cacheKey, out var contato))
        {
            contato = await _obterContatoPorIdUseCase.ExecuteAsync(idContato);

            if (contato == null) return NotFound($"Contato {idContato} não encontrado");

            var cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
            };
            _memoryCache.Set(cacheKey, contato, cacheEntryOptions);
        }
        return Ok(contato);
    }

    [HttpPost]
    public async Task<IActionResult> CriarContato([FromBody] CriarContatoDto contatoCriarDto)
    {
        try
        {
            if (contatoCriarDto == null) return BadRequest("Dados inválidos");
            var contato = await _criarContatoUseCase.ExecuteAsync(contatoCriarDto);
            _memoryCache.Remove("TodosContratos");
            return CreatedAtAction(nameof(CriarContato), new { id = contato.Id }, contato);
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
            return StatusCode(500, $"Erro interno: {ex.Message}");
        }
    }

    [HttpPut("{idContato}")]
    public async Task<IActionResult> AtualizarContato(int idContato, [FromBody] AtualizarContatoDto contatoAtualizarDto)
    {
        try
        {
            if (contatoAtualizarDto == null || idContato != contatoAtualizarDto.Id) return BadRequest("Dados inválidos");
            var contato = await _atualizarContatoUseCase.ExecuteAsync(contatoAtualizarDto);
            _memoryCache.Remove("TodosContratos");
            _memoryCache.Remove($"Contato_{idContato}");
            return Ok(contato);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro interno: {ex.Message}");
        }
    }

    [HttpDelete("{idContato}")]
    public async Task<IActionResult> ExcluirContato(int idContato)
    {
        try
        {
            await _excluirContatoUseCase.ExecuteAsync(idContato);
            _memoryCache.Remove("TodosContratos");
            _memoryCache.Remove($"Contato_{idContato}");
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro interno: {ex.Message}");
        }
    }
}