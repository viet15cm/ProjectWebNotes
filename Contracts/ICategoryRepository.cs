using Entities;
using Entities.Models;
using ExtentionLinqEntitys;

namespace Contracts
{
    public interface ICategoryRepository : IRepositoryBase<Category>
    {
        IEnumerable<Category> GetAll(IExpLinqEntity<Category> expLinqEntity = default);

        Category GetById(string categoryId, IExpLinqEntity<Category> expLinqEntity = default);
        
        Task<IEnumerable<Category>> GetAllAsync(IExpLinqEntity<Category> expLinqEntity = default ,CancellationToken cancellationToken = default);
     
        Task<Category> GetByIdAsync(string categoryId , IExpLinqEntity<Category> expLinqEntity = default, CancellationToken cancellationToken = default);
           
        void Edit(Category category);

        void Insert(Category category);

        void Remove(Category category);

    }
}
