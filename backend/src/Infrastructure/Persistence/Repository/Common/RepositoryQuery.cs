using Application.Interfaces.Persistencia;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure.Persistence.Repository.Common
{
    public class RepositoryQuery<TContext> : IRepositoryQuery where TContext : DbContext
    {
        private readonly TContext _context;
        public RepositoryQuery(TContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<T?> FirstOrDefaultAsync<T>(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default)
        where T : class
        {
            return await _context
                .Set<T>()
                .FirstOrDefaultAsync(
                    predicate,
                    cancellationToken);
        }

        public IQueryable<T> Query<T>() where T : class
        {
            return _context.Set<T>();
        }
    }
}
