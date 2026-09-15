using Domain.Common.ResultPattern;
using MediatR;

namespace Application.UseCases.Billetera.Command.Depositar
{
    public sealed class DepositCommand : IRequest<Result<DepositResponse>>
    {
        public int UsuarioId { get; set; }

        public decimal Monto { get; set; }
    }
}
