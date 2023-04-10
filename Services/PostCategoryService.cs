using ATMapper;
using Contracts;
using Dto;
using Services.Abstractions;


namespace Services
{
    public class PostCategoryService : ServiceBase, IPostCategoryService
    {
        public PostCategoryService(IRepositoryWrapper repositoryManager) : base(repositoryManager)
        {
        }

        public async Task<IEnumerable<PostCategoryForWithDetailDto>> GetByIdCategoryWithDetailAsync(string categoryId, CancellationToken cancellationToken = default)
        {
            var postCategory =  await _repositoryManager.PostCategory.GetByIdCategoryWithDetailAsync(categoryId);

            return ObjectMapper.Mapper.Map<IEnumerable<PostCategoryForWithDetailDto>>(postCategory);
        }

        public async Task<IEnumerable<PostCategoryForWithDetailDto>> GetByIdPostWithDetailAsync(string postId, CancellationToken cancellationToken = default)
        {
            var postCategory = await _repositoryManager.PostCategory.GetByIdCategoryWithDetailAsync(postId);

            return ObjectMapper.Mapper.Map<IEnumerable<PostCategoryForWithDetailDto>>(postCategory);
        }
    }
}
