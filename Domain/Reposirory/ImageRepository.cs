using Contracts;
using Domain.Repository;
using Entities.Models;
using ExtentionLinqEntitys;
using Microsoft.EntityFrameworkCore;

namespace Domain.Reposirory
{
    public class ImageRepository : RepositoryBase<Image>, IImageRepository
    {
        public ImageRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
        }

        public void Edit(Image image)
        {
            Update(image);
        }
  
        public async Task<IEnumerable<Image>> GetAllAsync(IExpLinqEntity<Image> expLinqEntity = null, CancellationToken cancellationToken = default)
        {
           return await Queryable(expLinqEntity).ToListAsync(cancellationToken);
        }
       
        public async Task<Image> GetByIdAsync(int Id, IExpLinqEntity<Image> expLinqEntity = null, CancellationToken cancellationToken = default)
        {
            return await Queryable(expLinqEntity).Where(x => x.Id.Equals(Id)).FirstOrDefaultAsync(cancellationToken);
        }
        public async Task<Image> GetByUrlAsync(string url, IExpLinqEntity<Image> expLinqEntity = null, CancellationToken cancellationToken = default)
        {
            return await Queryable(expLinqEntity).Where(x => x.Url.Equals(url)).FirstOrDefaultAsync(cancellationToken);
        }
        public void Insert(Image image)
        {
            Create(image);
        }

        public void Remove(Image image)
        {
            Delete(image);
        }
    }
}
