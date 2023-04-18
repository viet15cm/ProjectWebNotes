using Entities;
using ExtentionLinqEntitys;
using System.Linq.Expressions;

namespace Contracts
{
    public interface IRepositoryBase<T> where T : class
    {
        IQueryable<T> FindAll();
        IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression);
        public IQueryable<T> Queryable(IExpLinqEntity<T> expLinqEntity = default);
        void Create(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}
