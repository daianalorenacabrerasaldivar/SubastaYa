//using Aplication.Common.Interface;
//using Application.Dto.Auctions;
//using Application.Interfaces;
//using Aplication.Common.Interface;
//using Domain.Common.ResultPattern;
//using Domain.Entity;
//using MediatR;
//using Microsoft.EntityFrameworkCore;

//namespace Application.UseCases.Subasta.Query.GetAuctionById
//{
//    public class GetAuctionByIdHandler : IRequestHandler<GetAuctionByIdQuery, Result<AuctionDetailResponseDto?>>
//    {
//        private readonly IRepositoryQuery _queryRepository;

//        public GetAuctionByIdHandler(IRepositoryQuery queryRepository)
//        {
//            _queryRepository = queryRepository;
//        }

//        public async Task<Result<AuctionDetailResponseDto?>> Handle(
//            GetAuctionByIdQuery request,
//            CancellationToken cancellationToken)
//        {
//            var subasta = await _queryRepository.Query<Subasta>()
//                .AsNoTracking()
//                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

//            if (subasta is null)
//            {
//                return new Success<AuctionDetailResponseDto?>(null);
//            }

//            return new Success<AuctionDetailResponseDto?>(Map(subasta));
//        }

//        private static AuctionDetailResponseDto Map(Subasta subasta) => new()
//        {
//            Id = subasta.Id,
//            VendedorId = subasta.VendedorId,
//            CategoriaId = subasta.CategoriaId,
//            Titulo = subasta.Titulo,
//            Descripcion = subasta.Descripcion,
//            UrlImagen = subasta.UrlImagen,
//            PrecioBase = subasta.PrecioBase,
//            IncrementoMinimo = subasta.IncrementoMinimo,
//            FechaInicio = subasta.FechaInicio,
//            FechaFin = subasta.FechaFin,
//            Estado = subasta.Estado,
//            Version = subasta.Version.ToArray()
//        };
//    }
//}
