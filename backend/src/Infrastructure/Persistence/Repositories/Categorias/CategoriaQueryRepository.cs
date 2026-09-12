using Application.Interfaces.Persistencia;
using Domain.Entity;
using Infrastructure.Persistence.Context;
using Infrastructure.Persistence.Repositories.Common;

namespace Infrastructure.Persistence.Repositories.Categorias
{
    public sealed class CategoriaQueryRepository : EfQueryRepository<Categoria>, ICategoriaQueryRepository
    {
        public CategoriaQueryRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
