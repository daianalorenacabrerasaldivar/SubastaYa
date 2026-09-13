using Domain.Common.ResultPattern;

namespace Application.Interfaces.Persistencia
{
    public interface IUnitOfWork
    {
        Task<Result<string>> SaveChangesAsync(CancellationToken cancellationToken = default);

        void DiscardChanges();
    }
}
