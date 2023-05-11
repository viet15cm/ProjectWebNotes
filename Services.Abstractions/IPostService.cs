using Dto;
using Entities.Models;
using ExtentionLinqEntitys;
using Paging;

namespace Services.Abstractions
{
    public interface IPostService
    {

        Post GetById(string postId, IExpLinqEntity<Post> expLinqEntity = default);

        IEnumerable<Post> GetAll(IExpLinqEntity<Post> expLinqEntity = default);
        Task<IEnumerable<Post>> GetAllAsync(IExpLinqEntity<Post> expLinqEntity = default, CancellationToken cancellationToken = default);

        Task<Post> GetByIdAsync(string postId, IExpLinqEntity<Post> expLinqEntity = default, CancellationToken cancellationToken = default);
        PagedList<PostDto> Posts(PostParameters postParameters);

        Task<PostDto> CreateAsync(PostForCreationDto postForCreationDto, CancellationToken cancellationToken = default);

        Task<PostDto> CreateAsync(PostForCreationDto postForCreationDto , string categoryId, CancellationToken cancellationToken = default);
        Task <PostDto>UpdateAsync(string postId, PostForUpdateDto postForUpdate, CancellationToken cancellationToken = default);

        Task<PostDto> UpdateAsync(string postId, PostForUpdateContentDto postForUpdate, CancellationToken cancellationToken = default);

        Task <PostDto>DeleteAsync(string postId, CancellationToken cancellationToken = default);

        Task RemoveFromCategorysAsync(string postId, IEnumerable<string> categoryId);
        Task AddToCategorysAsync(string postId, IEnumerable<string> categoryId);
    }
}
