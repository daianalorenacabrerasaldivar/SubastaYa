using Domain.Enum;

namespace Application.Interfaces.Persistencia.Lectura
{
    public sealed record AuctionListItem(
        int Id,
        string Titulo,
        string UrlImagen,
        int CategoriaId,
        string CategoriaNombre,
        decimal PrecioBase,
        decimal? OfertaMasAlta,
        int CantidadOfertas,
        DateTime FechaInicio,
        DateTime FechaFin,
        EstadoSubasta Estado);
}
