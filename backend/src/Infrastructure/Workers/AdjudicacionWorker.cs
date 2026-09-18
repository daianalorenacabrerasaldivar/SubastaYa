using Application.UseCases.Subastas.Command.Adjudicar;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Workers
{
    public sealed class AdjudicacionWorker : BackgroundService
    {
        private static readonly TimeSpan Interval = TimeSpan.FromSeconds(30);

        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<AdjudicacionWorker> _logger;

        public AdjudicacionWorker(IServiceScopeFactory scopeFactory, ILogger<AdjudicacionWorker> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("AdjudicacionWorker iniciado. Intervalo: {Intervalo}s.", Interval.TotalSeconds);

            using var timer = new PeriodicTimer(Interval);

            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                await ProcessTickAsync(stoppingToken);
            }
        }

        private async Task ProcessTickAsync(CancellationToken stoppingToken)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var sender = scope.ServiceProvider.GetRequiredService<ISender>();

                var result = await sender.Send(new AdjudicarSubastasVencidasCommand(), stoppingToken);

                if (result.IsSuccess)
                {
                    var r = result.Value;
                    if (r.Finalizadas > 0 || r.Desiertas > 0 || r.Activadas > 0)
                        _logger.LogInformation(
                            "Adjudicación completada: {F} finalizada(s), {D} desierta(s), {A} activada(s).",
                            r.Finalizadas, r.Desiertas, r.Activadas);
                }
                else
                {
                    _logger.LogWarning("Adjudicación falló: {Info}", result.Info);
                }
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "Error inesperado en AdjudicacionWorker.");
            }
        }
    }
}
