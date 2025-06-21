using System.Collections;
using AuraDecor.Core.Entities;
using AuraDecor.Core.Repositories.Contract;
using AuraDecor.Repository.Data;
using AuraDecor.Repository.Repositories;

namespace AuraDecor.Repository;

public class UnitOfWork : IUnitOfWork
{
    private readonly Hashtable _repositories;
    private readonly AppDbContext _dbContext;

    public UnitOfWork(AppDbContext dbContext)
    {
        _dbContext = dbContext;
        _repositories = new Hashtable();
    }
    public IGenericRepository<T> Repository<T>() where T : BaseEntity
    {
        var type = typeof(T).Name;
        if (!_repositories.ContainsKey(type))
        {
            var repositoryType = typeof(GenericRepository<>).MakeGenericType(typeof(T));
            var repositoryInstance = Activator.CreateInstance(repositoryType, _dbContext);
            _repositories.Add(type, repositoryInstance);
        }

        return (IGenericRepository<T>)_repositories[type];
    }
    

    public async Task<int> CompleteAsync()
    {
       return await _dbContext.SaveChangesAsync();
    }
    
    public async Task BeginTransactionAsync()
    {
        await _dbContext.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        try
        {
            await _dbContext.SaveChangesAsync();
            await _dbContext.Database.CommitTransactionAsync();
        }
        catch
        {
            await _dbContext.Database.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task RollbackTransactionAsync()
    {
        await _dbContext.Database.RollbackTransactionAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await _dbContext.DisposeAsync();
    }
}
