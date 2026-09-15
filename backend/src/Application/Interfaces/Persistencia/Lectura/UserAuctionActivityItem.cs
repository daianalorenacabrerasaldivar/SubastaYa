using Domain.Enum;

namespace Application.Interfaces.Persistencia.Lectura
{
    public sealed record UserAuctionActivityItem(
        int SubastaId,
        string Titulo,
        string UrlImagen,
        EstadoSubasta Estado,
        DateTime FechaInicio,
        DateTime FechaFin,
        decimal PrecioBase,
        decimal? OfertaActual,
        int CantidadPujas);
}
