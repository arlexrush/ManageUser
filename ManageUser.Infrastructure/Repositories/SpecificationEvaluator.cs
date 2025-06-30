using ManageUser.Specification;
using Microsoft.EntityFrameworkCore;

namespace ManageUser.Infrastructure.Repositories
{
    public class SpecificationEvaluator<T> where T : class
    {
        public static IQueryable<T> GetQuery(IQueryable<T> inputQuery, ISpecification<T> spec)
        {
            if (spec.Criteria != null)
            {
                inputQuery = inputQuery.Where(spec.Criteria);
            }

            if (spec.OrderBy != null)
            {
                inputQuery = inputQuery.OrderBy(spec.OrderBy);
            }

            if (spec.OrderByDescending != null)
            {
                inputQuery = inputQuery.OrderBy(spec.OrderByDescending);
            }

            if (spec.IsPagingEnable)
            {
                inputQuery = inputQuery.Skip((int)spec.Skip!).Take((int)spec.Take!);
            }

            inputQuery = spec.Includes!.Aggregate(inputQuery, (current, include) => current.Include(include)).AsSingleQuery().AsNoTracking();

            return inputQuery;
        }
    }
}
