using Contracts;
using Domain.Reposirory;
using Entities.Models;
using Paging;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Domain.Repository
{
    public class PostRepository : RepositoryBase<Post>, IPostRepository
    {
        public PostRepository(RepositoryContext repositoryContext)
            : base(repositoryContext)
        {
        }

        public IEnumerable<Post> GetAll()
        {
            return FindAll()
                .OrderBy(x => x.Title)
                .ToList();
        }

        public async Task<IEnumerable<Post>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await FindAll()
                .OrderBy(x => x.Title)
                .ToListAsync();
        }

        public async Task<Post> GetByIdWithDetailAsync(string id, CancellationToken cancellationToken = default)
        {
            return await FindAll()
                        .Include (p=> p.PostCategories)
                        .FirstOrDefaultAsync(p => p.Id == id);
        }


        public async Task<IEnumerable<Post>> GetAllWithDetailAsync(CancellationToken cancellationToken = default)
        {
            return await  FindAll()
                         .Include(p => p.PostChilds)
                         .Include(p => p.PostParent)
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

        public async Task<Post> PostInclue(Expression<Func<Post, object>>[] includeExpressions, string id)
        {
            DbSet<Post> dbSet = RepositoryContext.Set<Post>();

            IQueryable<Post> query = null;
            foreach (var includeExpression in includeExpressions)
            {
                query = dbSet.Include(includeExpression);
            }

            return await query.FirstOrDefaultAsync(x => x.Id == id) ?? await dbSet.FirstOrDefaultAsync(x => x.Id == id);
        }
       
    }
}
