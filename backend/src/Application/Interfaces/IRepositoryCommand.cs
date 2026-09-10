using Domain.Common.ResultPattern;

namespace Aplication.Common.Interface
{
    public interface IRepositoryCommand
    {
        void Add<TEntity>(TEntity entity) where TEntity : class;
        void Remove<T>(T entity) where T : class;
        Task<Result<string>> SaveAsync();
        void Update<T>(T entity) where T : class;
    }
}