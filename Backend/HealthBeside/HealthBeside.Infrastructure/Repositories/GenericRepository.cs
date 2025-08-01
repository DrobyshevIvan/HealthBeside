using HealthBeside.Domain.Shared;
using Microsoft.EntityFrameworkCore;
using System.Threading; // Необхідний простір імен
using System.Threading.Tasks;

namespace HealthBeside.Infrastructure.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    private readonly AppDbContext _context;

    public GenericRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<T?> GetAsync(Guid? id, CancellationToken cancellationToken = default)
    {
        if (id is null)
        {
            return null;
        }

        // CancellationToken передається у FindAsync
        return await _context.Set<T>().FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<List<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        // CancellationToken передається у ToListAsync
        return await _context.Set<T>().ToListAsync(cancellationToken);
    }

    public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _context.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await GetAsync(id, cancellationToken);
        if (entity == null)
        {
            throw new NullReferenceException($"Entity with id {id} was not found");
        }

        _context.Set<T>().Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        _context.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> Exists(Guid id, CancellationToken cancellationToken = default)
    {
        // Передача cancellationToken в GetAsync
        var entity = await GetAsync(id, cancellationToken);
        return entity != null;
    }
}