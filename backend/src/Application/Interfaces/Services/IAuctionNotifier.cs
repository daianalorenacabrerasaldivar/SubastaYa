namespace Application.Interfaces.Services
{
    public interface IAuctionNotifier
    {
        Task NotificarNuevaPujaAsync(int subastaId, object payload, CancellationToken ct = default);
    }
}
