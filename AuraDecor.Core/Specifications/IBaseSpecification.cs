using System.Linq.Expressions;
using AuraDecor.Core.Entities;

namespace AuraDecor.Core.Specifications;

public interface IBaseSpecification<T> where T : BaseEntity
{
     public Expression<Func<T,bool>> Criteria { get; set; }
     public List<Expression<Func<T,object>>> Includes { get; set; }
     
     public Expression<Func<T,object>> OrderByAsc { get; set; }
     
     public Expression<Func<T,object>> OrderByDesc { get; set; }
     
     public int Skip { get; set; }
     
     public int Take { get; set; }
     
     public bool IsPaginationEnabled { get; set; }

     
    
}