using System.Diagnostics.CodeAnalysis;
using ContatosGrupo4.Domain.Entities;
using ContatosGrupo4.Domain.Interfaces;
using ContatosGrupo4.Infrastructure.Data.Contexts;
using Microsoft.EntityFrameworkCore;

namespace ContatosGrupo4.Infrastructure.Data.Repositories
{
    [ExcludeFromCodeCoverage]
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly AppDbContext _context;

        public UsuarioRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AdicionarAsync(Usuario usuario)
        {
            await _context.Usuario.AddAsync(usuario);
            await _context.SaveChangesAsync();
        }

        public async Task AtualizarAsync(Usuario usuario)
        {
            _context.Usuario.Update(usuario);
            await _context.SaveChangesAsync();
        }

        public async Task ExcluirAsync(int id)
        {
            var usuario = await ObterPorIdAsync(id);
            if (usuario != null)
            {
                _context.Usuario.Remove(usuario);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Usuario?> ObterPorIdAsync(int id)
        {
            return await _context.Usuario.FindAsync(id);
        }

        public async Task<Usuario?> ObterPorLoginAsync(string login)
        {
            return await _context.Usuario.FirstOrDefaultAsync(u => u.Login == login);
        }

        public async Task<IEnumerable<Usuario>> ObterTodosAsync()
        {
            return await _context.Usuario.ToListAsync();
        }
    }
}
