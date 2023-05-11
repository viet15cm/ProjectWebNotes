using Entities.Models;
using ExtentionLinqEntitys;


namespace Contracts
{
    public interface IImageRepository : IRepositoryBase<Image>
    {
 
        Task<IEnumerable<Image>> GetAllAsync(IExpLinqEntity<Image> expLinqEntity = default, CancellationToken cancellationToken = default);
      
        Task<Image> GetByIdAsync(int Id, IExpLinqEntity<Image> expLinqEntity = default, CancellationToken cancellationToken = default);

        Task<Image> GetByUrlAsync(string url , IExpLinqEntity<Image> expLinqEntity = default, CancellationToken cancellationToken = default);
        void Edit(Image image);

        void Insert(Image image);

        void Remove(Image image);
    }
}
