using Application.Interfaces.Persistencia;
using Application.Interfaces.Persistencia.Lectura;
using Domain.Entity;
using Infrastructure.Persistence.Context;
using Infrastructure.Persistence.Repositories.Common;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories.Billeteras
{
    public sealed class BilleteraQueryRepository : EfQueryRepository<Billetera>, IBilleteraQueryRepository
    {
        public BilleteraQueryRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<WalletBalanceResult?> GetBalanceByUsuarioIdAsync(int usuarioId, CancellationToken cancellationToken)
        {
            return await Query
                .Where(b => b.UsuarioId == usuarioId)
                .Select(b => new WalletBalanceResult(b.UsuarioId, b.SaldoTotal, b.SaldoRetenido, b.SaldoDisponible))
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<LedgerTransactionItem>> ListTransactionsByUsuarioIdAsync(int usuarioId, CancellationToken cancellationToken)
        {
            return await Query
                .Where(b => b.UsuarioId == usuarioId)
                .SelectMany(b => b.Transacciones)
                .OrderByDescending(t => t.Fecha)
                .ThenByDescending(t => t.Id)
                .Select(t => new LedgerTransactionItem(t.Id, t.Tipo, t.Monto, t.Fecha, t.SubastaId))
                .ToListAsync(cancellationToken);
        }
    }
}
