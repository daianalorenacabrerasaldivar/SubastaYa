using Domain.Common.ResultPattern;
using MediatR;
using Application.Dto.Auctions;

namespace Application.UseCases.Subasta.Query.GetAuctionById
{
    public class GetAuctionByIdQuery : IRequest<Result<AuctionDetailResponseDto?>>
    {
        public int Id { get; set; }

        public GetAuctionByIdQuery(int id)
        {
            Id = id;
        }
    }
}
