using Application.Interfaces.Persistencia;
using Domain.Entity;
using Domain.Enum;
using Infrastructure.Persistence.Context;
using Infrastructure.Persistence.Repositories.Common;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories.Subastas
{
    public sealed class SubastaCommandRepository : EfCommandRepository<Subasta>, ISubastaCommandRepository
    {
        public SubastaCommandRepository(ApplicationDbContext context) : base(context)
        {
        }

        public override void Update(Subasta entity)
        {
            Set.Entry(entity).State = EntityState.Modified;
        }

        public override async Task<Subasta?> GetByIdAsync(int id, CancellationToken cancellationToken) =>
            await Set.Include(s => s.Pujas).FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

        public Task<Subasta?> GetForBiddingAsync(int id, CancellationToken cancellationToken)
        {
            return GetByIdAsync(id, cancellationToken);
        }

        public Task<IReadOnlyList<Subasta>> GetVencidasAsync(DateTime ahora, CancellationToken cancellationToken)
        {
            return Set
                .Include(s => s.Pujas)
                .Where(s => s.Estado == EstadoSubasta.ACTIVA && s.FechaFin <= ahora)
                .ToListAsync(cancellationToken)
                .ContinueWith(t => (IReadOnlyList<Subasta>)t.Result, TaskContinuationOptions.ExecuteSynchronously);
        }

        public Task<IReadOnlyList<Subasta>> GetProgramadasParaActivarAsync(DateTime ahora, CancellationToken cancellationToken)
        {
            return Set
                .Where(s => s.Estado == EstadoSubasta.PROGRAMADA && s.FechaInicio <= ahora)
                .ToListAsync(cancellationToken)
                .ContinueWith(t => (IReadOnlyList<Subasta>)t.Result, TaskContinuationOptions.ExecuteSynchronously);
        }
    }
}
