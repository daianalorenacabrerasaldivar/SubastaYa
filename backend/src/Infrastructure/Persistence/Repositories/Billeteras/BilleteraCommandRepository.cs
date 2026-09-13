using Application.Interfaces.Persistencia;
using Domain.Entity;
using Infrastructure.Persistence.Context;
using Infrastructure.Persistence.Repositories.Common;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories.Billeteras
{
    public sealed class BilleteraCommandRepository : EfCommandRepository<Billetera>, IBilleteraCommandRepository
    {
        public BilleteraCommandRepository(ApplicationDbContext context) : base(context)
        {
        }

        public Task<Billetera?> GetByUsuarioIdAsync(int usuarioId, CancellationToken cancellationToken)
        {
            return Set.FirstOrDefaultAsync(b => b.UsuarioId == usuarioId, cancellationToken);
        }
    }
}
