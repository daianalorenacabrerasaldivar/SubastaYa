using Application.Interfaces.Persistencia;
using Domain.Common.ResultPattern;
using Domain.Entity;
using FluentValidation;
using MediatR;
using System.Text.Json;

namespace Application.UseCases.Pujas.Command.Ofertar
{
    public sealed class PlaceBidHandler : IRequestHandler<PlaceBidCommand, Result<PlaceBidResponse>>
    {
        private const string MensajeLiberacionFallida = "No se pudo liberar la retención del postor anterior.";

        private const string MensajeGuardadoFallido = "No se pudo registrar la puja. Intentá de nuevo.";

        private readonly ISubastaCommandRepository _subastaRepository;
        private readonly IBilleteraCommandRepository _billeteraRepository;
        private readonly IAuditoriaLogCommandRepository _auditoriaRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<PlaceBidCommand> _validator;

        public PlaceBidHandler(
            ISubastaCommandRepository subastaRepository,
            IBilleteraCommandRepository billeteraRepository,
            IAuditoriaLogCommandRepository auditoriaRepository,
            IUnitOfWork unitOfWork,
            IValidator<PlaceBidCommand> validator)
        {
            _subastaRepository = subastaRepository;
            _billeteraRepository = billeteraRepository;
            _auditoriaRepository = auditoriaRepository;
            _unitOfWork = unitOfWork;
            _validator = validator;
        }

        public async Task<Result<PlaceBidResponse>> Handle(PlaceBidCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                var errors = string.Join(" | ", validationResult.Errors.Select(x => x.ErrorMessage));
                return new Failed<PlaceBidResponse>(errors, DataStatus.RequestValidation);
            }

            var ahora = DateTime.UtcNow;

            var subasta = await _subastaRepository.GetForBiddingAsync(request.SubastaId, cancellationToken);

            if (subasta is null)
            {
                return new Failed<PlaceBidResponse>($"No existe la subasta con Id {request.SubastaId}.", DataStatus.NotFound);
            }

            if (!subasta.EstaAbiertaParaPujas(ahora))
            {
                return await RechazarAsync(subasta, request, "La subasta no está abierta para pujas.", DataStatus.Conflict, ahora, cancellationToken);
            }

            if (request.CompradorId == subasta.VendedorId)
            {
                return await RechazarAsync(subasta, request, "El vendedor no puede pujar en su propia subasta.", DataStatus.BusinessRule, ahora, cancellationToken);
            }

            var pujaLider = subasta.PujaLider;

            if (pujaLider?.CompradorId == request.CompradorId)
            {
                return await RechazarAsync(subasta, request, "Ya sos el postor líder de esta subasta.", DataStatus.BusinessRule, ahora, cancellationToken);
            }

            var montoMinimo = subasta.MontoMinimoProximaPuja();

            if (request.Monto < montoMinimo)
            {
                return await RechazarAsync(subasta, request, $"El monto mínimo para pujar es {montoMinimo}.", DataStatus.BusinessRule, ahora, cancellationToken);
            }

            var billetera = await _billeteraRepository.GetByUsuarioIdAsync(request.CompradorId, cancellationToken);

            if (billetera is null)
            {
                var motivoSinBilletera = $"El usuario {request.CompradorId} no tiene billetera.";
                _ = await AuditarRechazoAsync(subasta.Id, request, "PujaRechazada", motivoSinBilletera, null, ahora, cancellationToken);
                return new Failed<PlaceBidResponse>(motivoSinBilletera, DataStatus.NotFound);
            }

            if (!billetera.PuedeRetener(request.Monto))
            {
                return await RechazarAsync(subasta, request, $"Saldo disponible insuficiente: {billetera.SaldoDisponible}.", DataStatus.BusinessRule, ahora, cancellationToken);
            }

