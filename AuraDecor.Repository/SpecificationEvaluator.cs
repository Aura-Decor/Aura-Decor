using AuraDecor.Core.Entities;
using AuraDecor.Core.Specifications;
using Microsoft.EntityFrameworkCore;

namespace AuraDecor.Repository;

public static class SpecificationEvaluator<T> where T : BaseEntity
{
    public static IQueryable<T> GetQuery(IQueryable<T> inputQuery, IBaseSpecification<T> Spec)
    {
        var query = inputQuery; // _dbContext.Set<Furniture>();

        if (Spec.Criteria is not null)
        {
            query =  query.Where(Spec.Criteria); // _dbContext.Set<Furniture>().Where(p=>p.Id==1)
        }
        if (Spec.OrderByAsc is not null)
        {
            query = query.OrderBy(Spec.OrderByAsc);
        }

        else if (Spec.OrderByDesc is not null )
        {
            query = query.OrderByDescending(Spec.OrderByDesc);
        }
        query = Spec.Includes.Aggregate(query, (current, include) => current.Include(include));
        if (Spec.IsPaginationEnabled)
        {
            query = query.Skip(Spec.Skip).Take(Spec.Take);
        }
        // _dbContext.Set<Furniture>().Where(p=>p.Id==1).Include(p=>p.Category).Include(p=>p.Brand)
        return query;
    }
    
}