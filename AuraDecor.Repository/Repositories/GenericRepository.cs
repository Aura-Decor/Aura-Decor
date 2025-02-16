using AuraDecor.Core.Entities;
using AuraDecor.Core.Repositories.Contract;
using AuraDecor.Repository.Data;
using Microsoft.EntityFrameworkCore;

namespace AuraDecor.Repository.Repositories;

public class GenericRepository<T>: IGenericRepository<T> where T : BaseEntity
{
    private readonly AppDbContext _dbContext;

    public GenericRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<T> GetByIdAsync(Guid id)
    {
        return await _dbContext.Set<T>().FindAsync(id); 
        
    }

    public async Task<IReadOnlyList<T>> GetAllAsync()
    {
        if (typeof(T) == typeof(Furniture))
        {
            return await _dbContext.Set<T>()
                .Include(p => ((Furniture)(object)p).Category)
                .Include(p => ((Furniture)(object)p).Brand)
                .ToListAsync();
        }
        return await _dbContext.Set<T>().ToListAsync();
    }

    public void Add(T entity)
    {
         _dbContext.Set<T>().AddAsync(entity);
    }

    public void UpdateAsync(T entity)
    {
        _dbContext.Set<T>().Update(entity);
    }

    public void DeleteAsync(T entity)
    {
        _dbContext.Set<T>().Remove(entity);
    }
 
}

 
