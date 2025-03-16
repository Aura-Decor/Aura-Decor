using System.Linq.Expressions;
using AuraDecor.Core.Entities;

namespace AuraDecor.Core.Specifications;

public class BaseSpecification<T> : IBaseSpecification<T> where T : BaseEntity
{
    public Expression<Func<T, bool>> Criteria { get; set; } = null;
    public List<Expression<Func<T, object>>> Includes { get; set; } = new List<Expression<Func<T, object>>>();
    public Expression<Func<T, object>> OrderByAsc { get; set; }
    public Expression<Func<T, object>> OrderByDesc { get; set; }
    public int Skip { get; set; }
    public int Take { get; set; }
    public bool IsPaginationEnabled { get; set; }
    public List<string> IncludeStrings { get; } = new();


    public BaseSpecification()
    {
    }

    public BaseSpecification(Expression<Func<T,bool>> criteriaExpression)
    {
        Criteria = criteriaExpression;
    }
    
    public void AddOrderBy(Expression<Func<T, object>> orderByExpression)
    {
        OrderByAsc = orderByExpression;
    }

    public void AddOrderByDesc(Expression<Func<T, object>> orderByExpressionDesc)
    {
        OrderByDesc = orderByExpressionDesc;
    }
    
    public void ApplyPagingation(int skip, int take)
    {
        IsPaginationEnabled = true;
        Skip = skip;
        Take = take;
    }
  

    protected void AddInclude(string includeString)
    {
        IncludeStrings.Add(includeString);
    }
    
}