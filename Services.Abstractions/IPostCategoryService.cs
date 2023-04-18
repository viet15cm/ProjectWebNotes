using Entities.Models;
using ExtentionLinqEntitys;

namespace Services.Abstractions
{
    public interface IPostCategoryService
    {
        Task<IEnumerable<PostCategory>> GetByIdCategoryWithDetailAsync(string categoryId , IExpLinqEntity<PostCategory> expLinqEntity = default, CancellationToken cancellationToken = default);

        Task<IEnumerable<PostCategory>> GetByIdPostWithDetailAsync(string postId , IExpLinqEntity<PostCategory> expLinqEntity = default, CancellationToken cancellationToken = default);
    }
}
