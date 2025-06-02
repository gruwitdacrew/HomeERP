using HomeERP.Domain.Common.Contexts;
using HomeERP.Domain.Common.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeERP.Domain.Common.Repositories
{
    public class BaseEntityRepository<T>(AppDBContext context) where T : BaseEntity
    {
        protected readonly AppDBContext Context = context;
        protected readonly DbSet<T> DbSet = context.Set<T>();

        public T? GetBy(Guid id)
        {
            return DbSet.FirstOrDefault(x => x.Id == id);
        }

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

        public void UpdateRange(List<T> entityCollection)
        {
            DbSet.UpdateRange(entityCollection);
            Context.SaveChanges();
        }

        public void AddRange(List<T> entityCollection)
        {
            DbSet.AddRange(entityCollection);
            Context.SaveChanges();
        }

        public void Delete(T entity)
        {
            DbSet.Remove(entity);
            Context.SaveChanges();
        }
    }
}
