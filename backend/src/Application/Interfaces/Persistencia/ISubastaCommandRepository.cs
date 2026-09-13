using Domain.Entity;

namespace Application.Interfaces.Persistencia
{
    public interface ISubastaCommandRepository : ICommandRepository<Subasta>
    {
        Task<Subasta?> GetForBiddingAsync(int id, CancellationToken cancellationToken);
    }
}
