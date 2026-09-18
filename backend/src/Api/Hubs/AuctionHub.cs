using Microsoft.AspNetCore.SignalR;

namespace Api.Hubs
{
    /// <summary>
    /// Hub de SignalR para la sala de subasta en vivo.
    /// Los clientes se unen a un grupo por subastaId y reciben eventos NewBid en tiempo real.
    /// </summary>
    public sealed class AuctionHub : Hub
    {
        /// <summary>
        /// El cliente se une al grupo de la subasta para recibir notificaciones de nuevas pujas.
        /// </summary>
        public async Task UnirseASubasta(int subastaId)
            => await Groups.AddToGroupAsync(Context.ConnectionId, GrupoKey(subastaId));

        /// <summary>
        /// El cliente abandona el grupo de la subasta.
        /// </summary>
        public async Task SalirDeSubasta(int subastaId)
            => await Groups.RemoveFromGroupAsync(Context.ConnectionId, GrupoKey(subastaId));

        public static string GrupoKey(int subastaId) => $"subasta-{subastaId}";
    }
}
