using Application.Interfaces;
using Application.Dto.Auctions;
using FluentValidation;
using MediatR;
using Domain.Common.ResultPattern;

namespace Application.UseCases.Subasta.Command.Creacion
{
    public class CrearSubastaHandler : IRequestHandler<CreateAuctionCommand, Result<AuctionDetailResponse>>
    {
        private readonly ISubastaService _subastaService;
        private readonly IValidator<CreateAuctionCommand> _validator;

        public CrearSubastaHandler(
            ISubastaService subastaService,
            IValidator<CreateAuctionCommand> validator)
        {
            _subastaService = subastaService;
            _validator = validator;
        }

        public async Task<Result<AuctionDetailResponse>> Handle(CreateAuctionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _validator.ValidateAndThrowAsync(request, cancellationToken);

                var result = await _subastaService.CreateAsync(new CreateAuctionCommand
                {
                    VendedorId = request.VendedorId,
                    CategoriaId = request.CategoriaId,
                    Titulo = request.Titulo,
                    Descripcion = request.Descripcion,
                    UrlImagen = request.UrlImagen,
                    PrecioBase = request.PrecioBase,
                    IncrementoMinimo = request.IncrementoMinimo,
                    FechaInicio = request.FechaInicio,
                    FechaFin = request.FechaFin
                }, cancellationToken);

                if (!result.IsSuccess)
                {
                    return new Failed<AuctionDetailResponse>(result.Info, result.Status);
                }

                var resultValue = result.Value;
                return new Success<AuctionDetailResponse>(new AuctionDetailResponse
                {
                    Id = resultValue.Id,
                    VendedorId = resultValue.VendedorId,
                    CategoriaId = resultValue.CategoriaId,
                    Titulo = resultValue.Titulo,
                    Descripcion = resultValue.Descripcion,
                    Url_imagen = resultValue.UrlImagen,
                    Precio_base = resultValue.PrecioBase,
                    Incremento_minimo = resultValue.IncrementoMinimo,
                    Fecha_inicio = resultValue.FechaInicio,
                    Fecha_fin = resultValue.FechaFin,
                    Estado = resultValue.Estado.ToString(),
                    Version = resultValue.Version
                });
            }
            catch (ValidationException ex)
            {
                return new Failed<AuctionDetailResponse>(ex.Message, DataStatus.RequestValidation);
            }
            catch (Exception ex)
            {
                return new Failed<AuctionDetailResponse>(ex.Message, DataStatus.Exception);
            }
        }
    }
}
