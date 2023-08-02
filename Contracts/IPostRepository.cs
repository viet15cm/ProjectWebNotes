using Entities;
using Entities.Models;
using ExtentionLinqEntitys;
using Paging;
using System.Linq.Expressions;

namespace Contracts
{
    public interface IPostRepository : IRepositoryBase<Post>
    {

        PagedList<Post> Posts(QueryStringParameters postParameters);
        void Insert(Post post);
        void Remove(Post post);
        void Edit(Post post);

        IEnumerable<Post> GetAll(IExpLinqEntity<Post> expLinqEntity = default);

        Post GetById(string postId, IExpLinqEntity<Post> expLinqEntity = default);

        Task<IEnumerable<Post>> GetAllAsync(IExpLinqEntity<Post> expLinqEntity = default, CancellationToken cancellationToken = default);

        Task<Post> GetByIdAsync(string postId, IExpLinqEntity<Post> expLinqEntity = default, CancellationToken cancellationToken = default);
    }
}