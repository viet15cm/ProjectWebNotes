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

        public void Edit(Content content)
        {
            Update(content);
        }

        public  IEnumerable<Content> GetAll()
        {
            return FindAll().ToList();
        }

        public async Task<IEnumerable<Content>> GetAllAsync(IExtendedQuery<Content> expLinqEntity = null, CancellationToken cancellationToken = default)
        {
            return await Queryable(expLinqEntity).ToListAsync(cancellationToken);
        }

        public async Task<Content> GetByIdAsync(int Id , IExtendedQuery<Content> expLinqEntity = null, CancellationToken cancellationToken = default)
        {
            return await Queryable(expLinqEntity).Where(x => x.Id == Id).FirstOrDefaultAsync(cancellationToken);
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
