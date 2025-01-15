using ContatosGrupo4.Domain.Entities;
using ContatosGrupo4.Domain.Interfaces;
using ContatosGrupo4.Infrastructure.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace ContatosGrupo4.Infrastructure.Data.Repositories;

public class ContatoRepository (AppDbContext appDbContext) : IContatoRepository
{
    readonly AppDbContext _appDbContext = appDbContext;

    public async Task<IEnumerable<Contato>> GetAllContatos()
    {
        return await _appDbContext.Contato.ToListAsync();
    }

    public async Task<IEnumerable<Contato>> GetContatoPorCodigoArea(int codigoArea)
    {
        return await _appDbContext.Contato.Where(c => c.CodigoArea == codigoArea).ToListAsync();
    }

    public async Task<Contato?> GetContatoPorNomeEmail(string nome, string email)
    {
        return await _appDbContext.Contato.Where(c => c.Nome == nome && c.Email == email).FirstOrDefaultAsync();
    }

    public async Task<Contato?> GetContatoPorId(int idContato)
    {
        return await _appDbContext.Contato.FindAsync(idContato);
    }

    public async Task PostContatos(Contato contato)
    {
        await _appDbContext.AddAsync(contato);
        await _appDbContext.SaveChangesAsync();
    }

    public async Task PutContato(Contato contato)
    {
        _appDbContext.Contato.Update(contato);
        await _appDbContext.SaveChangesAsync();
    }

    public async Task DeleteContatos(int idContato)
    {
        var contato = await this.GetContatoPorId(idContato);

        if (contato != null)
        {
            _appDbContext.Contato.Remove(contato);
            await _appDbContext.SaveChangesAsync();
        }
    }
}