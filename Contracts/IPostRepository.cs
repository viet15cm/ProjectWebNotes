using Entities.Models;
using Paging;
using System.Linq.Expressions;

namespace Contracts
{
    public interface IPostRepository : IRepositoryBase<Post>
    {

        Task<Post> PostInclue(Expression<Func<Post, object>>[] includeExpressions, string id);
        IEnumerable<Post> GetAll();
        Post GetById(string postId);
        PagedList<Post> Posts(PostParameters postParameters);

        void Insert(Post post);
        void Remove(Post post);
        void Edit(Post post);

        Task<IEnumerable<Post>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<IEnumerable<Post>> GetAllWithDetailAsync(CancellationToken cancellationToken = default);

        Task<Post> GetByIdAsync(string postId, CancellationToken cancellationToken = default);

        Task<Post> GetByIdWithDetailAsync(string postId, CancellationToken cancellationToken = default);

    }
}