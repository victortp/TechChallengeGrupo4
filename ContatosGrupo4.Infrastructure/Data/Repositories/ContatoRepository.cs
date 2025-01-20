using System.Diagnostics.CodeAnalysis;
using ContatosGrupo4.Domain.Entities;
using ContatosGrupo4.Domain.Interfaces;
using ContatosGrupo4.Infrastructure.Data.Contexts;
using Microsoft.EntityFrameworkCore;

namespace ContatosGrupo4.Infrastructure.Data.Repositories;

[ExcludeFromCodeCoverage]
public class ContatoRepository (AppDbContext appDbContext) : IContatoRepository
{
    readonly AppDbContext _appDbContext = appDbContext;

    public async Task<IEnumerable<Contato>> ObterTodosAsync()
    {
        return await _appDbContext.Contato.ToListAsync();
    }

    public async Task<IEnumerable<Contato>> ObterPorDddsAsync(int codigoArea)
    {
        return await _appDbContext.Contato.Where(c => EF.Functions.Like(c.Telefone, $"{codigoArea}%")).ToListAsync();
    }

    public async Task<Contato?> ObterPorNomeEmailAsync(string nome, string email)
    {
        return await _appDbContext.Contato.Where(c => c.Nome == nome || c.Email == email).FirstOrDefaultAsync();
    }

    public async Task<Contato?> ObterPorIdAsync(int idContato)
    {
        return await _appDbContext.Contato.FindAsync(idContato);
    }

    public async Task AdicionarAsync(Contato contato)
    {
        await _appDbContext.AddAsync(contato);
        await _appDbContext.SaveChangesAsync();
    }

    public async Task AtualizarAsync(Contato contato)
    {
        _appDbContext.Contato.Update(contato);
        await _appDbContext.SaveChangesAsync();
    }

    public async Task ExcluirAsync(int idContato)
    {
        var contato = await this.ObterPorIdAsync(idContato);

        if (contato != null)
        {
            _appDbContext.Contato.Remove(contato);
            await _appDbContext.SaveChangesAsync();
        }
    }
}