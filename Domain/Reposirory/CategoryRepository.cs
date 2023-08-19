
using Contracts;
using Domain.Reposirory;
using Entities;
using Entities.Models;
using ExtentionLinqEntitys;
using Microsoft.EntityFrameworkCore;
using System.Threading;

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

        public IEnumerable<Category> GetAll(IExtendedQuery<Category> expLinqEntity = null)
        {
            return  Queryable(expLinqEntity).ToList();
        }

        public async Task<IEnumerable<Category>> GetAllAsync(IExtendedQuery<Category> expLinqEntity = null , CancellationToken cancellationToken = default)
        {
            return await Queryable(expLinqEntity).ToListAsync(cancellationToken);
        }

        public Category GetById(string categoryId, IExtendedQuery<Category> expLinqEntity = null)
        {
            return  Queryable(expLinqEntity).Where(x => x.Id.Equals(categoryId)).FirstOrDefault();
        }

      

        public void Insert(Category category)
        {
            Create(category);
        }

        public void Remove(Category category)
        {
            Delete(category);
        }

        public async Task<Category> GetByIdAsync(string categoryId, IExtendedQuery<Category> expLinqEntity = null, CancellationToken cancellationToken = default)
        {
            return await Queryable(expLinqEntity).Where(x => x.Id.Equals(categoryId)).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<Category> GetBySlugAsync(string slug, IExtendedQuery<Category> expLinqEntity = null, CancellationToken cancellationToken = default)
        {
            return await Queryable(expLinqEntity).Where(x => x.Slug.Equals(slug)).FirstOrDefaultAsync(cancellationToken);
        }
    }
}
