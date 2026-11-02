using Domain.Abstractions.Repository;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Utils
{
    public static class SpecificationEvaluator
    {
        public static IQueryable<T> GetQuery<T>(
            IQueryable<T> query,
            ISpecification<T>? spec) where T : class
        {

            if (spec == null) return query;

            if (spec.Criteria != null)
            {
                query = query.Where(spec.Criteria);
            }

            query = spec.Includes.Aggregate(query, (current, include) => current.Include(include));

            if (spec.OrderBy != null)
            {
                query = query.OrderBy(spec.OrderBy);
            }
            else if (spec.OrderByDescending != null)
            {
                query = query.OrderByDescending(spec.OrderByDescending);
            }

            return query;
        }
        
        public static IQueryable<TResult> GetQuery<T, TResult>(
            IQueryable<T> query,
            ISpecification<T, TResult>? spec) where T : class
        {
            query = GetQuery<T>(query, spec);

            if (spec?.Selector == null)
            {
                throw new InvalidOperationException($"The specification for '{typeof(T).Name}' is missing a Selector.");
            }

            return query.Select(spec.Selector);
        }
    }
}
