
using Contracts;
using Domain.Reposirory;
using Entities;
using Entities.Models;
using ExtentionLinqEntitys;
using Microsoft.EntityFrameworkCore;

namespace Domain.Repository
{
    public class CategoryRepository : RepositoryBase<Category>, ICategoryRepository
    {
        public CategoryRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
        }

        public void Edit(Category category)
        {
            Update(category);
        }

        public async Task<IEnumerable<Category>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var categorys = await FindAll()
                                   .OrderBy(x => x.Serial).
                                   ToListAsync();

            return categorys;
        }

        public async Task<IEnumerable<Category>> GetAllWithDetailAsync(IExpLinqEntity<Category> expLinqEntity = null, CancellationToken cancellationToken = default)
        {
            return await Queryable(expLinqEntity).ToListAsync();
        }

        public async Task<Category> GetByIdAsync(string categoryId, CancellationToken cancellationToken = default)
        {
            return await FindByCondition(x => x.Id == categoryId).FirstOrDefaultAsync();
        }

        public async Task<Category> GetByIdWithDetailAsync(string categoryId, IExpLinqEntity<Category> expLinqEntity = null, CancellationToken cancellationToken = default)
        {
            var c = ExpLinqEntity<Category>.ResLinqEntity(ExpExpressions.ExtendInclude<Category>(x => x.Include(x => x.PostCategories).ThenInclude(x => x.Post)));

            return await Queryable(expLinqEntity).Where(x => x.Id == categoryId).FirstOrDefaultAsync();
        }

        public void Insert(Category category)
        {
            Create(category);
        }

        public void Remove(Category category)
        {
            Delete(category);
        }

      
    }
}
