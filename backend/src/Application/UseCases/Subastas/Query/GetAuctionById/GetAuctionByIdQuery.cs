using Domain.Common.ResultPattern;
using MediatR;

namespace Application.UseCases.Subastas.Query.GetAuctionById
{
    public sealed record GetAuctionByIdQuery(int Id) : IRequest<Result<GetAuctionByIdResponse>>;
}
