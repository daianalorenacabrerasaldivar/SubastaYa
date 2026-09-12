using Domain.Common;

namespace Application.Interfaces.Persistencia
{
    public interface ICommandRepository<TEntity> where TEntity : class, IEntity
    {
        Task<TEntity?> GetByIdAsync(int id, CancellationToken cancellationToken);

        void Add(TEntity entity);

        void Update(TEntity entity);

        void Remove(TEntity entity);
    }
}
