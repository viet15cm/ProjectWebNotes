using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IPostCategoryRepository
    {

        Task<IEnumerable<PostCategory>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<IEnumerable<PostCategory>> GetAllWithDetailAsync(CancellationToken cancellationToken = default);

        Task<PostCategory> GetByIdAsync(string categoryId, string postId, CancellationToken cancellationToken = default);

        Task<IEnumerable<PostCategory>> GetByIdCategoryWithDetailAsync(string categoryId, CancellationToken cancellationToken = default);
        Task<IEnumerable<PostCategory>> GetByIdPostWithDetailAsync(string postId, CancellationToken cancellationToken = default);

        Task<PostCategory> GetByIdWithDetailAsync(string categoryId, string postId, CancellationToken cancellationToken = default);

        void Edit(PostCategory postCategory);

        void Insert(PostCategory postCategory);

        void Remove(PostCategory postCategory);
    }
}
