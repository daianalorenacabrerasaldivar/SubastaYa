using Domain.Entity;
using Domain.Enum;

namespace Application.Interfaces.Persistencia
{
    public interface ISubastaQueryRepository : IQueryRepository<Subasta>
    {
        Task<IReadOnlyList<Subasta>> ListByStatusAsync(EstadoSubasta status, CancellationToken cancellationToken);
    }
}
