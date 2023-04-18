using Entities;
using Entities.Models;
using ExtentionLinqEntitys;
using Paging;
using System.Linq.Expressions;

namespace Contracts
{
    public interface IPostRepository : IRepositoryBase<Post>
    {

        PagedList<Post> Posts(PostParameters postParameters);
        void Insert(Post post);
        void Remove(Post post);
        void Edit(Post post);

        Task<IEnumerable<Post>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<IEnumerable<Post>> GetAllWithDetailAsync(IExpLinqEntity<Post> expLinqEntity = default, CancellationToken cancellationToken = default);

        Task<Post> GetByIdAsync(string postId, CancellationToken cancellationToken = default);

        Task<Post> GetByIdWithDetailAsync(string postId , IExpLinqEntity<Post> expLinqEntity = default, CancellationToken cancellationToken = default);

    }
}