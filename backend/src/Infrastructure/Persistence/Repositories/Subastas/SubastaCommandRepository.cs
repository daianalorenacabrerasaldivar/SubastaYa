using Application.Interfaces.Persistencia;
using Domain.Entity;
using Infrastructure.Persistence.Context;
using Infrastructure.Persistence.Repositories.Common;

namespace Infrastructure.Persistence.Repositories.Subastas
{
    public sealed class SubastaCommandRepository : EfCommandRepository<Subasta>, ISubastaCommandRepository
    {
        public SubastaCommandRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
