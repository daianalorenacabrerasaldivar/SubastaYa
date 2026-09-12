using Domain.Common.ResultPattern;

namespace Application.Interfaces.Persistencia
{
    public interface IRepositoryCommand
    {
        void Add<TEntity>(TEntity entity) where TEntity : class;
        void Remove<T>(T entity) where T : class;
        void Update<T>(T entity) where T : class;
    }
}