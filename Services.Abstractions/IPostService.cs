using Dto;
using Paging;

namespace Services.Abstractions
{
    public interface IPostService
    {
        Task<IEnumerable<PostDto>> GetAllAsync(CancellationToken cancellationToken = default);
        PagedList<PostDto> Posts(PostParameters postParameters);

        Task<PostForWithDetailDto> GetByIdWithDetailAsync(string postId, CancellationToken cancellationToken = default);
        Task<IEnumerable<PostForWithDetailDto>> GetAllWithDetailAsync(CancellationToken cancellationToken = default);
        Task<PostDto> GetByIdAsync(string postId, CancellationToken cancellationToken = default);
        Task<PostDto> CreateAsync(PostForCreationDto postForCreationDto, CancellationToken cancellationToken = default);

        Task<PostDto> CreateAsync(PostForCreationDto postForCreationDto , string categoryId, CancellationToken cancellationToken = default);
        Task <PostDto>UpdateAsync(string postId, PostForUpdateDto postForUpdate, CancellationToken cancellationToken = default);
        Task <PostDto>DeleteAsync(string postId, CancellationToken cancellationToken = default);

        Task RemoveFromCategorysAsync(string postId, IEnumerable<string> categoryId);
        Task AddToCategorysAsync(string postId, IEnumerable<string> categoryId);
    }
}
