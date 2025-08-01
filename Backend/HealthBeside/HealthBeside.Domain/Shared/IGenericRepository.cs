namespace HealthBeside.Domain.Shared;

public interface IGenericRepository<T> where T: class
{
    Task<T?> GetAsync(Guid? id, CancellationToken cancellationToken = default);
    Task<List<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
    Task<bool> Exists(Guid id, CancellationToken cancellationToken = default);
}