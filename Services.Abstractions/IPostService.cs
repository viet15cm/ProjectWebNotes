using Dto;
using Entities.Models;
using ExtentionLinqEntitys;
using Paging;

namespace Services.Abstractions
{
    public interface IPostService
    {

        Post GetById(string postId, IExtendedQuery<Post> expLinqEntity = default);

        Task<Post> GetBySlugAsync(string slug, IExtendedQuery<Post> expLinqEntity = default, CancellationToken cancellationToken = default);
        IEnumerable<Post> GetAll(IExtendedQuery<Post> expLinqEntity = default);
        Task<IEnumerable<Post>> GetAllAsync(IExtendedQuery<Post> expLinqEntity = default, CancellationToken cancellationToken = default);

        Task<PagedList<Post>> PostPaging(QueryStringParameters postParameters, IExtendedQuery<Post> expLinqEntity = default);
        
        Task<Post> GetByIdAsync(string postId, IExtendedQuery<Post> expLinqEntity = default, CancellationToken cancellationToken = default);
        PagedList<PostDto> Posts(QueryStringParameters postParameters, IExtendedQuery<Post> expLinqEntity = default);

        Task<PostDto> CreateAsync(PostForCreationDto postForCreationDto, CancellationToken cancellationToken = default);

        Task<PostDto> CreateAsync(PostForCreationDto postForCreationDto , string categoryId, CancellationToken cancellationToken = default);
        Task <PostDto>UpdateAsync(string postId, PostForUpdateDto postForUpdate, CancellationToken cancellationToken = default);

        Task<PostDto> UpdateAsync(string postId, PostForUpdateContentDto postForUpdate, CancellationToken cancellationToken = default);

        Task<PostDto> UpdateAsync(string postId, PostForUpdateBannerDto postForUpdateBannerDto, CancellationToken cancellationToken = default);
        
        Task<PostDto> UpDateAsync(string postId, PostForUpdateIdCategoryDto postForUpdate, CancellationToken cancellationToken = default);
        Task <PostDto>DeleteAsync(string postId, CancellationToken cancellationToken = default);

        Task RemoveFromCategorysAsync(string postId, IEnumerable<string> categoryId);
        Task AddToCategorysAsync(string postId, IEnumerable<string> categoryId);
    }
}
