using Application.Interfaces.Persistencia;
using Application.Interfaces.Persistencia.Lectura;
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

        public async Task<PagedResult<AuctionListItem>> ListAsync(AuctionListFilter filter, CancellationToken cancellationToken)
        {
            var subastas = Query;

            subastas = filter.Status switch
            {
                AuctionStatusFilter.Activas => subastas.Where(s => s.Estado == EstadoSubasta.ACTIVA),
                AuctionStatusFilter.Proximas => subastas.Where(s => s.Estado == EstadoSubasta.PROGRAMADA),
                AuctionStatusFilter.Finalizadas => subastas.Where(s => s.Estado == EstadoSubasta.FINALIZADA || s.Estado == EstadoSubasta.DESIERTA),
                _ => subastas
            };

            if (filter.CategoryId is int categoryId)
            {
                subastas = subastas.Where(s => s.CategoriaId == categoryId);
            }

            var proyeccion = subastas.Select(s => new
            {
                s.Id,
                s.Titulo,
                s.UrlImagen,
                s.CategoriaId,
                CategoriaNombre = s.Categoria.Nombre,
                s.PrecioBase,
                OfertaMasAlta = s.Pujas.Max(p => (decimal?)p.Monto),
                CantidadOfertas = s.Pujas.Count(),
                s.FechaInicio,
                s.FechaFin,
                s.Estado
            });

            if (filter.MinPrice is decimal minPrice)
            {
                proyeccion = proyeccion.Where(x => (x.OfertaMasAlta ?? x.PrecioBase) >= minPrice);
            }

            if (filter.MaxPrice is decimal maxPrice)
            {
                proyeccion = proyeccion.Where(x => (x.OfertaMasAlta ?? x.PrecioBase) <= maxPrice);
            }

            proyeccion = filter.SortBy == AuctionSortBy.MayorPuja
                ? proyeccion.OrderByDescending(x => x.OfertaMasAlta ?? x.PrecioBase).ThenBy(x => x.Id)
                : proyeccion.OrderBy(x => x.FechaFin).ThenBy(x => x.Id);

            var totalItems = await proyeccion.CountAsync(cancellationToken);

            var items = await proyeccion
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(x => new AuctionListItem(
                    x.Id,
                    x.Titulo,
                    x.UrlImagen,
                    x.CategoriaId,
                    x.CategoriaNombre,
                    x.PrecioBase,
                    x.OfertaMasAlta,
                    x.CantidadOfertas,
                    x.FechaInicio,
                    x.FechaFin,
                    x.Estado))
                .ToListAsync(cancellationToken);

            return new PagedResult<AuctionListItem>(items, totalItems);
        }
    }
}
