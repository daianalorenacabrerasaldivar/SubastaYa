using Application.Interfaces.Persistencia.Lectura;
using Domain.Common.ResultPattern;
using MediatR;

namespace Application.UseCases.Billetera.Query.ListarTransacciones
{
    public sealed record ListWalletTransactionsQuery(int UsuarioId) : IRequest<Result<IReadOnlyList<LedgerTransactionItem>>>;
}
