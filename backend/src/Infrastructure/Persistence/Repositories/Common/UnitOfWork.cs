using Application.Interfaces.Persistencia;
using Domain.Common.ResultPattern;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories.Common
{
    public sealed class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Result<string>> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var registrosModificados = await _context.SaveChangesAsync(cancellationToken);

                if (registrosModificados > 0)
                {
                    return new Success<string>(
                        $"Se guardaron {registrosModificados} cambios exitosamente");
                }

                return new Failed<string>(
                    "No se realizaron cambios en la base de datos",
                    DataStatus.Failed);
            }
            catch (DbUpdateConcurrencyException)
            {
                return new Failed<string>(
                    "El registro fue modificado por otro usuario. Volvé a cargarlo e intentá de nuevo.",
                    DataStatus.Conflict);
            }
            catch (DbUpdateException ex)
            {
                return new Failed<string>(
                    $"Error al guardar los cambios: {ex.InnerException?.Message ?? ex.Message}",
                    DataStatus.Failed);
            }
            catch (Exception ex)
            {
                return new Failed<string>(
                    $"Error inesperado: {ex.Message}",
                    DataStatus.Exception);
            }
        }
    }
}
