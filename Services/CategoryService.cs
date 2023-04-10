using ATMapper;
using AutoMapper;
using Contracts;
using Dto;
using Entities.Models;
using Exceptions;
using Services.Abstractions;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

namespace Services
{
    internal sealed class CategoryService : ServiceBase , ICategoryService
    {
        public CategoryService(IRepositoryWrapper repositoryManager) : base(repositoryManager)
        {
        }

        public async Task<CategoryDto> CreateAsync(CategoryForCreationDto categoryDto, CancellationToken cancellationToken = default)
        {
            if (categoryDto == null)
            {
                throw new CategoryNotFoundException(categoryDto.Id);
            }

            if (categoryDto.Id == null)
            {
                categoryDto.Id = Guid.NewGuid().ToString();
            }

            //if (categoryDto.DateUpdated == null)
            //{
            //    categoryDto.DateUpdated = _httpClient.GetNistTime();
            //}

            var category = ObjectMapper.Mapper.Map<Category>(categoryDto);

            _repositoryManager.Category.Insert(category);


            await _repositoryManager.UnitOfWork.SaveChangesAsync();

            return ObjectMapper.Mapper.Map<CategoryDto>(category);
        }

        public async Task<CategoryDto> DeleteAsync(string categoryId, CancellationToken cancellationToken = default)
        {
            if (categoryId == null)
            {
                throw new CategoryIdNullException("Không tải được Id danh Mục.");
            }

            var category = await _repositoryManager.Category.GetByIdWithDetailAsync(categoryId, cancellationToken);

            if (category.CategoryChildren?.Count > 0)
            {
                foreach (var item in category.CategoryChildren)
                {
                    item.ParentCategoryId = category.ParentCategoryId;
                }
            }

            if (category.PostCategories?.Count > 0)
            {
                foreach (var item in category.PostCategories)
                {
                    _repositoryManager.PostCategory.Remove(item);
                }
            }
          
            _repositoryManager.Category.Remove(category);

            await _repositoryManager.UnitOfWork.SaveChangesAsync();

            return ObjectMapper.Mapper.Map<CategoryDto>(category);

        }


        public async Task<IEnumerable<CategoryDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var categorys = await _repositoryManager.Category.GetAllAsync(cancellationToken);
            
            var categorysDtos = ObjectMapper.Mapper.Map<IEnumerable<CategoryDto>>(categorys);
            
            return categorysDtos;
        }

        public async Task<CategoryDto> GetByIdAsync(string categoryId, CancellationToken cancellationToken = default)
        {
            var category = await _repositoryManager.Category.GetByIdAsync(categoryId, cancellationToken);

            if (category is null)
            {
                throw new CategoryNotFoundException(category.Id);
            }

            var categoryDto = ObjectMapper.Mapper.Map<CategoryDto>(category);

            return categoryDto;
        }
        public async Task<CategoryDto> UpdateAsync(string idCategory , 
                                        CategoryForUpdateDto categoryForUpdate , 
                                        CancellationToken cancellationToken)
        {

            var category = await _repositoryManager.Category.GetByIdWithDetailAsync(idCategory, cancellationToken);

            if (category == null)
            {
                throw new CategoryNotFoundException(category.Id);
            }

            if (categoryForUpdate == null)
            {
                throw new CategoryNotFoundException(categoryForUpdate.Title);
            }

            if (categoryForUpdate.ParentCategoryId != null)
            {
                if (category.Id == categoryForUpdate.ParentCategoryId)
                {
                    throw new CategoryIdDuplicatePatentIdException("Lỗi trùng lập Id");
                }
            }

            if (category.CategoryChildren?.Count > 0)
            {
                if (category.ParentCategoryId != categoryForUpdate.ParentCategoryId)
                {
                    foreach (var item in category.CategoryChildren)
                    {
                        item.ParentCategoryId = category.ParentCategoryId;
                    }
                }
            }

            Type TypeCategoryForupdateDto = categoryForUpdate.GetType();

            Type TypeCategory = category.GetType();

            IList<PropertyInfo> props = new List<PropertyInfo>(TypeCategoryForupdateDto.GetProperties());

            foreach (PropertyInfo prop in props)
            {             
                    PropertyInfo propertyInfo = TypeCategory.GetProperty(prop.Name);
                    propertyInfo.SetValue(category, prop.GetValue(categoryForUpdate, null));

            }

            _repositoryManager.Category.Update(category);

            await _repositoryManager.UnitOfWork.SaveChangesAsync(cancellationToken);

            return ObjectMapper.Mapper.Map<CategoryDto>(category);
        }


        public async Task<IEnumerable<CategoryForWithDetailDto>> GetAllWithDetailAsync(CancellationToken cancellationToken = default)
        {
            var data = await _repositoryManager.Category.GetAllWithDetailAsync(cancellationToken);

            return ObjectMapper.Mapper.Map<IEnumerable<CategoryForWithDetailDto>>(data);
        }

        public async Task<CategoryForWithDetailDto> GetByIdWithDetailAsync(string categoryId, CancellationToken cancellationToken = default)
        {
            var data = await _repositoryManager.Category.CategoryInclue(categoryId , new Expression<Func<Category, object>>[] { c => c.ParentCategoryId, c => c.PostCategories });
            
            return ObjectMapper.Mapper.Map<CategoryForWithDetailDto>(data);
        }

    }
}
