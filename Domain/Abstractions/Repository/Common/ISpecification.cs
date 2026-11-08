using System.Linq.Expressions;

namespace Domain.Abstractions.Repository.Common
{
    /// <summary>Query object describing filtering, eager loading and sorting for an entity.</summary>
    /// <typeparam name="T">Entity type being queried.</typeparam>
    public interface ISpecification<T>
    {
        /// <summary>Optional <c>WHERE</c> predicate.</summary>
        Expression<Func<T, bool>>? Criteria { get; }

        /// <summary>Navigation properties to eager-load.</summary>
        List<Expression<Func<T, object>>> Includes { get; }

        /// <summary>Optional ascending sort expression.</summary>
        Expression<Func<T, object>>? OrderBy { get; }

        /// <summary>Optional descending sort expression.</summary>
        Expression<Func<T, object>>? OrderByDescending { get; }
    }

    /// <summary>Specification that also projects each entity into <typeparamref name="TResult"/>.</summary>
    /// <typeparam name="T">Entity type being queried.</typeparam>
    /// <typeparam name="TResult">Projection result type.</typeparam>
    public interface ISpecification<T, TResult> : ISpecification<T>
    {
        /// <summary>Optional <c>SELECT</c> projection.</summary>
        Expression<Func<T, TResult>>? Selector { get; }
    }
}
