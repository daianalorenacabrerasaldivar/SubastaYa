using Application.Interfaces.Persistencia.Lectura;
using Domain.Entity;
using Domain.Enum;

namespace Application.Interfaces.Persistencia
{
    public interface ISubastaQueryRepository : IQueryRepository<Subasta>
    {
        Task<IReadOnlyList<Subasta>> ListByStatusAsync(EstadoSubasta status, CancellationToken cancellationToken);

        Task<PagedResult<AuctionListItem>> ListAsync(AuctionListFilter filter, CancellationToken cancellationToken);

        Task<AuctionDetail?> GetDetailAsync(int id, CancellationToken cancellationToken);
    }
}
