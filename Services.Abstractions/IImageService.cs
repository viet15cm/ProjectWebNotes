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
    public interface  IImageService
    {
        Task<ImageDto> DeleteAsync(int idImage , CancellationToken cancellationToken = default);

        Task<ImageDto> CreateAsync(ImageForCreateDto imageForCreate , CancellationToken cancellationToken = default);

        Task<IEnumerable<ImageDto>> CreateFromAsync(IEnumerable<ImageForCreateDto> imageForCreateDtos, CancellationToken cancellationToken = default);
        
        Task<Image> GetByIdAsync(int idImage ,ExpLinqEntity<Image> expLinqEntity, CancellationToken cancellationToken = default);

        Task<bool> DeleteFormImagesAsync(IList<int> idImage, CancellationToken cancellationToken = default);
    }
}
