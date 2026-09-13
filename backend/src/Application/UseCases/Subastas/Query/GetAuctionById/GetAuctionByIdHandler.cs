using Application.Interfaces.Persistencia;
using Domain.Common.ResultPattern;
using Domain.Entity;
using MediatR;

namespace Application.UseCases.Subastas.Query.GetAuctionById
{
    public sealed class GetAuctionByIdHandler : IRequestHandler<GetAuctionByIdQuery, Result<GetAuctionByIdResponse>>
    {
        private readonly ISubastaQueryRepository _subastaRepository;

        public GetAuctionByIdHandler(ISubastaQueryRepository subastaRepository)
        {
            _subastaRepository = subastaRepository;
        }

        public async Task<Result<GetAuctionByIdResponse>> Handle(GetAuctionByIdQuery request, CancellationToken cancellationToken)
        {
            var detalle = await _subastaRepository.GetDetailAsync(request.Id, cancellationToken);

            if (detalle is null)
            {
                return new Failed<GetAuctionByIdResponse>($"No existe la subasta con Id {request.Id}.", DataStatus.NotFound);
            }

            var response = new GetAuctionByIdResponse(
                detalle.Id,
                detalle.VendedorId,
                detalle.VendedorNombre,
                detalle.CategoriaId,
                detalle.CategoriaNombre,
                detalle.Titulo,
                detalle.Descripcion,
                detalle.UrlImagen,
                detalle.PrecioBase,
                detalle.IncrementoMinimo,
                detalle.OfertaMasAlta,
                detalle.CantidadOfertas,
                detalle.PostorLiderId,
                Subasta.CalcularMontoMinimo(detalle.OfertaMasAlta, detalle.PrecioBase, detalle.IncrementoMinimo),
                detalle.FechaInicio,
                detalle.FechaFin,
                detalle.Estado,
                DateTime.UtcNow,
                detalle.Version);

            return new Success<GetAuctionByIdResponse>(response);
        }
    }
}
