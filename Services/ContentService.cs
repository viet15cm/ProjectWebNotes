using ATMapper;
using Contracts;
using Dto;
using Entities.Models;
using Exceptions;
using ExtentionLinqEntitys;
using Services.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class ContentService : ServiceBase, IContentService
    {
        public ContentService(IRepositoryWrapper repositoryManager) : base(repositoryManager)
        {
        }

        public async Task<ContentDto> CreateAsync(ContentForCreateDto contentForCreateDto, CancellationToken cancellationToken = default)
        {
            if (contentForCreateDto is null)
            {

                throw new PostNotFoundException("Dữ liệu trống");
            }

            if (contentForCreateDto.PostId == null)
            {
                throw new PostNotFoundException("Dữ liệu IdPost trống");
            }

            if (contentForCreateDto.KeyTitleId is null)
            {
                contentForCreateDto.KeyTitleId = Guid.NewGuid().ToString().Substring(24);
            }

            var content = ObjectMapper.Mapper.Map<Content>(contentForCreateDto);

            _repositoryManager.Content.Insert(content);

            await _repositoryManager.UnitOfWork.SaveChangesAsync();

            return ObjectMapper.Mapper.Map<ContentDto>(content);
        }

        public Task<ContentDto> DeleteAsync(string Id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ContentDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Content>> GetAllWithDetailAsync(IExpLinqEntity<Content> expLinqEntity = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<ContentDto> GetByIdAsync(string Id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<ContentDto> GetByIdWithDetailAsync(string Id, IExpLinqEntity<Content> expLinqEntity = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<ContentDto> UpdateAsync(string Id, ContentForUpdateDto contentForUpdateDto, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
