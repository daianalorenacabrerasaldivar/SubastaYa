using Application.Interfaces.Persistencia.Lectura;
using Domain.Common.ResultPattern;
using MediatR;

namespace Application.UseCases.Billetera.Query.ConsultarSaldo
{
    public sealed record GetWalletBalanceQuery(int UsuarioId) : IRequest<Result<WalletBalanceResult>>;
}
