namespace Application.Interfaces.Persistencia.Lectura
{
    public sealed record BidHistoryItem(
        int Id,
        decimal Monto,
        DateTime FechaPuja,
        string Seudonimo);
}
