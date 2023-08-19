using Contracts;
using Domain.Reposirory;
using Entities.Models;
using Paging;
using Microsoft.EntityFrameworkCore;
using ExtentionLinqEntitys;

namespace Domain.Repository
{
    public class PostRepository : RepositoryBase<Post>, IPostRepository
    {
        public PostRepository(RepositoryContext repositoryContext)
            : base(repositoryContext)
        {
        }

        public void Insert(Post post)
        {
            Create(post);
        }


        public void Remove(Post post)
        {
            Delete(post);
        }

        public void Edit(Post post)
        {
            Update(post);
        }

        public PagedList<Post> Posts(QueryStringParameters postParameters , IExtendedQuery<Post>expLinqEntity = null)
        {
            return  PagedList<Post>.ToPagedList(Queryable(expLinqEntity).OrderByDescending(on => on.DateCreate),
                    postParameters.PageNumber,
                    postParameters.PageSize);
        }

        public IEnumerable<Post> GetAll(IExtendedQuery<Post> expLinqEntity = null)
        {
            return Queryable(expLinqEntity).ToList();
        }

        public  Post GetById(string postId, IExtendedQuery<Post> expLinqEntity = null)
        {
            return  Queryable(expLinqEntity).Where(x => x.Id == postId).FirstOrDefault();
        }

        public async Task<IEnumerable<Post>> GetAllAsync(IExtendedQuery<Post> expLinqEntity = null,
                                                            CancellationToken cancellationToken = default)
        {
            return await Queryable(expLinqEntity).ToListAsync(cancellationToken);
        }

        public async Task<Post> GetByIdAsync(string postId, IExtendedQuery<Post> expLinqEntity = null,
                                                               CancellationToken cancellationToken = default)
        {
            return await Queryable(expLinqEntity).Where(x => x.Id == postId).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<Post> GetBySlugAsync(string slug, IExtendedQuery<Post> expLinqEntity = null, CancellationToken cancellationToken = default)
        {
            return await Queryable(expLinqEntity).Where(x => x.Slug == slug).FirstOrDefaultAsync(cancellationToken);
        }
    }
}
