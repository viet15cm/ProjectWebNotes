using Entities.Models;
using ExtentionLinqEntitys;

namespace Services.Abstractions
{
    public interface IPostCategoryService
    {
        Task<IEnumerable<PostCategory>> GetByIdCategoryAsync(string categoryId , IExpLinqEntity<PostCategory> expLinqEntity = default, CancellationToken cancellationToken = default);

        Task<IEnumerable<PostCategory>> GetByIdPostAsync(string postId , IExpLinqEntity<PostCategory> expLinqEntity = default, CancellationToken cancellationToken = default);
    }
}
