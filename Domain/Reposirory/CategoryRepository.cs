using Contracts;
using Domain.Reposirory;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq.Expressions;

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
          
            var categorys =  await FindAll().
                                OrderBy(x => x.Serial).
                                ToListAsync();

            return categorys;

        }

        public async Task<IEnumerable<Category>> GetAllWithDetailAsync(CancellationToken cancellationToken = default)
        {
            var categorys = await FindAll()
                                  .OrderBy(x => x.Serial)
                                  .Include(c => c.PostCategories)      
                                  .ToListAsync();
            return categorys;
        }

        public async Task<Category> GetByIdAsync(string categoryId, CancellationToken cancellationToken = default)
        {
            return await FindByCondition(x => x.Id == categoryId).FirstOrDefaultAsync();
        }

        public async Task<Category> GetByIdWithDetailAsync(string categoryId, CancellationToken cancellationToken = default)
        {
            var category = await FindAll()                            
                                .Include(c => c.PostCategories)
                                .FirstOrDefaultAsync(c => c.Id == categoryId);
            return category;
        }

        public void Insert(Category category)
        {

            Create(category);
        }

        public async Task<Category> CategoryInclue(string categoryId, Expression<Func<Category, object>>[] includeExpressions = default)
        {
            DbSet<Category> dbSet = RepositoryContext.Set<Category>();

            IQueryable<Category> query = null;
            if (includeExpressions != null)
            {
                foreach (var includeExpression in includeExpressions)
                {
                    query = dbSet.Include(includeExpression);
                }
            }

            if (query != null)
            {
                return await query.FirstOrDefaultAsync(x => x.Id == categoryId);
            }

            return await dbSet?.FirstOrDefaultAsync(x => x.Id == categoryId);
        }

        public void Remove(Category category)
        {
            Delete(category);
        }

        
    }
}
