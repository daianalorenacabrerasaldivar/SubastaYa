using Domain.Entity;

namespace Application.Interfaces.Persistencia
{
    public interface ICategoriaQueryRepository : IQueryRepository<Categoria>
    {
        Task<IReadOnlyList<Categoria>> ListAllAsync(CancellationToken cancellationToken);
    }
}
