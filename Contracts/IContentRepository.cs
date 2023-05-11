using Entities.Models;
using ExtentionLinqEntitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IContentRepository : IRepositoryBase<Content>
    {
     
        Task<IEnumerable<Content>> GetAllAsync(IExpLinqEntity<Content> expLinqEntity = default, CancellationToken cancellationToken = default);
        Task<Content> GetByIdAsync(int Id , IExpLinqEntity<Content> expLinqEntity = default, CancellationToken cancellationToken = default);
      
        void Edit(Content content);

        void Insert(Content content);

        void Remove(Content content);
    }
}
