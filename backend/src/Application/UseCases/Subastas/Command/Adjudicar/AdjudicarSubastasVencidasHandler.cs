using Application.Interfaces.Persistencia;
using Domain.Common.ResultPattern;
using Domain.Entity;
using MediatR;
using System.Text.Json;

namespace Application.UseCases.Subastas.Command.Adjudicar
{
    public sealed class AdjudicarSubastasVencidasHandler
        : IRequestHandler<AdjudicarSubastasVencidasCommand, Result<AdjudicacionResult>>
    {
        private readonly ISubastaCommandRepository _subastaRepository;
        private readonly IBilleteraCommandRepository _billeteraRepository;
        private readonly IAuditoriaLogCommandRepository _auditoriaRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AdjudicarSubastasVencidasHandler(
            ISubastaCommandRepository subastaRepository,
            IBilleteraCommandRepository billeteraRepository,
            IAuditoriaLogCommandRepository auditoriaRepository,
            IUnitOfWork unitOfWork)
        {
            _subastaRepository = subastaRepository;
            _billeteraRepository = billeteraRepository;
            _auditoriaRepository = auditoriaRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<AdjudicacionResult>> Handle(
            AdjudicarSubastasVencidasCommand request,
            CancellationToken cancellationToken)
        {
            var ahora = DateTime.UtcNow;
            int finalizadas = 0, desiertas = 0, activadas = 0;

            // 1. Activar subastas PROGRAMADAS cuya fecha de inicio ya pasó
            var programadas = await _subastaRepository.GetProgramadasParaActivarAsync(ahora, cancellationToken);
            foreach (var subasta in programadas)
            {
                subasta.Activar();
                _subastaRepository.Update(subasta);
                activadas++;
            }

            // 2. Adjudicar subastas ACTIVAS vencidas
            var vencidas = await _subastaRepository.GetVencidasAsync(ahora, cancellationToken);
            foreach (var subasta in vencidas)
            {
                if (subasta.PuedeFinalizar(ahora))
                {
                    await AdjudicarConGanadorAsync(subasta, ahora, cancellationToken);
                    finalizadas++;
                }
                else if (subasta.PuedeMarcarDesierta(ahora))
                {
                    AdjudicarDesierta(subasta, ahora);
                    desiertas++;
                }
            }

            if (finalizadas == 0 && desiertas == 0 && activadas == 0)
                return new Success<AdjudicacionResult>(new AdjudicacionResult(0, 0, 0));

            var saveResult = await _unitOfWork.SaveChangesAsync(cancellationToken);

            if (!saveResult.IsSuccess)
                return new Failed<AdjudicacionResult>(
                    "Error al persistir la adjudicación.",
                    saveResult.Status);

            return new Success<AdjudicacionResult>(new AdjudicacionResult(finalizadas, desiertas, activadas));
        }

        private async Task AdjudicarConGanadorAsync(
            Subasta subasta, DateTime ahora, CancellationToken cancellationToken)
        {
            var ganador = subasta.PujaLider!;

            var billeteraComprador = await _billeteraRepository
                .GetByUsuarioIdAsync(ganador.CompradorId, cancellationToken);
            var billeteraVendedor = await _billeteraRepository
                .GetByUsuarioIdAsync(subasta.VendedorId, cancellationToken);

            if (billeteraComprador is null || billeteraVendedor is null)
                return;

            subasta.Finalizar(ahora);
            billeteraComprador.Pagar(ganador.Monto, subasta.Id, ahora);
            billeteraVendedor.Cobrar(ganador.Monto, subasta.Id, ahora);

            _subastaRepository.Update(subasta);
            _billeteraRepository.Update(billeteraComprador);
            _billeteraRepository.Update(billeteraVendedor);

            _auditoriaRepository.Add(new AuditoriaLog
            {
                Entidad = nameof(Subasta),
                EntidadId = subasta.Id,
                Accion = "SubastaFinalizada",
                UsuarioId = null,
                DetalleJson = JsonSerializer.Serialize(new
                {
                    GanadorId = ganador.CompradorId,
                    MontoFinal = ganador.Monto,
                    subasta.VendedorId
                }),
                Fecha = ahora
            });
        }

        private void AdjudicarDesierta(Subasta subasta, DateTime ahora)
        {
            subasta.MarcarDesierta(ahora);
            _subastaRepository.Update(subasta);

            _auditoriaRepository.Add(new AuditoriaLog
            {
                Entidad = nameof(Subasta),
                EntidadId = subasta.Id,
                Accion = "SubastaDesierta",
                UsuarioId = null,
                DetalleJson = JsonSerializer.Serialize(new { FechaVencimiento = subasta.FechaFin }),
                Fecha = ahora
            });
        }
    }
}
