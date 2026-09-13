using Application.Interfaces.Persistencia;
using Application.Interfaces.Persistencia.Lectura;
using Domain.Common.ResultPattern;
using MediatR;

namespace Application.UseCases.Pujas.Query.ListarPorSubasta
{
    public sealed class ListBidsHandler : IRequestHandler<ListBidsQuery, Result<IReadOnlyList<BidHistoryItem>>>
    {
        private readonly ISubastaQueryRepository _subastaRepository;
        private readonly IPujaQueryRepository _pujaRepository;

        public ListBidsHandler(ISubastaQueryRepository subastaRepository, IPujaQueryRepository pujaRepository)
        {
            _subastaRepository = subastaRepository;
            _pujaRepository = pujaRepository;
        }

        public async Task<Result<IReadOnlyList<BidHistoryItem>>> Handle(ListBidsQuery request, CancellationToken cancellationToken)
        {
            var subasta = await _subastaRepository.GetByIdAsync(request.SubastaId, cancellationToken);

            if (subasta is null)
            {
                return new Failed<IReadOnlyList<BidHistoryItem>>($"No existe la subasta con Id {request.SubastaId}.", DataStatus.NotFound);
            }

            var pujas = await _pujaRepository.ListByAuctionAsync(request.SubastaId, cancellationToken);

            return new Success<IReadOnlyList<BidHistoryItem>>(pujas);
        }
    }
}
