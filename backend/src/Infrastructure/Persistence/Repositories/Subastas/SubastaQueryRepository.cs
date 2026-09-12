using Application.Interfaces.Persistencia;
using Domain.Entity;
using Domain.Enum;
using Infrastructure.Persistence.Context;
using Infrastructure.Persistence.Repositories.Common;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories.Subastas
{
    public sealed class SubastaQueryRepository : EfQueryRepository<Subasta>, ISubastaQueryRepository
    {
        public SubastaQueryRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IReadOnlyList<Subasta>> ListByStatusAsync(EstadoSubasta status, CancellationToken cancellationToken)
        {
            return await Query
                .Where(s => s.Estado == status)
                .OrderBy(s => s.FechaFin)
                .ToListAsync(cancellationToken);
        }
    }
}
