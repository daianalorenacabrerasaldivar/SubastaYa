using Application.Interfaces.Persistencia.Lectura;
using Domain.Common.ResultPattern;
using MediatR;

namespace Application.UseCases.Pujas.Query.ListarPorSubasta
{
    public sealed record ListBidsQuery(int SubastaId) : IRequest<Result<IReadOnlyList<BidHistoryItem>>>;
}
