using Domain.Abstractions.Repository.Common;
using System.Linq.Expressions;

namespace Application.Specifications.Common
{
    public abstract class BaseSpecification<T> : ISpecification<T>
    {
        public Expression<Func<T, bool>>? Criteria { get; protected set; }
        public List<Expression<Func<T, object>>> Includes { get; } = [];
        public Expression<Func<T, object>>? OrderBy { get; protected set; }
        public Expression<Func<T, object>>? OrderByDescending { get; protected set; }

        protected void AddInclude(Expression<Func<T, object>> includeExpression)
            => Includes.Add(includeExpression);

        protected void ApplyOrderBy(Expression<Func<T, object>> orderByExpression)
            => OrderBy = orderByExpression;

        protected void ApplyOrderByDescending(Expression<Func<T, object>> orderByDescExpression)
            => OrderByDescending = orderByDescExpression;
    }

    public abstract class BaseSpecification<T, TResult> : BaseSpecification<T>, ISpecification<T, TResult>
    {
        public Expression<Func<T, TResult>>? Selector { get; protected set; }

        protected void ApplySelector(Expression<Func<T, TResult>> selectorExpression)
            => Selector = selectorExpression;
    }
}

