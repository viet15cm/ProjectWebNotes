using Entities;
using Entities.Models;
using ExtentionLinqEntitys;

namespace Contracts
{
    public interface ICategoryRepository : IRepositoryBase<Category>
    {
        IEnumerable<Category> GetAll(IExtendedQuery<Category> expLinqEntity = default);

        Category GetById(string categoryId, IExtendedQuery<Category> expLinqEntity = default);
        
        Task<IEnumerable<Category>> GetAllAsync(IExtendedQuery<Category> expLinqEntity = default ,CancellationToken cancellationToken = default);
     
        Task<Category> GetByIdAsync(string categoryId , IExtendedQuery<Category> expLinqEntity = default, CancellationToken cancellationToken = default);
        Task<Category> GetBySlugAsync(string slug, IExtendedQuery<Category> expLinqEntity = default, CancellationToken cancellationToken = default);
        void Edit(Category category);

        void Insert(Category category);

        void Remove(Category category);

    }
}
