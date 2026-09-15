using Application.Interfaces.Persistencia.Lectura;
using Domain.Entity;

namespace Application.Interfaces.Persistencia
{
    public interface IBilleteraQueryRepository : IQueryRepository<Billetera>
    {
        Task<WalletBalanceResult?> GetBalanceByUsuarioIdAsync(int usuarioId, CancellationToken cancellationToken);
        Task<IReadOnlyList<LedgerTransactionItem>> ListTransactionsByUsuarioIdAsync(int usuarioId, CancellationToken cancellationToken);
    }
}
