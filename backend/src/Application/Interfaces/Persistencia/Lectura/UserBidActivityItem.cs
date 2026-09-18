using Domain.Enum;

namespace Application.Interfaces.Persistencia.Lectura
{
    public sealed record UserBidActivityItem(
        int SubastaId,
        string Titulo,
        string UrlImagen,
        decimal MiMejorPuja,
        decimal? OfertaActual,
        EstadoSubasta Estado,
        DateTime FechaFin,
        bool EsLider);
}
