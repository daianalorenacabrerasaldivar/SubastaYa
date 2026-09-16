using Api.Hubs;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.SignalR;

namespace Api.Services
{
    public sealed class SignalRAuctionNotifier : IAuctionNotifier
    {
        private readonly IHubContext<AuctionHub> _hub;

        public SignalRAuctionNotifier(IHubContext<AuctionHub> hub)
        {
            _hub = hub;
        }

        public Task NotificarNuevaPujaAsync(int subastaId, object payload, CancellationToken ct = default)
            => _hub.Clients
                   .Group(AuctionHub.GrupoKey(subastaId))
                   .SendAsync("NewBid", payload, ct);
    }
}
