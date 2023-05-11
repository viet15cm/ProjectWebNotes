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

        public PagedList<Post> Posts(PostParameters postParameters)
        {
            return  PagedList<Post>.ToPagedList(FindAll().OrderByDescending(on => on.DateCreate),
                    postParameters.PageNumber,
                    postParameters.PageSize);
        }

        public IEnumerable<Post> GetAll(IExpLinqEntity<Post> expLinqEntity = null)
        {
            return Queryable(expLinqEntity).ToList();
        }

        public  Post GetById(string postId, IExpLinqEntity<Post> expLinqEntity = null)
        {
            return  Queryable(expLinqEntity).Where(x => x.Id == postId).FirstOrDefault();
        }

        public async Task<IEnumerable<Post>> GetAllAsync(IExpLinqEntity<Post> expLinqEntity = null,
                                                            CancellationToken cancellationToken = default)
        {
            return await Queryable(expLinqEntity).ToListAsync(cancellationToken);
        }

        public async Task<Post> GetByIdAsync(string postId, IExpLinqEntity<Post> expLinqEntity = null,
                                                               CancellationToken cancellationToken = default)
        {
            return await Queryable(expLinqEntity).Where(x => x.Id == postId).FirstOrDefaultAsync(cancellationToken);
        }
    }
}
