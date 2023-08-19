using Entities;
using Entities.Models;
using ExtentionLinqEntitys;
using Paging;
using System.Linq.Expressions;

namespace Contracts
{
    public interface IPostRepository : IRepositoryBase<Post>
    {

        PagedList<Post> Posts(QueryStringParameters postParameters, IExtendedQuery<Post> expLinqEntity = default);
        void Insert(Post post);
        void Remove(Post post);
        void Edit(Post post);

        IEnumerable<Post> GetAll(IExtendedQuery<Post> expLinqEntity = default);

        Post GetById(string postId, IExtendedQuery<Post> expLinqEntity = default);

        Task<Post> GetBySlugAsync(string slug, IExtendedQuery<Post> expLinqEntity = default, CancellationToken cancellationToken = default);
        
        Task<IEnumerable<Post>> GetAllAsync(IExtendedQuery<Post> expLinqEntity = default, CancellationToken cancellationToken = default);

        Task<Post> GetByIdAsync(string postId, IExtendedQuery<Post> expLinqEntity = default, CancellationToken cancellationToken = default);
    }
}