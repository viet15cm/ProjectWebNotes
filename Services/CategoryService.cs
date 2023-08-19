using ATMapper;
using Contracts;
using Dto;
using Entities.Models;
using Exceptions;
using ExtentionLinqEntitys;
using Microsoft.EntityFrameworkCore;
using Services.Abstractions;
using System.Reflection;

namespace Services
{
    internal class CategoryService : ServiceBase, ICategoryService
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


            await _repositoryManager.UnitOfWork.SaveChangesAsync(cancellationToken);

            return ObjectMapper.Mapper.Map<CategoryDto>(category);
        }

        public async Task<CategoryDto> DeleteAsync(string categoryId, CancellationToken cancellationToken = default)
        {
            if (categoryId == null)
            {
                throw new CategoryIdNullException("Không tải được Id danh Mục.");
            }

            var category = await _repositoryManager
                .Category
                .GetByIdAsync(categoryId, ExtendedQuery<Category>
                .Set(ExtendedInclue.Set<Category>(x => x.Include(x => x.PostCategories).Include(x => x.CategoryChildren))), cancellationToken);

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

            await _repositoryManager.UnitOfWork.SaveChangesAsync(cancellationToken);

            return ObjectMapper.Mapper.Map<CategoryDto>(category);

        }


        public async Task<IEnumerable<Category>> GetAllAsync(IExtendedQuery<Category> expLinqEntity = null ,CancellationToken cancellationToken = default)
        {
             return await _repositoryManager.Category.GetAllAsync(expLinqEntity, cancellationToken);

        }

        public async Task<Category> GetByIdAsync(string categoryId, IExtendedQuery<Category> expLinqEntity = null, CancellationToken cancellationToken = default)
        {
            var category = await _repositoryManager.Category.GetByIdAsync(categoryId, expLinqEntity , cancellationToken);

            if (category is null)
            {
                throw new CategoryNotFoundException(category.Id);
            }

            return category;
        }
        public async Task<CategoryDto> UpdateAsync(string idCategory , 
                                        CategoryForUpdateDto categoryForUpdate , 
                                        CancellationToken cancellationToken)
        {
            var category = await _repositoryManager
                .Category
                .GetByIdAsync(idCategory,
                                        ExtendedQuery<Category>.Set(ExtendedInclue.Set<Category>(x => x.Include(x => x.PostCategories).ThenInclude(x => x.Post))),
                                        cancellationToken);

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


        public Category GetById(string categoryId, IExtendedQuery<Category> expLinqEntity = null)
        {
            return _repositoryManager.Category.GetById(categoryId, expLinqEntity);
        }

        public IEnumerable<Category> GetAll(IExtendedQuery<Category> expLinqEntity = null)
        {
            return _repositoryManager.Category.GetAll(expLinqEntity);
        }

        public async Task<Category> GetBySlugAsync(string slug, IExtendedQuery<Category> expLinqEntity = null, CancellationToken cancellationToken = default)
        {
            var category = await _repositoryManager.Category.GetBySlugAsync(slug, expLinqEntity, cancellationToken);

            if (category is null)
            {
                throw new CategoryNotFoundException(category.Id);
            }

            return category;
        }

        public async Task<CategoryDto> UpdateAsync(string idCategory, CategoryForUpdateContentDto categoryForUpdate, CancellationToken cancellationToken = default)
        {
            var category = await _repositoryManager
               .Category
               .GetByIdAsync(idCategory,
                                       ExtendedQuery<Category>.Set(ExtendedInclue.Set<Category>(x => x.Include(x => x.PostCategories).ThenInclude(x => x.Post))),
                                       cancellationToken);

            if (category == null)
            {
                throw new CategoryNotFoundException(category.Id);
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

        public async Task<CategoryDto> UpdateAsync(string idCategory, CategoryForUpdateIconDto categoryForUpdate, CancellationToken cancellationToken = default)
        {
            var category = await _repositoryManager
              .Category
              .GetByIdAsync(idCategory,
                                      ExtendedQuery<Category>.Set(ExtendedInclue.Set<Category>(x => x.Include(x => x.PostCategories).ThenInclude(x => x.Post))),
                                      cancellationToken);

            if (category == null)
            {
                throw new CategoryNotFoundException(category.Id);
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
    }
}
