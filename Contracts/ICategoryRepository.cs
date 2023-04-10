using Entities.Models;
using System.Collections;
using System.Linq.Expressions;

namespace Contracts
{
    public interface ICategoryRepository : IRepositoryBase<Category>
    {
        Task<Category> CategoryInclue(string categoryId , Expression<Func<Category, object>>[] includeExpressions = default);
        Task<IEnumerable<Category>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<Category>> GetAllWithDetailAsync(CancellationToken cancellationToken = default);

        Task<Category> GetByIdAsync(string categoryId, CancellationToken cancellationToken = default);

        Task<Category> GetByIdWithDetailAsync(string categoryId, CancellationToken cancellationToken = default);

        void Edit(Category category);

        void Insert(Category category);

        void Remove(Category category);



    }
}
