using Application.Interfaces.Persistencia;
using Application.UseCases.Categorias.Queries.BuscarCategoriaPorId;
using Application.UseCases.Subastas.Command.Creacion;
using Application.UseCases.Usuarios.Query.BuscarUsuarioPorId;
using Domain.Common.ResultPattern;
using Domain.Entity;
using FluentValidation;
using MediatR;

public sealed class CreateAuctionHandler
    : IRequestHandler<
        CreateAuctionCommand,
        Result<AuctionDetailResponse>>
{
    private readonly ISubastaCommandRepository _subastaRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateAuctionCommand> _validator;
    private readonly ISender _sender;

    public CreateAuctionHandler(
        ISubastaCommandRepository subastaRepository,
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
            new GetUsuarioByIdQuery(request.VendedorId),
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
            new GetCategoriaByIdQuery(request.CategoriaId),
            cancellationToken);

        if (!categoriaResult.IsSuccess)
        {
            return new Failed<AuctionDetailResponse>(
                categoriaResult.Info,
                categoriaResult.Status);
        }

        var categoria = categoriaResult.Value;

        var subasta = Subasta.Create(
            usuario.Id,
            categoria.Id,
            request.Titulo,
            request.Descripcion,
            request.UrlImagen,
            request.PrecioBase,
            request.IncrementoMinimo,
            request.FechaInicio,
            request.FechaFin);

        // 6. Agregar al repositorio
        _subastaRepository.Add(subasta);

        // 7. Confirmar Unit of Work
        var saveResult = await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        if (!saveResult.IsSuccess)
        {
            return new Failed<AuctionDetailResponse>(
                "No se pudo crear la subasta. Intentá de nuevo.",
                saveResult.Status == DataStatus.Conflict ? DataStatus.Conflict : DataStatus.Exception);
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
