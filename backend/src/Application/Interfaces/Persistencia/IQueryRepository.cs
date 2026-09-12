using Domain.Common;

namespace Application.Interfaces.Persistencia
{
    public interface IQueryRepository<TEntity> where TEntity : class, IEntity
    {
        Task<TEntity?> GetByIdAsync(int id, CancellationToken cancellationToken);
    }
}
