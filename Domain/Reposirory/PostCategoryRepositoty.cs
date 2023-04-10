using Contracts;
using Domain.Repository;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Reposirory
{
    public class PostCategoryRepositoty : RepositoryBase<PostCategory>, IPostCategoryRepository
    {
        public PostCategoryRepositoty(RepositoryContext repositoryContext) : base(repositoryContext)
        {
        }

        public void Edit(PostCategory postCategory)
        {
            Update(postCategory);
        }

        public async Task<IEnumerable<PostCategory>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await FindAll().ToListAsync();
        }

        public async Task<IEnumerable<PostCategory>> GetAllWithDetailAsync(CancellationToken cancellationToken = default)
        {
           return await FindAll().
                        Include(pc => pc.Category)
                       .Include(pc => pc.Post).ToListAsync();
        }

        public async Task<PostCategory> GetByIdAsync(string categoryId, string postId, CancellationToken cancellationToken = default)
        {
            return await FindByCondition(pc => pc.PostID == postId && pc.CategoryID == categoryId)
                        .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<PostCategory>> GetByIdCategoryWithDetailAsync(string categoryId, CancellationToken cancellationToken = default)
        {

            return await FindByCondition(x => x.CategoryID == categoryId)
                        .Include(x => x.Post)
                        .Include(x => x.Category)
                        .ToListAsync();

        }

        public async Task<IEnumerable<PostCategory>> GetByIdPostWithDetailAsync(string postId, CancellationToken cancellationToken = default)
        {


            return await FindByCondition(x => x.PostID == postId)
                        .Include(x => x.Post)
                        .Include(x => x.Category)
                        .ToListAsync();

        }

        public async Task<PostCategory> GetByIdWithDetailAsync(string categoryId, string postId, CancellationToken cancellationToken = default)
        {
            return await FindByCondition(x => x.PostID == postId && x.CategoryID == categoryId)
                        .Include(x => x.Post)
                        .Include(x => x.Category)
                        .FirstOrDefaultAsync();
        }

        public void Insert(PostCategory postCategory)
        {
            Create(postCategory);
        }

        public void Remove(PostCategory postCategory)
        {
            Delete(postCategory);
        }
    }
}
