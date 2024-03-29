﻿using Entities;
using Entities.Models;
using ExtentionLinqEntitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IPostCategoryRepository
    {

      
        Task<IEnumerable<PostCategory>> GetAllAsync(IExtendedQuery<PostCategory> expLinqEntity = default ,CancellationToken cancellationToken = default);

        Task<PostCategory> GetByIdAsync(string categoryId, string postId , IExtendedQuery<PostCategory> expLinqEntity, CancellationToken cancellationToken = default);

        Task<IEnumerable<PostCategory>> GetByIdCategoryAsync(string categoryId , IExtendedQuery<PostCategory> expLinqEntity = default, CancellationToken cancellationToken = default);
        Task<IEnumerable<PostCategory>> GetByIdPostAsync(string postId, IExtendedQuery<PostCategory> expLinqEntity = default,  CancellationToken cancellationToken = default);

        void Edit(PostCategory postCategory);

        void Insert(PostCategory postCategory);

        void Remove(PostCategory postCategory);
    }
}
