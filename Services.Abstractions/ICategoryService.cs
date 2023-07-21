using Dto;
using Entities;
using Entities.Models;
using ExtentionLinqEntitys;

namespace Services.Abstractions
{
    public interface ICategoryService
    {
        Category GetById(string categoryId, IExpLinqEntity<Category> expLinqEntity = default);
        
        IEnumerable<Category> GetAll(IExpLinqEntity<Category> expLinqEntity = default);
        Task<IEnumerable<Category>> GetAllAsync(IExpLinqEntity<Category> expLinqEntity = default, CancellationToken cancellationToken = default);
  
        Task<Category> GetByIdAsync(string categoryId , IExpLinqEntity<Category> expLinqEntity = default, CancellationToken cancellationToken = default);

        Task<Category> GetBySlugAsync(string slug, IExpLinqEntity<Category> expLinqEntity = default, CancellationToken cancellationToken = default);
        Task<CategoryDto> CreateAsync(CategoryForCreationDto categoryDto, CancellationToken cancellationToken = default);
        
        Task<CategoryDto> UpdateAsync(string IdCategory , CategoryForUpdateDto categoryForUpdate, CancellationToken cancellationToken = default);

        Task<CategoryDto> UpdateAsync(string IdCategory, CategoryForUpdateContentDto categoryForUpdate, CancellationToken cancellationToken = default);

        Task<CategoryDto> DeleteAsync(string IdCategory , CancellationToken cancellationToken = default);

    }
}
