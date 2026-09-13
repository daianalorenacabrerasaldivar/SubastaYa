using Application.Interfaces.Persistencia.Lectura;
using Domain.Entity;

namespace Application.Interfaces.Persistencia
{
    public interface IPujaQueryRepository : IQueryRepository<Puja>
    {
        Task<IReadOnlyList<BidHistoryItem>> ListByAuctionAsync(int subastaId, CancellationToken cancellationToken);
    }
}
