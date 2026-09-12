using System.Linq.Expressions;

namespace Application.Interfaces.Persistencia;
public interface IRepositoryQuery
{
    IQueryable<T> Query<T>() where T : class;

    Task<T?> FirstOrDefaultAsync<T>(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default)
        where T : class;

}