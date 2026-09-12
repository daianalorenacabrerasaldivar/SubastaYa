using Application.Interfaces.Persistencia;
using Domain.Common;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories.Common
{
    public abstract class EfCommandRepository<TEntity> : ICommandRepository<TEntity>
        where TEntity : class, IEntity
    {
        protected DbSet<TEntity> Set { get; }

        protected EfCommandRepository(ApplicationDbContext context)
        {
            Set = context.Set<TEntity>();
        }

        public virtual async Task<TEntity?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await Set.FindAsync(new object[] { id }, cancellationToken);
        }

        public void Add(TEntity entity)
        {
            Set.Add(entity);
        }

        public void Update(TEntity entity)
        {
            Set.Update(entity);
        }

        public void Remove(TEntity entity)
        {
            Set.Remove(entity);
        }
    }
}
