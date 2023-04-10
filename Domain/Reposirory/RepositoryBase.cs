using System.Collections.Generic;
using System.Linq.Expressions;
using Contracts;
using Domain.Reposirory;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Domain.Repository
{
    public abstract  class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        protected RepositoryContext RepositoryContext { get; set; }
        public RepositoryBase(RepositoryContext repositoryContext)
        {
            RepositoryContext = repositoryContext;
        }
        public IQueryable<T> FindAll() => RepositoryContext.Set<T>().AsNoTracking();
    
        public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression) =>
            RepositoryContext.Set<T>().Where(expression).AsNoTracking();
        public void Create(T entity) => RepositoryContext.Set<T>().Add(entity);
        public void Update(T entity) => RepositoryContext.Set<T>().Update(entity);
        public void Delete(T entity) => RepositoryContext.Set<T>().Remove(entity);

        public IQueryable<T> Include(params Expression<Func<T, object>>[] includeExpressions)
        {
            DbSet<T> dbSet = RepositoryContext.Set<T>();

            IQueryable<T> query = null;
            foreach (var includeExpression in includeExpressions)
            {
                query = dbSet.Include(includeExpression);
            }

            return query ?? dbSet;
        }
    }
}
