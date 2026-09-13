using Application.Interfaces.Persistencia;
using Application.Interfaces.Persistencia.Lectura;
using Domain.Entity;
using Infrastructure.Persistence.Context;
using Infrastructure.Persistence.Repositories.Common;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories.Pujas
{
    public sealed class PujaQueryRepository : EfQueryRepository<Puja>, IPujaQueryRepository
    {
        public PujaQueryRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IReadOnlyList<BidHistoryItem>> ListByAuctionAsync(int subastaId, CancellationToken cancellationToken)
        {
            var pujas = await Query
                .Where(p => p.SubastaId == subastaId)
                .OrderByDescending(p => p.FechaPuja)
                .ThenByDescending(p => p.Id)
                .Select(p => new { p.Id, p.Monto, p.FechaPuja, p.CompradorId })
                .ToListAsync(cancellationToken);

            return pujas
                .Select(p => new BidHistoryItem(p.Id, p.Monto, p.FechaPuja, $"Comprador #{p.CompradorId}"))
                .ToList();
        }
    }
}
