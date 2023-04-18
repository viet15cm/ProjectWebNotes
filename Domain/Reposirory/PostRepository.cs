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

        public async Task<IEnumerable<Post>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await FindAll()
                .OrderBy(x => x.Title)
                .ToListAsync();
        }

      
        public Post GetById(string postId)
        {
            return FindByCondition(owner => owner.Id.Equals(postId))
                    .FirstOrDefault();
        }
        
        public async Task<Post> GetByIdAsync(string postId, CancellationToken cancellationToken = default)
        {
            return await FindByCondition(owner => owner.Id.Equals(postId))
                        .FirstOrDefaultAsync();
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
            return  PagedList<Post>.ToPagedList(FindAll().OrderBy(on => on.Serial),
                    postParameters.PageNumber,
                    postParameters.PageSize);
        }

        public async Task<IEnumerable<Post>> GetAllWithDetailAsync(IExpLinqEntity<Post> expLinqEntity = null, CancellationToken cancellationToken = default)
        {
            return await Queryable(expLinqEntity).ToListAsync();
        }

        public async Task<Post> GetByIdWithDetailAsync(string postId, IExpLinqEntity<Post> expLinqEntity = null, CancellationToken cancellationToken = default)
        {
            return  await Queryable(expLinqEntity).Where(x => x.Id == postId).FirstOrDefaultAsync();   
        }
    }
}
