using Application.Interfaces.Persistencia;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repository.Common
{
    public class RepositoryCommand : IRepositoryCommand
    {
        private readonly DbContext _context;

        public RepositoryCommand(ApplicationDbContext context)
        {
            _context = context;
        }

        public void Add<TEntity>(TEntity entity) where TEntity : class
        {
            _context.Set<TEntity>().Add(entity);
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