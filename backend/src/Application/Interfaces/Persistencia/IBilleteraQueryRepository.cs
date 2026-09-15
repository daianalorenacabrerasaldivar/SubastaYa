using Domain.Entity;

namespace Application.Interfaces.Persistencia
{
    public interface IBilleteraQueryRepository : IQueryRepository<Billetera>
    {
        Task<Billetera?> GetByUsuarioIdAsync(int usuarioId, CancellationToken cancellationToken);
    }
}
