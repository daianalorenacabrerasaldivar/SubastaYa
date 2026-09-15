using Application.Interfaces.Persistencia;
using Domain.Common.ResultPattern;
using MediatR;

namespace Application.UseCases.Billetera.Query.ConsultarSaldo
{
    public sealed class GetWalletBalanceHandler : IRequestHandler<GetWalletBalanceQuery, Result<GetWalletBalanceResponse>>
    {
        private readonly IBilleteraQueryRepository _billeteraRepository;

        public GetWalletBalanceHandler(IBilleteraQueryRepository billeteraRepository)
        {
            _billeteraRepository = billeteraRepository;
        }

        public async Task<Result<GetWalletBalanceResponse>> Handle(GetWalletBalanceQuery request, CancellationToken cancellationToken)
        {
            var billetera = await _billeteraRepository.GetByUsuarioIdAsync(request.UsuarioId, cancellationToken);

            if (billetera is null)
            {
                return new Failed<GetWalletBalanceResponse>(
                    $"No existe una billetera para el usuario con Id {request.UsuarioId}.",
                    DataStatus.NotFound);
            }

            return new Success<GetWalletBalanceResponse>(new GetWalletBalanceResponse(
                billetera.UsuarioId,
                billetera.SaldoTotal,
                billetera.SaldoRetenido,
                billetera.SaldoDisponible));
        }
    }
}
