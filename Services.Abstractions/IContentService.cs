using Dto;
using Entities.Models;
using ExtentionLinqEntitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Abstractions
{
    public interface IContentService
    {
    
        Task<IEnumerable<Content>> GetAllAsync(IExtendedQuery<Content> expLinqEntity = default, CancellationToken cancellationToken = default);
    
        Task<Content> GetByIdAsync(int Id, IExtendedQuery<Content> expLinqEntity = default, CancellationToken cancellationToken = default);

        Task<ContentDto> CreateAsync(ContentForCreateDto contentForCreateDto, CancellationToken cancellationToken = default);

        Task<ContentDto> UpdateAsync(int Id, ContentForUpdateDto contentForUpdateDto, CancellationToken cancellationToken = default);

        Task<ContentDto> DeleteAsync(int Id, CancellationToken cancellationToken = default);
    }
}
