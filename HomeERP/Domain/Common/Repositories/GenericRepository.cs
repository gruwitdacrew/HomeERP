using HomeERP.Domain.Common.Contexts;
using HomeERP.Domain.Common.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeERP.Domain.Common.Repositories
{
    public class GenericRepository<T>(AppDBContext context) where T : class
    {
        protected readonly AppDBContext Context = context;
        protected readonly DbSet<T> DbSet = context.Set<T>();

        public IQueryable<T> Query()
        {
            return DbSet.AsQueryable();
        }

        public void Add(T entity)
        {
            DbSet.Add(entity);
            Context.SaveChanges();
        }

        public void Update(T entity)
        {
            DbSet.Update(entity);
            Context.SaveChanges();
        }

        public void Delete(T entity)
        {
            DbSet.Remove(entity);
            Context.SaveChanges();
        }
    }
}
