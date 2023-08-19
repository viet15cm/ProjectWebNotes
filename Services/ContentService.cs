using ATMapper;
using Contracts;
using Dto;
using Entities.Models;
using Exceptions;
using ExtentionLinqEntitys;
using Microsoft.EntityFrameworkCore;
using Services.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

            await _repositoryManager.UnitOfWork.SaveChangesAsync(cancellationToken);

            return ObjectMapper.Mapper.Map<ContentDto>(content);
        }

        public async Task<ContentDto> DeleteAsync(int Id, CancellationToken cancellationToken = default)
        {

            var content = await _repositoryManager
               .Content
               .GetByIdAsync(Id,
               ExtendedQuery<Content>.Set(ExtendedInclue.Set<Content>(x => x.Include(x => x.ContentChildrens))), cancellationToken);


            if (content.ContentChildrens?.Count > 0)
            {
                
                foreach (var item in content.ContentChildrens)
                {
                    item.ParentContentId = content.ParentContentId;
                }
                
            }

            _repositoryManager.Content.Remove(content);

            await _repositoryManager.UnitOfWork.SaveChangesAsync(cancellationToken);

            return ObjectMapper.Mapper.Map<ContentDto>(content);
        }


        public async Task<IEnumerable<Content>> GetAllAsync(IExtendedQuery<Content> expLinqEntity = null, CancellationToken cancellationToken = default)
        {
            return await _repositoryManager.Content.GetAllAsync(expLinqEntity, cancellationToken);
        }

        public async Task<Content> GetByIdAsync(int Id, IExtendedQuery<Content> expLinqEntity = null, CancellationToken cancellationToken = default)
        {
            return await _repositoryManager.Content.GetByIdAsync(Id, expLinqEntity, cancellationToken);
        }


        public async Task<ContentDto> UpdateAsync(int Id, ContentForUpdateDto contentForUpdateDto, CancellationToken cancellationToken = default)
        {
            var content = await _repositoryManager
                .Content
                .GetByIdAsync(Id,
                ExtendedQuery<Content>.Set(ExtendedInclue.Set<Content>(x => x.Include(x => x.ContentChildrens).Include(x=> x.ParentContent))), cancellationToken);

            if (content == null)
            {
                throw new PostNotFoundException(content.Id.ToString());
            }

            if (content == null)
            {
                throw new PostNotFoundException("Không tìm thấy dữ liệu đầu vào .");
            }

            if (contentForUpdateDto.ParentContentId != null)
            {
                if (content.Id == contentForUpdateDto.ParentContentId)
                {
                    throw new CategoryIdDuplicatePatentIdException("Lỗi trùng lập Id");
                }
            }
            if (content.ContentChildrens?.Count > 0)
            {
                if (content.ParentContentId != contentForUpdateDto.ParentContentId)
                {
                    foreach (var item in content.ContentChildrens)
                    {
                        item.ParentContentId = content.ParentContentId;
                    }
                }
            }

            Type TypeContentForupdateDto = contentForUpdateDto.GetType();

            Type TypeContent = content.GetType();

            IList<PropertyInfo> props = new List<PropertyInfo>(TypeContentForupdateDto.GetProperties());

            foreach (PropertyInfo prop in props)
            {
                PropertyInfo propertyInfo = TypeContent.GetProperty(prop.Name);
                propertyInfo.SetValue(content, prop.GetValue(contentForUpdateDto, null));

            }

            _repositoryManager.Content.Edit(content);
            
            await _repositoryManager.UnitOfWork.SaveChangesAsync(cancellationToken);

            return ObjectMapper.Mapper.Map<ContentDto>(content);
        }
    }
}
