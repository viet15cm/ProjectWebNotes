using ATMapper;
using Contracts;
using Dto;
using Entities.Models;
using ExtentionLinqEntitys;
using Services.Abstractions;
using System.Linq.Expressions;

namespace Services
{
    public class PostCategoryService : ServiceBase, IPostCategoryService
    {
        
        public PostCategoryService(IRepositoryWrapper repositoryManager) : base(repositoryManager)
        {
        }

        public async Task<IEnumerable<PostCategory>> GetByIdCategoryAsync(string categoryId, IExpLinqEntity<PostCategory> expLinqEntity = null, CancellationToken cancellationToken = default)
        {
            return await _repositoryManager.PostCategory.GetByIdCategoryAsync(categoryId, expLinqEntity, cancellationToken);
        }


        public async Task<IEnumerable<PostCategory>> GetByIdPostAsync(string postId, IExpLinqEntity<PostCategory> expLinqEntity = null, CancellationToken cancellationToken = default)
        {
            return await _repositoryManager.PostCategory.GetByIdPostAsync(postId, expLinqEntity, cancellationToken);
        }

      
    }
}
