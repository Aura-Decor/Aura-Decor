using AuraDecor.Core.Entities;
using AuraDecor.Core.Specifications;
using Microsoft.EntityFrameworkCore;

namespace AuraDecor.Repository;

public static class SpecificationEvaluator<T> where T : BaseEntity
{
  
        public static IQueryable<T> GetQuery(IQueryable<T> inputQuery, IBaseSpecification<T> spec)
        {
            var query = inputQuery;

            if (spec.Criteria is not null)
            {
                query = query.Where(spec.Criteria);
            }

            // Handle regular includes
            query = spec.Includes.Aggregate(query, (current, include) => current.Include(include));

            // Handle string-based includes with ThenIncludes
            if (spec.IncludeStrings.Any())
            {
                query = spec.IncludeStrings.Aggregate(query, (current, include) => current.Include(include));
            }

            if (spec.OrderByAsc is not null)
            {
                query = query.OrderBy(spec.OrderByAsc);
            }
            else if (spec.OrderByDesc is not null)
            {
                query = query.OrderByDescending(spec.OrderByDesc);
            }

            if (spec.IsPaginationEnabled)
            {
                query = query.Skip(spec.Skip).Take(spec.Take);
            }

            return query;
        }
    }


