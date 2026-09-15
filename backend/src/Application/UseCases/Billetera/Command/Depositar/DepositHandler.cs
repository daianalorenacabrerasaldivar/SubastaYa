using Application.Interfaces.Persistencia;
using Domain.Common.ResultPattern;
using Domain.Entity;
using FluentValidation;
using MediatR;
using System.Text.Json;

namespace Application.UseCases.Billetera.Command.Depositar
{
    public sealed class DepositHandler : IRequestHandler<DepositCommand, Result<DepositResponse>>
    {
        private readonly IBilleteraCommandRepository _billeteraRepository;
        private readonly IAuditoriaLogCommandRepository _auditoriaRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<DepositCommand> _validator;

        public DepositHandler(
            IBilleteraCommandRepository billeteraRepository,
            IAuditoriaLogCommandRepository auditoriaRepository,
            IUnitOfWork unitOfWork,
            IValidator<DepositCommand> validator)
        {
            _billeteraRepository = billeteraRepository;
            _auditoriaRepository = auditoriaRepository;
            _unitOfWork = unitOfWork;
            _validator = validator;
        }

        public async Task<Result<DepositResponse>> Handle(DepositCommand request, CancellationToken cancellationToken)
        {
            var validation = await _validator.ValidateAsync(request, cancellationToken);

            if (!validation.IsValid)
            {
                var errors = string.Join(" | ", validation.Errors.Select(e => e.ErrorMessage));
                return new Failed<DepositResponse>(errors, DataStatus.RequestValidation);
            }

            var billetera = await _billeteraRepository.GetByUsuarioIdAsync(request.UsuarioId, cancellationToken);

            if (billetera is null)
                return new Failed<DepositResponse>(
                    $"No existe una billetera para el usuario con Id {request.UsuarioId}.",
                    DataStatus.NotFound);

            var ahora = DateTime.UtcNow;
            billetera.Depositar(request.Monto, ahora);

            _billeteraRepository.Update(billetera);

            _auditoriaRepository.Add(new AuditoriaLog
            {
                Entidad = nameof(Billetera),
                EntidadId = billetera.Id,
                Accion = "DepositoSaldo",
                UsuarioId = request.UsuarioId,
                DetalleJson = JsonSerializer.Serialize(new
                {
                    request.Monto,
                    SaldoTotalResultante = billetera.SaldoTotal
                }),
                Fecha = ahora
            });

            var saveResult = await _unitOfWork.SaveChangesAsync(cancellationToken);

            if (!saveResult.IsSuccess)
                return new Failed<DepositResponse>("No se pudo registrar el deposito. Intenta de nuevo.", saveResult.Status);

            return new Success<DepositResponse>(new DepositResponse(
                billetera.UsuarioId,
                request.Monto,
                billetera.SaldoTotal,
                billetera.SaldoRetenido,
                billetera.SaldoDisponible,
                ahora));
        }
    }
}
