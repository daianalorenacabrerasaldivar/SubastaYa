using Application.Interfaces.Persistencia.Lectura;
using Domain.Entity;

namespace Application.Interfaces.Persistencia
{
    public interface IUsuarioQueryRepository : IQueryRepository<Usuario>
    {
        Task<Usuario?> GetByEmailAsync(string email, CancellationToken cancellationToken);

        Task<IReadOnlyList<UserBidActivityItem>> ListBidActivityAsync(int usuarioId, CancellationToken cancellationToken);

        Task<IReadOnlyList<UserAuctionActivityItem>> ListAuctionActivityAsync(int usuarioId, CancellationToken cancellationToken);
    }
}
