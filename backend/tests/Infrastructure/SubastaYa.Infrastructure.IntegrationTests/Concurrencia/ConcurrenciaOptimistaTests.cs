using Domain.Common.ResultPattern;
using Domain.Enum;
using Infrastructure.Persistence.Context;
using Infrastructure.Persistence.Repositories.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace SubastaYa.Infrastructure.IntegrationTests.Concurrencia
{
    public sealed class ConcurrenciaOptimistaTests
    {
        private static ApplicationDbContext CreateContext()
        {
            var configuration = new ConfigurationBuilder()
                .AddUserSecrets<ConcurrenciaOptimistaTests>(optional: true)
                .AddEnvironmentVariables()
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Falta ConnectionStrings:DefaultConnection (user-secrets o variable de entorno).");

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer(connectionString)
                .Options;

            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task DosContextosModificanLaMismaSubasta_ElSegundoRecibeConflict()
        {
            using var primero = CreateContext();
            using var segundo = CreateContext();

            var subastaEnPrimero = await primero.Subastas.FirstAsync(s => s.Estado == EstadoSubasta.ACTIVA);
            var subastaEnSegundo = await segundo.Subastas.FirstAsync(s => s.Id == subastaEnPrimero.Id);
            var fechaFinOriginal = subastaEnPrimero.FechaFin;

            subastaEnPrimero.FechaFin = fechaFinOriginal.AddMinutes(1);
            subastaEnSegundo.FechaFin = fechaFinOriginal.AddMinutes(2);

            var resultadoPrimero = await new UnitOfWork(primero).SaveChangesAsync();
            var resultadoSegundo = await new UnitOfWork(segundo).SaveChangesAsync();

            resultadoPrimero.IsSuccess.Should().BeTrue();
            resultadoSegundo.IsSuccess.Should().BeFalse();
            resultadoSegundo.Status.Should().Be(DataStatus.Conflict);

            using var limpieza = CreateContext();
            var subasta = await limpieza.Subastas.FirstAsync(s => s.Id == subastaEnPrimero.Id);
            subasta.FechaFin = fechaFinOriginal;
            await limpieza.SaveChangesAsync();
        }

        [Fact]
        public async Task UnSoloContexto_GuardaSinConflicto()
        {
            using var contexto = CreateContext();

            var subasta = await contexto.Subastas.FirstAsync(s => s.Estado == EstadoSubasta.ACTIVA);
            var fechaFinOriginal = subasta.FechaFin;
            subasta.FechaFin = fechaFinOriginal.AddMinutes(1);

            var resultado = await new UnitOfWork(contexto).SaveChangesAsync();

            resultado.IsSuccess.Should().BeTrue();

            subasta.FechaFin = fechaFinOriginal;
            await contexto.SaveChangesAsync();
        }
    }
}
