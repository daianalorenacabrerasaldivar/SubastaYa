namespace Application.UseCases.Pujas.Command.Ofertar
{
    public sealed record PlaceBidResponse(
        int PujaId,
        int SubastaId,
        int CompradorId,
        decimal Monto,
        DateTime FechaPuja,
        decimal OfertaMasAlta,
        int CantidadOfertas,
        decimal ProximaPujaMinima,
        DateTime FechaFin,
        bool Extendida);
}
