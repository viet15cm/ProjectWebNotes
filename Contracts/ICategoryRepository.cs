using Entities;
using Entities.Models;
using ExtentionLinqEntitys;

namespace Contracts
{
    public interface ICategoryRepository : IRepositoryBase<Category>
    {       
        Task<IEnumerable<Category>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<Category>> GetAllWithDetailAsync(IExpLinqEntity<Category> expLinqEntity = default , CancellationToken cancellationToken = default);
        Task<Category> GetByIdAsync(string categoryId, CancellationToken cancellationToken = default);
        Task<Category> GetByIdWithDetailAsync(string categoryId, IExpLinqEntity<Category> expLinqEntity = default, CancellationToken cancellationToken = default);
        
        void Edit(Category category);

        void Insert(Category category);

        void Remove(Category category);

    }
}
