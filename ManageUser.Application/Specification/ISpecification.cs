using System.Linq.Expressions;

namespace ManageUser.Specification
{
    public interface ISpecification<T>
    {
        // ISpecification define la estructura común de una especificación de consulta parametrizable: propiedades para filtros, ordenamiento, paginación, etc.
        Expression<Func<T, bool>>? Criteria { get; }

        List<Expression<Func<T, object>>> Includes { get; }

        Expression<Func<T, object>>? OrderBy { get; }

        Expression<Func<T, object>>? OrderByDescending { get; }

        int? Take { get; }

        int? Skip { get; }

        bool IsPagingEnable { get; }

        
    }
}
