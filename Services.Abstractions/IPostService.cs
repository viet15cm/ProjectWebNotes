using Dto;
using Entities.Models;
using ExtentionLinqEntitys;
using Paging;

namespace Services.Abstractions
{
    public interface IPostService
    {
        Task<IEnumerable<PostDto>> GetAllAsync(CancellationToken cancellationToken = default);
        PagedList<PostDto> Posts(PostParameters postParameters);

        Task<Post> GetByIdWithDetailAsync(string postId, IExpLinqEntity<Post> expLinqEntity = default, CancellationToken cancellationToken = default);
        Task<IEnumerable<Post>> GetAllWithDetailAsync(IExpLinqEntity<Post> expLinqEntity = default, CancellationToken cancellationToken = default);
        Task<PostDto> GetByIdAsync(string postId, CancellationToken cancellationToken = default);
        Task<PostDto> CreateAsync(PostForCreationDto postForCreationDto, CancellationToken cancellationToken = default);

        Task<PostDto> CreateAsync(PostForCreationDto postForCreationDto , string categoryId, CancellationToken cancellationToken = default);
        Task <PostDto>UpdateAsync(string postId, PostForUpdateDto postForUpdate, CancellationToken cancellationToken = default);

        Task<PostDto> UpdateAsync(string postId, PostForUpdateContentDto postForUpdate, CancellationToken cancellationToken = default);

        Task <PostDto>DeleteAsync(string postId, CancellationToken cancellationToken = default);

        Task RemoveFromCategorysAsync(string postId, IEnumerable<string> categoryId);
        Task AddToCategorysAsync(string postId, IEnumerable<string> categoryId);
    }
}
