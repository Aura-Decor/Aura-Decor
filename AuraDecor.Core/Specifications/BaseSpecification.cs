using System.Linq.Expressions;
using AuraDecor.Core.Entities;

namespace AuraDecor.Core.Specifications;

public class BaseSpecification<T> : IBaseSpecification<T> where T : BaseEntity
{
    public Expression<Func<T, bool>> Criteria { get; set; } = null;
    public List<Expression<Func<T, object>>> Includes { get; set; } = new List<Expression<Func<T, object>>>();

    public BaseSpecification()
    {
        
    }

    public BaseSpecification(Expression<Func<T,bool>> criteriaExpression)
    {
        Criteria = criteriaExpression;
    }

}