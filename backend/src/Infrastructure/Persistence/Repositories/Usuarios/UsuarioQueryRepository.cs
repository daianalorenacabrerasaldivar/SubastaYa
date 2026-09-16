using Application.Interfaces.Persistencia;
using Application.Interfaces.Persistencia.Lectura;
using Domain.Entity;
using Domain.Enum;
using Infrastructure.Persistence.Context;
using Infrastructure.Persistence.Repositories.Common;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories.Usuarios
{
    public sealed class UsuarioQueryRepository : EfQueryRepository<Usuario>, IUsuarioQueryRepository
    {
        private readonly ApplicationDbContext _context;

        public UsuarioQueryRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public Task<Usuario?> GetByEmailAsync(string email, CancellationToken cancellationToken)
            => Query.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

        public async Task<IReadOnlyList<UserBidActivityItem>> ListBidActivityAsync(
            int usuarioId, CancellationToken cancellationToken)
        {
            return await _context.Subastas
                .AsNoTracking()
                .Where(s => s.Pujas.Any(p => p.CompradorId == usuarioId))
                .OrderByDescending(s => s.FechaFin)
                .Select(s => new UserBidActivityItem(
                    s.Id,
                    s.Titulo,
                    s.UrlImagen,
                    s.Pujas.Where(p => p.CompradorId == usuarioId).Max(p => p.Monto),
                    s.Pujas.Max(p => (decimal?)p.Monto),
                    s.Estado,
                    s.FechaFin,
                    s.Pujas
                        .OrderByDescending(p => p.Monto)
                        .ThenByDescending(p => p.FechaPuja)
                        .Select(p => (int?)p.CompradorId)
                        .FirstOrDefault() == usuarioId))
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<UserAuctionActivityItem>> ListAuctionActivityAsync(
            int usuarioId, CancellationToken cancellationToken)
        {
            return await _context.Subastas
                .AsNoTracking()
                .Where(s => s.VendedorId == usuarioId)
                .OrderByDescending(s => s.FechaFin)
                .Select(s => new UserAuctionActivityItem(
                    s.Id,
                    s.Titulo,
                    s.UrlImagen,
                    s.Estado,
                    s.FechaInicio,
                    s.FechaFin,
                    s.PrecioBase,
                    s.Pujas.Max(p => (decimal?)p.Monto),
                    s.Pujas.Count))
                .ToListAsync(cancellationToken);
        }
    }
}
