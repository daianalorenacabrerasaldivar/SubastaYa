using Aplication.Common.Interface;
using Domain.Common.ResultPattern;
using Microsoft.EntityFrameworkCore;

namespace Infraestructure.Persistencia.Repository
{
    public class RepositoryCommand : IRepositoryCommand
    {
        private readonly DbContext _context;

        public RepositoryCommand(DbContext context)
        {
            _context = context;
        }

        public void Add<TEntity>(TEntity entity) where TEntity : class
        {
            _context.Set<TEntity>().Add(entity);
        }

        public async Task<Result<string>> SaveAsync()
        {
            try
            {
                var nroRegistrosModificados = await _context.SaveChangesAsync();
                if (nroRegistrosModificados > 0)
                {
                    return new Success<string>($"Se guardaron {nroRegistrosModificados} cambios exitosamente");
                }
                else
                {
                    return new Failed<string>("No se realizaron cambios en la base de datos");
                }
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"Error al guardar los cambios: {ex.Message}");
                return new Failed<string>($"Error al guardar los cambios: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inesperado: {ex.Message}");
                return new Failed<string>($"Error inesperado: {ex.Message}");
            }
        }

        public void Update<T>(T entity) where T : class
        {
            _context.Set<T>().Update(entity);
        }
        public void Remove<T>(T entity) where T : class
        {
            _context.Set<T>().Remove(entity);
        }
    }
}