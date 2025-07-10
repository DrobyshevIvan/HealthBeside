using HealthBeside.Domain.Models.Shared;
using Microsoft.EntityFrameworkCore;

namespace HealthBeside.Infrastructure.Repository;

public class GenericRepository<T> : IGenericRepository<T> where T: class
{
    private readonly AppDbContext _context;

    public GenericRepository(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<T> GetAsync(Guid? id)
    {
        if (id is null)
        {
            return null;
        }

        return await _context.Set<T>().FindAsync(id);
    }

    public async Task<List<T>> GetAllAsync()
    {
        return await _context.Set<T>().ToListAsync();
    }

    public async Task<T> AddAsync(T entity)
    {
        await _context.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task DeleteAsync(Guid id)
    {
        var entity = await GetAsync(id);
        if (entity == null)
        {
            throw new NullReferenceException($"Entity with id {id} was not found");
        }

        _context.Set<T>().Remove(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(T entity)
    {
        _context.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> Exists(Guid id)
    {
        var entity =  await GetAsync(id);
        return entity != null;
    }
}