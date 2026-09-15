using Application.Interfaces.Persistencia;
using Domain.Entity;
using Infrastructure.Persistence.Context;
using Infrastructure.Persistence.Repositories.Common;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories.Categorias
{
    public sealed class CategoriaQueryRepository : EfQueryRepository<Categoria>, ICategoriaQueryRepository
    {
        public CategoriaQueryRepository(ApplicationDbContext context) : base(context)
        {
        }

        public Task<IReadOnlyList<Categoria>> ListAllAsync(CancellationToken cancellationToken)
        {
            return Query.OrderBy(c => c.Nombre)
                .ToListAsync(cancellationToken)
                .ContinueWith(t => (IReadOnlyList<Categoria>)t.Result,
                    TaskContinuationOptions.ExecuteSynchronously);
        }
    }
}
