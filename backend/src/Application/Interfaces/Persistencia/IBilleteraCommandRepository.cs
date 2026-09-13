using Domain.Entity;

namespace Application.Interfaces.Persistencia
{
    public interface IBilleteraCommandRepository : ICommandRepository<Billetera>
    {
        Task<Billetera?> GetByUsuarioIdAsync(int usuarioId, CancellationToken cancellationToken);
    }
}
