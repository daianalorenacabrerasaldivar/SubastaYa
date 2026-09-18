using Domain.Entity;

namespace Application.Interfaces.Persistencia
{
    public interface ISubastaCommandRepository : ICommandRepository<Subasta>
    {
        Task<Subasta?> GetForBiddingAsync(int id, CancellationToken cancellationToken);

        Task<IReadOnlyList<Subasta>> GetVencidasAsync(DateTime ahora, CancellationToken cancellationToken);

        Task<IReadOnlyList<Subasta>> GetProgramadasParaActivarAsync(DateTime ahora, CancellationToken cancellationToken);
    }
}
