using Dto;
using Entities;
using Entities.Models;
using ExtentionLinqEntitys;

namespace Services.Abstractions
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDto>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<IEnumerable<Category>> GetAllWithDetailAsync(IExpLinqEntity<Category> expLinqEntity = default ,CancellationToken cancellationToken = default);
        Task<CategoryDto> GetByIdAsync(string categoryId, CancellationToken cancellationToken = default);

        Task<Category> GetByIdWithDetailAsync(string categoryId , IExpLinqEntity<Category> expLinqEntity = default, CancellationToken cancellationToken = default);

        Task<CategoryDto> CreateAsync(CategoryForCreationDto categoryDto, CancellationToken cancellationToken = default);
        
        Task<CategoryDto> UpdateAsync(string IdCategory , CategoryForUpdateDto categoryForUpdate, CancellationToken cancellationToken = default);
 
        Task<CategoryDto> DeleteAsync(string IdCategory , CancellationToken cancellationToken = default);

    }
}
