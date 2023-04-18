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
        Task<IEnumerable<ContentDto>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<IEnumerable<Content>> GetAllWithDetailAsync(IExpLinqEntity<Content> expLinqEntity = default, CancellationToken cancellationToken = default);
        Task<ContentDto> GetByIdAsync(string Id, CancellationToken cancellationToken = default);

        Task<ContentDto> GetByIdWithDetailAsync(string Id, IExpLinqEntity<Content> expLinqEntity = default, CancellationToken cancellationToken = default);

        Task<ContentDto> CreateAsync(ContentForCreateDto contentForCreateDto, CancellationToken cancellationToken = default);

        Task<ContentDto> UpdateAsync(string Id, ContentForUpdateDto contentForUpdateDto, CancellationToken cancellationToken = default);

        Task<ContentDto> DeleteAsync(string Id, CancellationToken cancellationToken = default);
    }
}
