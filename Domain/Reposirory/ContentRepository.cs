using Contracts;
using Domain.Repository;
using Entities.Models;
using ExtentionLinqEntitys;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Reposirory
{
    public class ContentRepository : RepositoryBase<Content>, IContentRepository
    {
        public ContentRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
        }

        public void Edit(Content category)
        {
            Edit(category);
        }

        public Task<IEnumerable<Content>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Content>> GetAllWithDetailAsync(IExpLinqEntity<Content> expLinqEntity = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Content> GetByIdAsync(int Id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<Content> GetByIdWithDetailAsync(int Id, IExpLinqEntity<Content> expLinqEntity = null, CancellationToken cancellationToken = default)
        {
            return await Queryable(expLinqEntity).Where(x => x.Id == Id).FirstOrDefaultAsync();
        }

        public void Insert(Content content)
        {
            Create(content);
        }

        public void Remove(Content content)
        {
            Delete(content);
        }
    }
}
