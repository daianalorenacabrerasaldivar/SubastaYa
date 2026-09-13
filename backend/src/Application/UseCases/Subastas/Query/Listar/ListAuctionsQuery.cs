using Application.Common;
using Application.Interfaces.Persistencia.Lectura;
using Domain.Common.ResultPattern;
using MediatR;

namespace Application.UseCases.Subastas.Query.Listar
{
    public sealed class ListAuctionsQuery : IRequest<Result<PagedResponse<AuctionListItem>>>
    {
        public AuctionStatusFilter? Status { get; set; }

        public int? CategoryId { get; set; }

        public decimal? MinPrice { get; set; }

        public decimal? MaxPrice { get; set; }

        public AuctionSortBy SortBy { get; set; } = AuctionSortBy.TiempoRestante;

        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 20;
    }
}
