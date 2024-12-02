using AuraDecor.Core.Entities;

namespace AuraDecor.Core.Repositories.Contract;

public interface IGenericRepository<T> where T : BaseEntity
{
    Task<T> GetByIdAsync (Guid id);
    Task<IReadOnlyList<T>> GetAllAsync();
    void Add(T entity);
    void UpdateAsync(T entity);
    void DeleteAsync(T entity);
    
}