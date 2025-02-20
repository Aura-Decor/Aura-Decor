using AuraDecor.Core.Entities;
using AuraDecor.Core.Specifications;

namespace AuraDecor.Core.Repositories.Contract;

public interface IGenericRepository<T> where T : BaseEntity
{
    Task<T> GetByIdAsync (Guid id);
    Task<IReadOnlyList<T>> GetAllAsync();
    Task<IReadOnlyList<T>> GetAllWithSpecAsync(IBaseSpecification<T> spec);
    Task<T?> GetWithSpecAsync(IBaseSpecification<T> spec);
    void Add(T entity);
    void UpdateAsync(T entity);
    void DeleteAsync(T entity);
    
}