            if (pujaLider is not null)
            {
                var billeteraAnterior = await _billeteraRepository.GetByUsuarioIdAsync(pujaLider.CompradorId, cancellationToken);

                if (billeteraAnterior is null)
                {
                    _ = await AuditarRechazoAsync(subasta.Id, request, "PujaRechazada", $"El postor líder {pujaLider.CompradorId} no tiene billetera.", request.CompradorId, ahora, cancellationToken);
                    return new Failed<PlaceBidResponse>(MensajeLiberacionFallida, DataStatus.Exception);
                }

                if (!billeteraAnterior.PuedeLiberar(pujaLider.Monto))
                {
                    _ = await AuditarRechazoAsync(subasta.Id, request, "PujaRechazada", $"No se puede liberar {pujaLider.Monto} de la billetera del postor líder {pujaLider.CompradorId}: saldo retenido {billeteraAnterior.SaldoRetenido}.", request.CompradorId, ahora, cancellationToken);
                    return new Failed<PlaceBidResponse>(MensajeLiberacionFallida, DataStatus.Exception);
                }

                billeteraAnterior.Liberar(pujaLider.Monto, subasta.Id, ahora);
            }

            billetera.Retener(request.Monto, subasta.Id, ahora);

            var puja = subasta.RegistrarPuja(request.CompradorId, request.Monto, ahora);
            var extendida = subasta.AplicarAntiSniping(ahora);

            _subastaRepository.Update(subasta);

            if (extendida)
            {
                _auditoriaRepository.Add(new AuditoriaLog
                {
                    Entidad = nameof(Subasta),
                    EntidadId = subasta.Id,
                    Accion = "ExtensionAntiSniping",
                    UsuarioId = request.CompradorId,
                    DetalleJson = JsonSerializer.Serialize(new { puja.Monto, NuevaFechaFin = subasta.FechaFin }),
                    Fecha = ahora
                });
            }

            var saveResult = await _unitOfWork.SaveChangesAsync(cancellationToken);

            if (!saveResult.IsSuccess)
            {
                _unitOfWork.DiscardChanges();

                if (saveResult.Status == DataStatus.Conflict)
                {
                    _ = await AuditarRechazoAsync(subasta.Id, request, "PujaRechazadaConcurrencia", saveResult.Info, request.CompradorId, ahora, cancellationToken);
                    return new Failed<PlaceBidResponse>(saveResult.Info, DataStatus.Conflict);
                }

                _ = await AuditarRechazoAsync(subasta.Id, request, "PujaRechazadaError", saveResult.Info, request.CompradorId, ahora, cancellationToken);
                return new Failed<PlaceBidResponse>(MensajeGuardadoFallido, DataStatus.Exception);
            }

            var response = new PlaceBidResponse(
                puja.Id,
                subasta.Id,
                puja.CompradorId,
                puja.Monto,
                puja.FechaPuja,
                puja.Monto,
                subasta.Pujas.Count,
                subasta.MontoMinimoProximaPuja(),
                subasta.FechaFin,
                extendida);

            return new Success<PlaceBidResponse>(response);
        }

        private async Task<Result<PlaceBidResponse>> RechazarAsync(
            Subasta subasta,
            PlaceBidCommand request,
            string motivo,
            DataStatus status,
            DateTime ahora,
            CancellationToken cancellationToken)
        {
            _ = await AuditarRechazoAsync(subasta.Id, request, "PujaRechazada", motivo, request.CompradorId, ahora, cancellationToken);
            return new Failed<PlaceBidResponse>(motivo, status);
        }

        private async Task<Result<string>> AuditarRechazoAsync(
            int subastaId,
            PlaceBidCommand request,
            string accion,
            string motivo,
            int? usuarioId,
            DateTime ahora,
            CancellationToken cancellationToken)
        {
            _auditoriaRepository.Add(new AuditoriaLog
            {
                Entidad = nameof(Subasta),
                EntidadId = subastaId,
                Accion = accion,
                UsuarioId = usuarioId,
                DetalleJson = JsonSerializer.Serialize(new { request.CompradorId, request.Monto, Motivo = motivo }),
                Fecha = ahora
            });

            return await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
