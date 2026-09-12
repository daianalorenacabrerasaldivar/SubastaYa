using Application.Dto.Auctions;
using Application.Interfaces;
using Domain.Common.ResultPattern;
using Domain.Entity;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Aplication.Common.Interface;

namespace Application.Services;

public sealed class SubastaService : ISubastaService
{
    private readonly ISubastaCommandRepository _commandRepository;
    private readonly IRepositoryQuery _queryRepository;
    private readonly IValidator<CreateAuctionRequestDto> _validator;

    public SubastaService(
        ISubastaCommandRepository commandRepository,
        IRepositoryQuery queryRepository,
        IValidator<CreateAuctionRequestDto> validator)
    {
        _commandRepository = commandRepository;
        _queryRepository = queryRepository;
        _validator = validator;
    }

    public async Task<Result<AuctionDetailResponseDto>> CreateAsync(
        CreateAuctionRequestDto request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        var vendedorExists = await _queryRepository.Query<Usuario>()
            .AnyAsync(x => x.Id == request.VendedorId, cancellationToken);
        if (!vendedorExists)
        {
            return new Failed<AuctionDetailResponseDto>("El vendedor indicado no existe.");
        }

        var categoriaExists = await _queryRepository.Query<Categoria>()
            .AnyAsync(x => x.Id == request.CategoriaId, cancellationToken);
        if (!categoriaExists)
        {
            return new Failed<AuctionDetailResponseDto>("La categoría indicada no existe.");
        }

        var subasta = Subasta.Create(
            request.VendedorId,
            request.CategoriaId,
            request.Titulo,
            request.Descripcion,
            request.UrlImagen,
            request.PrecioBase,
            request.IncrementoMinimo,
            request.FechaInicio!.Value,
            request.FechaFin!.Value);

        _commandRepository.Add(subasta);
        var saveResult = await _commandRepository.SaveAsync(cancellationToken);
        if (saveResult.IsSuccess)
        {
            throw new InvalidOperationException(saveResult.Info);
        }

        return new Success<AuctionDetailResponseDto>(Map(subasta));
    }

    public async Task<Result<AuctionDetailResponseDto?>> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var subasta = await _queryRepository.Query<Subasta>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return subasta is null ? new Success<AuctionDetailResponseDto?>(null) : new Success<AuctionDetailResponseDto?>(Map(subasta));
    }

    private static AuctionDetailResponseDto Map(Subasta subasta) => new()
    {
        Id = subasta.Id,
        VendedorId = subasta.VendedorId,
        CategoriaId = subasta.CategoriaId,
        Titulo = subasta.Titulo,
        Descripcion = subasta.Descripcion,
        UrlImagen = subasta.UrlImagen,
        PrecioBase = subasta.PrecioBase,
        IncrementoMinimo = subasta.IncrementoMinimo,
        FechaInicio = subasta.FechaInicio,
        FechaFin = subasta.FechaFin,
        Estado = subasta.Estado,
        Version = subasta.Version.ToArray()
    };
}
