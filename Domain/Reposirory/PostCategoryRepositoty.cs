﻿using Contracts;
using Domain.Repository;
using Entities.Models;
using ExtentionLinqEntitys;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;

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

        public async Task<IEnumerable<PostCategory>> GetAllAsync(IExtendedQuery<PostCategory> expLinqEntity = null, CancellationToken cancellationToken = default)
        {
            return await Queryable(expLinqEntity).ToListAsync();
        }

        public async Task<PostCategory> GetByIdAsync(string categoryId, string postId, IExtendedQuery<PostCategory> expLinqEntity, CancellationToken cancellationToken = default)
        {
            return await Queryable(expLinqEntity).Where(pc => pc.PostID == postId && pc.CategoryID == categoryId)
                        .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<PostCategory>> GetByIdCategoryAsync(string categoryId, IExtendedQuery<PostCategory> expLinqEntity = null, CancellationToken cancellationToken = default)
        {
           return  await Queryable(expLinqEntity).Where(x => x.CategoryID.Equals(categoryId))
                   .ToListAsync();
        }


        public async Task<IEnumerable<PostCategory>> GetByIdPostAsync(string postId, IExtendedQuery<PostCategory> expLinqEntity = null, CancellationToken cancellationToken = default)
        {
            return await Queryable(expLinqEntity).Where(x => x.PostID.Equals(postId)).ToListAsync();
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
