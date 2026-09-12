using Application.Interfaces.Persistencia;
using Application.UseCases.Categorias.Queries.BuscarCategoriaPorId;
using Application.UseCases.Subasta.Command.Creacion;
using Application.UseCases.Usuario.Query.BuscarUsuarioPorId;
using Domain.Common.ResultPattern;
using Domain.Entity;
using FluentValidation;
using MediatR;

public sealed class CreateAuctionHandler
    : IRequestHandler<
        CreateAuctionCommand,
        Result<AuctionDetailResponse>>
{
    private readonly ISubastaRepositoryCommand _subastaRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateAuctionCommand> _validator;
    private readonly ISender _sender;

    public CreateAuctionHandler(
        ISubastaRepositoryCommand subastaRepository,
        IUnitOfWork unitOfWork,
        IValidator<CreateAuctionCommand> validator,
        ISender sender)
    {
        _subastaRepository = subastaRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
        _sender = sender;
    }

    public async Task<Result<AuctionDetailResponse>> Handle(
        CreateAuctionCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Validaciones de entrada
        var validationResult = await _validator.ValidateAsync(
            request,
            cancellationToken);

        if (!validationResult.IsValid)
        {
            var errors = string.Join(
                " | ",
                validationResult.Errors.Select(x => x.ErrorMessage));

            return new Failed<AuctionDetailResponse>(
                errors,
                DataStatus.RequestValidation);
        }

        // 2. Obtener Usuario mediante CQRS
        var usuarioResult = await _sender.Send(
            new BuscarUsuarioPorIdQuery(request.VendedorId),
            cancellationToken);

        if (!usuarioResult.IsSuccess)
        {
            return new Failed<AuctionDetailResponse>(
                usuarioResult.Info,
                usuarioResult.Status);
        }

        var usuario = usuarioResult.Value;

        // 3. Obtener Categoría mediante CQRS
        var categoriaResult = await _sender.Send(
            new BuscarCategoriaPorIdQuery(request.CategoriaId),
            cancellationToken);

        if (!categoriaResult.IsSuccess)
        {
            return new Failed<AuctionDetailResponse>(
                categoriaResult.Info,
                categoriaResult.Status);
        }

        var categoria = categoriaResult.Value;

        // 5. Crear entidad Subasta
        var subasta = new Subasta
        {
            Id = usuario.Id,
            CategoriaId = categoria.Id,

            Titulo = request.Titulo,
            Descripcion = request.Descripcion,
            UrlImagen = request.UrlImagen,

            PrecioBase = request.PrecioBase,
            IncrementoMinimo = request.IncrementoMinimo,

            FechaInicio = request.FechaInicio,
            FechaFin = request.FechaFin
        };

        // 6. Agregar al Repository
        _subastaRepository.Add(subasta);

        // 7. Confirmar Unit of Work
        var saveResult = await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        if (!saveResult.IsSuccess)
        {
            return new Failed<AuctionDetailResponse>(
                saveResult.Info,
                saveResult.Status);
        }

        // 8. Crear respuesta
        var response = new AuctionDetailResponse(
            subasta.Id,
             subasta.VendedorId,
            subasta.CategoriaId,
            subasta.Titulo,
            subasta.Descripcion,
            subasta.UrlImagen,
            subasta.PrecioBase,
            subasta.IncrementoMinimo,
            subasta.FechaInicio,
            subasta.FechaFin,
            subasta.Estado.ToString(),
            subasta.Version);

        return new Success<AuctionDetailResponse>(response);
    }
}
