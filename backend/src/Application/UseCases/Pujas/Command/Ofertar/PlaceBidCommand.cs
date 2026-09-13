using Domain.Common.ResultPattern;
using MediatR;

namespace Application.UseCases.Pujas.Command.Ofertar
{
    public sealed class PlaceBidCommand : IRequest<Result<PlaceBidResponse>>
    {
        public int SubastaId { get; set; }

        public int CompradorId { get; set; }

        public decimal Monto { get; set; }
    }
}
