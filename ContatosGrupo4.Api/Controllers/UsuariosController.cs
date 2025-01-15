using ContatosGrupo4.Application.DTOs;
using ContatosGrupo4.Application.UseCases.Usuarios;
using ContatosGrupo4.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ContatosGrupo4.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly CriarUsuarioUseCase _criarUsuarioUseCase;
        private readonly ObterTodosUsuariosUseCase _obterTodosUsuariosUseCase;
        private readonly ObterUsuarioPorIdUseCase _obterUsuarioPorIdUseCase;
        private readonly AtualizarUsuarioUseCase _atualizaUsuarioUseCase;
        private readonly ExcluirUsuarioUseCase _excluirUsuarioUseCase;
        private readonly IMemoryCache _memoryCache;

        public UsuariosController(
            CriarUsuarioUseCase criarUsuarioUseCase,
            ObterTodosUsuariosUseCase obterTodosUsuariosUseCase,
            ObterUsuarioPorIdUseCase obterUsuarioPorIdUseCase,
            AtualizarUsuarioUseCase atualizaUsuarioUseCase,
            ExcluirUsuarioUseCase excluirUsuarioUseCase,
            IMemoryCache memoryCache)
        {
            _criarUsuarioUseCase = criarUsuarioUseCase;
            _obterTodosUsuariosUseCase = obterTodosUsuariosUseCase;
            _obterUsuarioPorIdUseCase = obterUsuarioPorIdUseCase;
            _atualizaUsuarioUseCase = atualizaUsuarioUseCase;
            _excluirUsuarioUseCase = excluirUsuarioUseCase;
            _memoryCache = memoryCache;
        }

        [HttpGet]
        public async Task<IActionResult> ObterTodosUsuarios()
        {
            const string cacheKey = "TodosUsuarios";

            if (!_memoryCache.TryGetValue(cacheKey, out var usuarios))
            {
                usuarios = await _obterTodosUsuariosUseCase.ExecuteAsync();

                var cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
                    SlidingExpiration = TimeSpan.FromMinutes(2)
                };
                _memoryCache.Set(cacheKey, usuarios, cacheEntryOptions);
            }

            return Ok(usuarios);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObterUsuarioPorId(int id)
        {
            try
            {
                var cacheKey = $"Usuario_{id}";

                if (!_memoryCache.TryGetValue(cacheKey, out var usuario))
                {
                    usuario = await _obterUsuarioPorIdUseCase.ExecuteAsync(id);
                    if (usuario == null) return NotFound("Usuário não encontrado");

                    var cacheEntryOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                    };

                    _memoryCache.Set(cacheKey, usuario, cacheEntryOptions);
                }
                return Ok(usuario);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CriarUsuario([FromBody] CriarUsuarioDto usuarioCriarDto)
        {
            if (usuarioCriarDto == null)
            {
                return BadRequest("Dados inválidos");
            }

            try
            {
                var usuario = await _criarUsuarioUseCase.ExecuteAsync(usuarioCriarDto);
                _memoryCache.Remove("TodosUsuarios");
                return CreatedAtAction(nameof(CriarUsuario), new { id = usuario.Id }, usuario);
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

        [HttpPut("{id}")]
        public async Task<IActionResult> AtualizarUsuario(int id, [FromBody] AtualizarUsuarioDto usuarioAtualizarDto)
        {
            if (usuarioAtualizarDto == null || id != usuarioAtualizarDto.Id)
            {
                return BadRequest("Dados inválidos");
            }

            try
            {
                var usuario = await _atualizaUsuarioUseCase.ExecuteAsync(usuarioAtualizarDto);
                _memoryCache.Remove("TodosUsuarios");
                _memoryCache.Remove($"Usuario_{id}");
                return Ok(usuario);
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> ExcluirUsuario(int id)
        {
            try
            {
                await _excluirUsuarioUseCase.ExecuteAsync(id);
                _memoryCache.Remove("TodosUsuarios");
                _memoryCache.Remove($"Usuario_{id}");
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
}
