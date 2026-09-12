using Application.Interfaces.Persistencia;
using Domain.Common.ResultPattern;
using Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repository
{

    public class UsuarioQuery: IUsuarioQuery
    {
        private readonly IRepositoryQuery _repositoryQuery;

        public UsuarioQuery(IRepositoryQuery repositoryQuery)
        {
            _repositoryQuery = repositoryQuery
                ?? throw new ArgumentNullException(nameof(repositoryQuery));
        }

        public async Task<Result<Usuario>> BuscarPorIdAsync(
            int usuarioId,
            CancellationToken cancellationToken)
        {
            var usuario = await _repositoryQuery
                .Query<Usuario>()
                .FirstOrDefaultAsync(
                    u => u.Id == usuarioId,
                    cancellationToken);

            if (usuario is null)
            {
                return new Failed<Usuario>(
                    $"No existe el usuario con Id {usuarioId}.",
                    DataStatus.NotFound);
            }

            return new Success<Usuario>(usuario);
        }
    }


}
