using Application.Interfaces.Persistencia;
using Domain.Common;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories.Common
{
    public abstract class EfQueryRepository<TEntity> : IQueryRepository<TEntity>
        where TEntity : class, IEntity
    {
        protected IQueryable<TEntity> Query { get; }

        protected EfQueryRepository(ApplicationDbContext context)
        {
            Query = context.Set<TEntity>().AsNoTracking();
        }

        public virtual Task<TEntity?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            return Query.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        }
    }
}
