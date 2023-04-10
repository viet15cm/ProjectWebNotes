using Dto;

namespace Services.Abstractions
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDto>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<IEnumerable<CategoryForWithDetailDto>> GetAllWithDetailAsync(CancellationToken cancellationToken = default);
        Task<CategoryDto> GetByIdAsync(string categoryId, CancellationToken cancellationToken = default);

        Task<CategoryForWithDetailDto> GetByIdWithDetailAsync(string categoryId, CancellationToken cancellationToken = default);

        Task<CategoryDto> CreateAsync(CategoryForCreationDto categoryDto, CancellationToken cancellationToken = default);
        
        Task<CategoryDto> UpdateAsync(string IdCategory , CategoryForUpdateDto categoryForUpdate, CancellationToken cancellationToken = default);
 
        Task<CategoryDto> DeleteAsync(string IdCategory , CancellationToken cancellationToken = default);

    }
}
