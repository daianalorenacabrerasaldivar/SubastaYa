using Domain.Common.ResultPattern;
using Domain.Entity;

namespace Application.Interfaces.Persistencia
{
    public interface IUsuarioQuery
    {
        Task<Result<Usuario>> BuscarPorIdAsync(int usuarioId, CancellationToken cancellationToken);
    }
}