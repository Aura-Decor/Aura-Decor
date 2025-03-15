using AuraDecor.Core.Entities;
using AuraDecor.Core.Repositories.Contract;
using AuraDecor.Core.Specifications;
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
        return await _dbContext.Set<T>().ToListAsync();
    }

    public async Task<IReadOnlyList<T>> GetAllWithSpecAsync(IBaseSpecification<T> spec)
    {
       return await SpecificationEvaluator<T>.GetQuery(_dbContext.Set<T>(), spec).ToListAsync();
    }

    public async Task<T?> GetWithSpecAsync(IBaseSpecification<T> spec)
    {
       return await SpecificationEvaluator<T>.GetQuery(_dbContext.Set<T>(), spec).FirstOrDefaultAsync();
    }

    public void Add(T entity)
    {
         _dbContext.Set<T>().AddAsync(entity);
    }

    public Task UpdateAsync(T entity)
    {
        _dbContext.Set<T>().Update(entity);
        return Task.CompletedTask;
    }

    public void DeleteAsync(T entity)
    {
        _dbContext.Set<T>().Remove(entity);
    }

    public async Task<int> GetCountAsync(IBaseSpecification<T> spec)
    {
        return await SpecificationEvaluator<T>.GetQuery(_dbContext.Set<T>(), spec).CountAsync();
    }
}

 
